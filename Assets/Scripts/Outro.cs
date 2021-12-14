using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;

public class Outro : Step
{
    [SerializeField] private int failNumberToLose = 2;
    [SerializeField] private ParticleSystem endConfettiFX;
    [SerializeField] private GameObject nextButton, retryButton;
    [SerializeField] private GameObject[] objectsToActivate, objectsToDeactivate;
    [SerializeField] private GameObject failUI;
    [SerializeField] private SkinnedMeshRenderer tops, hoodie;
    [SerializeField] private bool forceHappy2;

    [Header("Win scene")]
    [SerializeField] private Transform winScene;
    [SerializeField] private GameObject fader;

    [Header("For Recap UI")]
    [SerializeField] private GameObject recapUI;
    [SerializeField] private Image starsMeter;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private GameObject starGlow;
    [SerializeField] private GameObject recapUINextButton;
    [SerializeField] private GameObject flyingCoinPrefab;
    [SerializeField] private RectTransform header;
    [SerializeField] private RectTransform coinImage;
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private Animator balanceAnimator;
    [SerializeField] private GameObject coinOriginObject;
    [SerializeField] private TextMeshProUGUI coinOriginCount;
    [SerializeField] private TextMeshProUGUI friendsCount;
    [SerializeField] private TextMeshProUGUI likesCount;
    [SerializeField] private Image commentor;
    [SerializeField] private Image commentBubble;
    [SerializeField] private TextMeshProUGUI commentText;
    [SerializeField] private Button sellButton;

    [Header("For Unlock UI")]
    [SerializeField] private GameObject unlockUI;
    [SerializeField] private GameObject unlockableObject;
    [SerializeField] private TextMeshProUGUI unlockSliderText;
    [SerializeField] private Slider unlockSlider;
    [SerializeField] private GameObject unlockNextButton;
    [SerializeField] private GameObject unlockableHeader, newUnlockHeader, glowObjectUnlock;

    [Header("For Squid Game Level")]
    public bool useSquidGameLose;
    [SerializeField] private Animator girlAnimator, guard1, guard2;
    [SerializeField] private GameObject failUISquid;

    private int currentbalance;
    private bool canSkipCoin, skippedCoin;
    private bool sell;
    public override void OnStepStart()
    {
        base.OnStepStart();
        canSkipCoin = false;
        skippedCoin = false;
        StartCoroutine(RecapRoutine());
    }

