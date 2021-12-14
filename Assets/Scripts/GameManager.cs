using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int numOfWrongAdjustments;
    public string nextScene;

    [Header("Configurations")]
    [SerializeField] private int currentLevelIndex;
    [SerializeField] private GameObject[] levels;
    [SerializeField] private GameObject levelForDebugging;
    [SerializeField] private Sprite[] globalSliderSprites;

    [Header("Scene References")]
    [SerializeField] private Transform globalSlider;
    [SerializeField] private Transform stepUI;
    [SerializeField] private GameObject globalTarget, bullies;
    [SerializeField] private GameObject handModel;
    [SerializeField] private Animator transitionAnim;
    [SerializeField] private GameObject playerFeedback;
    [SerializeField] private Sprite positiveFeedback, negativeFeedback;
    [SerializeField] private Image feedbackImage, globalSliderImage;
    [SerializeField] private GameObject playerFeedback2_greenZone, playerFeedback2_redZone, playerFeedback2_yellowZone, playerFeedback2_whiteZone;
    [SerializeField] private GameObject tutorialFlip, tutorialResize;
    [SerializeField] private GameObject sessionChecker, mainGameEnvironment, menuEnvironment;

    [SerializeField] private ToggleMuteButton toggleMuteButtonScript;
    [SerializeField] private ToggleHapticButton toggleHapticButtonScript;
    [SerializeField] private GameObject[] objectsToDeactivateUponStart;

    [Header("For level tracker")]
    [SerializeField] private float[] levelTrackerSliderValues;
    [SerializeField] private TextMeshProUGUI[] levelTexts;
    [SerializeField] private GameObject[] giftImages;
    [SerializeField] private Image levelSlider;
    [SerializeField] private GameObject levelTracker, toolNotif;
    [SerializeField] private TextMeshProUGUI levelSliderText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Transform unlockablesParent;
    
    public Transform GlobalSlider => globalSlider;
    public GameObject GlobalTarget => globalTarget;
    public GameObject HandModel => handModel;

    private LevelManager levelManagerScript;

    private bool levelDebugMode;

    private bool skipIntro;

    private bool resizeTutorialShown, flipTutorialShown;

    private bool firstTimeLoad;
    public int CurrentLevel => currentLevelIndex;
    private void Awake()
    {
        instance = this;

        if(GameObject.FindGameObjectsWithTag("SessionChecker").Length <= 0)
        {
            GameObject sessionCheck = Instantiate(sessionChecker, null, false);
            DontDestroyOnLoad(sessionCheck);
            firstTimeLoad = true;
            skipIntro = false;
        }
        else
        {
            firstTimeLoad = false;
            skipIntro = true;
        }
    }

    private void Start()
    {
        //Camera.main.GetComponent<AudioListener>().enabled = !SoundManager.instance.muted;
        CheckLevel();
        SpawnNewLevel();
    }

    private void CheckLevel()
    {
        if (currentLevelIndex <= -1)
        {
            levelDebugMode = false;
            // if no player prefs for level yet, set to 0
            if (!PlayerPrefs.HasKey("level"))
                PlayerPrefs.SetInt("level", 0);

            currentLevelIndex = PlayerPrefs.GetInt("level");
        }
        else
            levelDebugMode = true;
    }
    private void SpawnNewLevel()
    {
        if (levelForDebugging != null)
        {
            levelForDebugging.GetComponent<LevelManager>().StartLevel();
            return;
        }

        GameObject newLevel = Instantiate(levels[currentLevelIndex % levels.Length] , transform, false);
        levelManagerScript = newLevel.GetComponent<LevelManager>();


        if (currentLevelIndex == 0 /*|| skipIntro*/) // uncomment if you want to turn back the menu skipping
            levelManagerScript.skipIntro = true;


        levelManagerScript.StartLevel();
        UnlockSystem.instance.SetLevel(currentLevelIndex);
        toggleMuteButtonScript.Setup();
        toggleHapticButtonScript.Setup();


        SetLevelHeader2();

        if (levelManagerScript.showUpgradePanel && !firstTimeLoad)
            UpgradeManager.instance?.ToUpgradePanel();

        //CLIK_Altplay.instance.LevelUp(currentLevelIndex + 1);
    }

    public void SetStepUI(Transform newStepUI)
    {
        stepUI = newStepUI;
    }

    public void StartGame(bool skipIntro = false)
    {
        if (currentLevelIndex == 0)
            CLIK_Altplay.instance.LevelUp(currentLevelIndex + 1);

        CLIK_Altplay.instance.MissionStarted(currentLevelIndex + 1);

        StartCoroutine(StartGameRoutine(skipIntro));
    }

    private IEnumerator StartGameRoutine(bool skipIntro = false)
    {
        foreach (GameObject o in objectsToDeactivateUponStart)
            o.SetActive(false);

        if (!skipIntro)
        {
            levelTracker.SetActive(false);
            TriggerTransition(true);
            yield return new WaitForSeconds(1);
            mainGameEnvironment.SetActive(true);
            menuEnvironment.SetActive(false);
            NextStep();
            bullies.SetActive(false);
            TriggerTransition(false);
            LevelManager.instance.characterAnim.SetTrigger("start");
        }
        else
        {
            LevelManager.instance.characterAnim.SetTrigger("start");
            yield return new WaitForEndOfFrame();
            levelTracker.SetActive(false);
            mainGameEnvironment.SetActive(true);
            menuEnvironment.SetActive(false);
            NextStep();
        }

        yield return new WaitForSeconds(0.5f);
        
        globalTarget.SetActive(true);
    }


    public void NextStep()
    {
        StepsManager.instance.NextStep();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [ContextMenu("Force next level")]

    public void NextLevel()
    {
        if (levelDebugMode)
        {
            RestartLevel();
            return;
        }
        else
        {
            CLIK_Altplay.instance.MissionCompleted(currentLevelIndex + 1);
            currentLevelIndex++;
            PlayerPrefs.SetInt("level", currentLevelIndex);
            CLIK_Altplay.instance.LevelUp(currentLevelIndex + 1);
        }

        RestartLevel();
    }

    public void TriggerTransition(bool isIn)
    {
        if (isIn)
            transitionAnim.gameObject.SetActive(true);
        else
        {
            transitionAnim.SetTrigger("out");
            Invoke(nameof(DisableTransition), 3);
        }

    }

    public void DisableTransition()
    {
        transitionAnim.gameObject.SetActive(false);

    }

    public void TriggerFeedback(bool isCorrect, string color = null)
    {
        StartCoroutine(FeedbackRoutine(isCorrect, color));
    }

    private IEnumerator FeedbackRoutine(bool isCorrect, string color = null)
    {
        //if (isCorrect)
        //    SoundManager.instance.GirlHappySound();
        //else
        //    SoundManager.instance.GirlSadSound();

        feedbackImage.sprite = isCorrect ? positiveFeedback : negativeFeedback;


        playerFeedback.SetActive(false);
        if (color == null)
        {
            if (isCorrect)
                playerFeedback2_greenZone.SetActive(false);
            else
                playerFeedback2_redZone.SetActive(false);

            yield return new WaitForEndOfFrame();

            if (isCorrect)
                playerFeedback2_greenZone.SetActive(true);
            else
                playerFeedback2_redZone.SetActive(true);

        }
        else
        {
            if (color == "green")
                playerFeedback2_greenZone.SetActive(false);
            else if (color == "red")
                playerFeedback2_redZone.SetActive(false);
            else if (color == "yellow")
                playerFeedback2_yellowZone.SetActive(false);
            else
                playerFeedback2_whiteZone.SetActive(false);

            yield return new WaitForEndOfFrame();

            if (color == "green")
                playerFeedback2_greenZone.SetActive(true);
            else if (color == "red")
                playerFeedback2_redZone.SetActive(true);
            else if (color == "yellow")
                playerFeedback2_yellowZone.SetActive(true);
            else
                playerFeedback2_whiteZone.SetActive(true);
        }

        playerFeedback.SetActive(true);

    }

    public void SetGlobalSliderSprite(int index)
    {
        globalSliderImage.sprite = globalSliderSprites[index];
    }

    public void ShowTutorial(string tutorialName)
    {

        if (tutorialName == "flip" && !flipTutorialShown)
        {
            flipTutorialShown = true;
            tutorialFlip.SetActive(true);
        }
        else if (tutorialName == "resize" && !resizeTutorialShown)
        {
            resizeTutorialShown = true;
            tutorialResize.SetActive(true);
        }
    }

    public bool CheckIfLevelLooped()
    {
        return currentLevelIndex >= levels.Length;
    }
    
    [ContextMenu("Set level Header")]
    public void SetLevelHeader()
    {
        float positionInHeader = (currentLevelIndex) % 5;

        float startingLevelInheader = (5 * Mathf.FloorToInt((currentLevelIndex) / 5));

        Debug.Log(positionInHeader);
        Debug.Log(startingLevelInheader);

        foreach (GameObject o in giftImages)
            o.SetActive(false);

        int nextUnlockIndex = UnlockSystem.instance.GetCurrentUnlockableInfo().levelIndexToUnlock;

        for (int i = 0; i < levelTexts.Length; i++)
        {
            levelTexts[i].text = ((startingLevelInheader + i) + 1).ToString();

            //if (UnlockSystem.instance.currentUnlockableIndex == (startingLevelInheader + i))
            //    giftImages[i].SetActive(true);

            if (startingLevelInheader + i == nextUnlockIndex)
                giftImages[i].SetActive(true);

        }

        levelSlider.fillAmount = levelTrackerSliderValues[(int)positionInHeader];



        //if (CheckIfLevelLooped())
        //    return;

    }

    [ContextMenu("Set level Header2")]
    public void SetLevelHeader2()
    {
        if (UnlockSystem.instance.allIsUnlocked)
        {
            levelTracker.SetActive(false);
        }

        levelSlider.fillAmount = UnlockSystem.instance.fromTo.x / 100;
        levelSliderText.text = $"{UnlockSystem.instance.fromTo.x}%";
        //if (CheckIfLevelLooped())
        //    return;

        if (levelSlider.fillAmount <= 0)
        {
            Debug.Log("TOOL NOTIF");
            toolNotif.SetActive(true);
        }

        levelText.text = $"Day {currentLevelIndex + 1}";

        float currentUnlockableIndex = UnlockSystem.instance.currentUnlockableIndex;

        if (UnlockSystem.instance.allIsUnlocked)
            currentUnlockableIndex = 100;

        Debug.Log(currentUnlockableIndex);

        for (int i = 0; i < unlockablesParent.childCount; i++)
        {
            if (i < currentUnlockableIndex)
                unlockablesParent.GetChild(i).GetChild(0).gameObject.SetActive(false);
            else
                unlockablesParent.GetChild(i).GetChild(0).gameObject.SetActive(true);

        }

    }
}