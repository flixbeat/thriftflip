using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Intro : Step
{
    [SerializeField] private Image targetPictureContainer;
    [SerializeField] private Animator targetPictureAnim;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject vipObject;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI numberText;

    private bool canClick, mouseDown;

    public override void OnStepStart()
    {
        base.OnStepStart();

        ChangeOutfit.instance.initialClothes();

        numberText.text = Random.Range(1, 10).ToString() + "$";

        Invoke(nameof(showDialogue), 1f);

        canClick = false;
        targetPictureContainer.sprite = LevelManager.instance.targetPicture;
        GameManager.instance.GlobalTarget.transform.GetChild(0).GetComponent<Image>().sprite = LevelManager.instance.targetPicture;

        if (LevelManager.instance.skipIntro)
        {
            StartGame(true);
            return;
        }

        //if (LevelManager.instance.VIPLevel)
        //    StepsManager.instance.GetCurrentStepCamera().transform.position = new Vector3(0, 1.7f, -1.94f);

        Invoke(nameof(ShowVIP), 1);

        targetPictureContainer.transform.parent.gameObject.SetActive(true);
        //Invoke(nameof(ShowNextButton), 2.5f);
        Invoke(nameof(ShowNextButton), 1f);
    }


    private void ShowVIP()
    {
        vipObject.SetActive(LevelManager.instance.VIPLevel);
    }
    private void ShowNextButton()
    {
        canClick = true;
        nextButton.SetActive(true);
    }

    private void Update()
    {
        if (Input.touchCount > 0 && (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
        }
        if (Input.GetMouseButtonUp(0))
        {


            if (canClick && mouseDown)
            {
                vipObject.SetActive(false);
                StartGame();
            }
        }
    }

    public void StartGame(bool skipIntro = false)
    {
        canClick = false;
        if(!skipIntro)
        {
            targetPictureAnim.SetTrigger("out");
            HapticManager.instance.ButtonHaptic();
            SoundManager.instance.TriggerSound(0);
        }

        nextButton.SetActive(false);
        GameManager.instance.StartGame(skipIntro);
        
    }


    private void showDialogue()
    {
        dialogueBox.SetActive(true);
    }

    public override void OnStepEnd()
    {
        base.OnStepEnd();
        ChangeOutfit.instance.changeClothes();
    }
    
}