    private void Update()
    {
        if (canSkipCoin && Input.GetMouseButtonDown(0))
            skippedCoin = true;
    }
    private IEnumerator RecapRoutine()
    {
        currentbalance = PlayerPrefs.GetInt("balance");
        balanceText.text = currentbalance.ToString();

        foreach (GameObject o in objectsToActivate)
            o.SetActive(true);

        foreach (GameObject o in objectsToDeactivate)
            o.SetActive(false);

        GameManager.instance.GlobalTarget.GetComponent<Animator>().SetTrigger("out");

        yield return new WaitForSeconds(1f);

        bool isWin = GameManager.instance.numOfWrongAdjustments <= failNumberToLose;
        

        if (useSquidGameLose && !isWin)
        {
            girlAnimator.SetTrigger("squidGameLose");
            guard1.SetTrigger("fail");
            guard2.SetTrigger("fail");
            yield return new WaitForSeconds(2.5f);
            failUISquid.SetActive(true);
            yield break;
        }

        int numOfCoins = LevelManager.instance.coinsAfterWinning;

        if (isWin) numOfCoins *= 3;

        PlayerPrefs.SetInt("balance", PlayerPrefs.GetInt("balance") + (numOfCoins * 10));

        if (!isWin)
        {
            // send win log also even if 1 star only
            //CLIK_Altplay.instance.MissionCompleted(GameManager.instance.CurrentLevel + 1);
            //CLIK_Altplay.instance.MissionFailed(GameManager.instance.CurrentLevel + 1);
            LevelManager.instance.characterAnim.SetTrigger("cry");
            LevelManager.instance.headAnim.SetTrigger("sad");
        }
        else
        {

            //CLIK_Altplay.instance.MissionCompleted(GameManager.instance.CurrentLevel + 1);
            if (tops.GetBlendShapeWeight(1) >= 50)
                LevelManager.instance.characterAnim.SetTrigger("happy_" + Random.Range(0, 6));
            else if (tops.gameObject.activeInHierarchy)
                LevelManager.instance.characterAnim.SetTrigger("happy2_" + Random.Range(0,3));
            else if (hoodie.GetBlendShapeWeight(3) >= 50 || hoodie.GetBlendShapeWeight(13) >= 50)
                LevelManager.instance.characterAnim.SetTrigger("happy_" + Random.Range(0, 6));
            else if (hoodie.gameObject.activeInHierarchy)
                LevelManager.instance.characterAnim.SetTrigger("happy2_" + Random.Range(0, 3));
            else if (forceHappy2)
                LevelManager.instance.characterAnim.SetTrigger("happy2_" + Random.Range(0, 3));
            else
                LevelManager.instance.characterAnim.SetTrigger("happy_" + Random.Range(0, 6));

            LevelManager.instance.headAnim.SetTrigger("happy");

        }

        SoundManager.instance.TriggerSound(5);
        endConfettiFX.Play();
        yield return new WaitForSeconds(1f);
        //GameManager.instance.TriggerTransition(true);

        if (useSquidGameLose)
            header.anchoredPosition = new Vector2(header.anchoredPosition.x, -52.3f);

        recapUI.SetActive(true);

        int starsCount = 0;
        
        yield return new WaitForSeconds(0.2f);
        stars[0].gameObject.SetActive(true);
        starsCount++;
        AddFriendAndLikesCount(starsCount);
        SoundManager.instance.TriggerSound(4);
        HapticManager.instance.LightHaptic();
        yield return new WaitForSeconds(0.1f);

        StartCoroutine(StarsMeter(isWin));

        IEnumerator StarsMeter(bool isWin)
        {
            float maxFill = isWin ? 1 : .225f;
            
            for (float i = 0; i < maxFill; i += 0.05f)
            {
                starsMeter.fillAmount = i;
                yield return new WaitForSeconds(0.01f);
            }
        }
        
        if (isWin)
        {
            yield return new WaitForSeconds(0.05f);
            SoundManager.instance.TriggerSound(4);
            
            stars[1].gameObject.SetActive(true);
            starsCount++;
            AddFriendAndLikesCount(starsCount);
            HapticManager.instance.LightHaptic();
            
            yield return new WaitForSeconds(0.2f);
            starsCount++;
            AddFriendAndLikesCount(starsCount);
            stars[2].gameObject.SetActive(true);
            AddFriendAndLikesCount(3);
            SoundManager.instance.TriggerSound(4);

            HapticManager.instance.LightHaptic();
            yield return new WaitForSeconds(0.35f);
        }
        
        // comments
        yield return new WaitForSeconds(0.5f);
        commentor.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        commentBubble.gameObject.SetActive(true);
        commentText.text = GetComment(starsCount);
        
        //starGlow.SetActive(true);

        yield return new WaitForSeconds(1f);
        sellButton.gameObject.SetActive(true);
        
        // COOOOOOINSSSSSS
        int numOfCoins2 = numOfCoins;
        coinOriginCount.text = (numOfCoins2 * 10).ToString();
        coinOriginObject.SetActive(true);

        yield return new WaitUntil(() => sell);
        sellButton.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(0.1f);
        canSkipCoin = true;
        for (int i = numOfCoins; i > 0; i--)
        {
            GameObject flyingCoin = Instantiate(flyingCoinPrefab, recapUI.transform, false);
            flyingCoin.GetComponent<FlyingCoins>().SetTarget(coinImage, this);
            coinOriginCount.text = (numOfCoins2 * 10).ToString();
            numOfCoins2 -= 1;
            
            if (skippedCoin)
            {
                DestroyImmediate(flyingCoin);
                SoundManager.instance.TriggerSound(0);
                balanceAnimator.SetTrigger("pop");
                coinOriginObject.SetActive(false);
                currentbalance += (10 * i);
                balanceText.text = currentbalance.ToString();
                i = 0;
                canSkipCoin = false;
            }

            yield return new WaitForSeconds(0.075f);
        }

        coinOriginCount.text = "0";
        coinOriginObject.SetActive(false);
        canSkipCoin = false;

        recapUINextButton.SetActive(true);
        #region old algo
        //if (GameManager.instance.numOfWrongAdjustments < 2)
        //{
        //    LevelManager.instance.CharacterObject.GetComponentInChildren<Animator>().SetTrigger("happy");

        //    if (winScene != null)
        //    {
        //        fader.SetActive(true);
        //        winScene.gameObject.SetActive(true);
        //        Utility.instance.SetCinemachineCameraSolo(winScene.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>());
        //    }
        //    endConfettiFX.Play();
        //    yield return new WaitForSeconds(1f);
        //    nextButton.SetActive(true);
        //}
        //else
        //{
        //    LevelManager.instance.CharacterObject.GetComponentInChildren<Animator>().SetTrigger("cry");
        //    yield return new WaitForSeconds(0.5f);
        //    failUI.SetActive(true);
        //}
        #endregion
    }

    private void AddFriendAndLikesCount(int stars)
    {
        System.Random rand = new System.Random();

        int friend = int.Parse(friendsCount.text);
        int likes = int.Parse(likesCount.text);

        switch (stars)
        {
            case 1:
                friend += rand.Next(1, 8);
                likes += rand.Next(3, 15);
                break;
            case 2:
                friend += rand.Next(12, 30);
                likes += rand.Next(20, 45);
                break;
            case 3:
                friend += rand.Next(40, 70);
                likes += rand.Next(60, 150);
                break;
        }
        
        friendsCount.text = $"+{friend}";
        likesCount.text = $"+{likes}";
    }

    private string GetComment(int star)
    {
        System.Random rand = new System.Random();
        
        string[] star1Comment = {"Hmm. Needs more style I guess.", "I know you can do better next time."};
        string[] star2Comment = {"Nice!\nWould love to try this style!", "Lovely! You know your outfit well!" };
        string[] star3Comment = {"Gorgeous!\nYou have the style!", "This is GODLY!", "OMG!!\nLooks amazing!" };

        if (star == 1)
            return star1Comment[rand.Next(0, star1Comment.Length)];
        if (star == 2)
            return star2Comment[rand.Next(0, star2Comment.Length)];
        if (star == 3)
            return star3Comment[rand.Next(0, star3Comment.Length)];

        return null;
    }

    public void OutroNextButton()
    {
        SoundManager.instance.TriggerSound(0);
        GameManager.instance.NextLevel();
    }

    public void AddCoin()
    {
        balanceAnimator.SetTrigger("pop");
        currentbalance += 10;
        balanceText.text = currentbalance.ToString();
    }

    public void SellButton()
    {
        sell = true;
    }

    public void RecapUINextButton()
    {
        if (!UnlockSystem.instance.allIsUnlocked)
        {
            SoundManager.instance.TriggerSound(0);
            ShowUnlockPanel();
        }
        else
            OutroNextButton();
    }

    private void ShowUnlockPanel()
    {
        recapUI.SetActive(false);
        StartCoroutine(UnlockRoutine());
    }

    private IEnumerator UnlockRoutine()
    {
        unlockableObject.GetComponent<Image>().sprite = UnlockSystem.instance.shadowSprite;
        unlockableObject.transform.GetChild(0).GetComponent<Image>().sprite = UnlockSystem.instance.fillSprite;

        Image unlockableFill = unlockableObject.transform.GetChild(0).GetComponent<Image>();
        unlockableFill.fillAmount = UnlockSystem.instance.fromTo.x / 100;
        unlockSlider.value = UnlockSystem.instance.fromTo.x / 100;
        unlockSliderText.text = $"{UnlockSystem.instance.fromTo.x}%";

        unlockNextButton.SetActive(false);
        unlockUI.SetActive(true);
        yield return new WaitForSeconds(0.3f);

        unlockableObject.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        float runningValue = UnlockSystem.instance.fromTo.x;

        HapticManager.instance.StartContinousHaptic();
        while (runningValue < UnlockSystem.instance.fromTo.y)
        {
            runningValue++;
            unlockableFill.fillAmount = runningValue / 100;
            unlockSlider.value = runningValue / 100;
            unlockSliderText.text = $"{runningValue}%";
            yield return new WaitForSeconds(0.005f);
        }
        HapticManager.instance.StopContinousHaptic();

        if (runningValue >= 100)
        {
            unlockableObject.SetActive(false);
            unlockableObject.GetComponent<Animator>().enabled = true;
            unlockableObject.GetComponent<Image>().sprite = UnlockSystem.instance.shadowSprite;
            unlockableObject.transform.GetChild(0).GetComponent<Image>().sprite = UnlockSystem.instance.fillSprite;
            unlockableHeader.SetActive(false);
            unlockSlider.gameObject.SetActive(false);
            newUnlockHeader.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            HapticManager.instance.HeavyHaptic();
            SoundManager.instance.TriggerSound(3);
            glowObjectUnlock.SetActive(true);
            unlockableObject.SetActive(true);
        }

        unlockNextButton.SetActive(true);
    }
}
