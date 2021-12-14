using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClotheCutter : MonoBehaviour
{
    [Header("References")]
    public bool skip;
    public ClotheResizerInfo info;
    public SkinnedMeshRenderer flyRenderer;
    public int cutFlyBlendshapeIndex;
    public Vector2 cutFlyFromTo = new Vector2(0, 100);
    public GameObject[] objectsToHideAfter, objectsToShowAfter;
    public Sprite handleSpriteNormal, handleSpriteGreen;
    public bool disableHide;
    public Transform scissorAnimator;
    public Renderer lineRenderer;
    private Slider slider;
    private Animator sliderAnim;
    private bool isActivated, mouseDown;
    private CutClothes triggererScript;
    private GameObject handModel;
    private GameObject sliderParent;
    private Vector3 lastMousePosition;
    private bool isUsingGlobalSlider;
    public bool isDone { get; private set; }
    private Image sliderHandle1, sliderHandle2;
    private float slideValue, localSpeed;

    private string currentResult;
    private void Start()
    {
        isActivated = false;
        mouseDown = false;
        if (info.pin != null)
            info.pin.SetActive(false);

        if (info.sliderParent != null)
            info.sliderParent.SetActive(false);
    }

    private void ChangeHandleSprite()
    {
        if (Mathf.Abs(slider.value - ((info.greenValueMinMax.x + info.greenValueMinMax.y) / 2)) <= 0.07f)
        {
            if (sliderHandle1.sprite == handleSpriteNormal || sliderHandle2.sprite == handleSpriteNormal)
                HapticManager.instance.LightHaptic();

            sliderHandle1.sprite = handleSpriteGreen;
            sliderHandle2.sprite = handleSpriteGreen;
        }
        else
        {
            sliderHandle1.sprite = handleSpriteNormal;
            sliderHandle2.sprite = handleSpriteNormal;
        }

    }

    private void Update()
    {

        if (!isActivated || isDone)
            return;

        if (sliderHandle1 != null && sliderHandle2 != null)
            ChangeHandleSprite();

        //if (handModel != null)
        //    handModel.transform.position = Vector3.Lerp(info.handFrom.transform.position, info.handTo.transform.position, slider.value);

        foreach (Resizers resizer in info.resizers)
        {
            if (resizer.direction == BlendShapeDirection.positive)
                resizer.clotheMesh.SetBlendShapeWeight(resizer.meshBlendShapeIndex, ((Mathf.Clamp01((slideValue - resizer.startAtSliderValue) / (resizer.endAtSliderValue - resizer.startAtSliderValue)) * 100)) * resizer.scale);
            else
                resizer.clotheMesh.SetBlendShapeWeight(resizer.meshBlendShapeIndex, (100 - (Mathf.Clamp01((slideValue - resizer.startAtSliderValue) / (resizer.endAtSliderValue - resizer.startAtSliderValue)) * 100)));
        }


        if (Input.touchCount > 0 && (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!info.freeHandControl)
        {
            Debug.Log(sliderAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);

            if (Input.GetMouseButtonDown(0))
            {
                mouseDown = true;
                //sliderAnim.SetFloat("sliderSpeed", info.sliderSpeed);
                localSpeed = 1;
                CutClothes.instance.MainScissorAnimTrigger("cut");
                //SoundManager.instance.StartScissor();
                if (!isUsingGlobalSlider)
                {
                    HapticManager.instance.StartContinousHaptic();
                    sliderAnim.SetTrigger("move");
                }
            }
            else if (Input.GetMouseButtonUp(0) && mouseDown)
            {
                if (!isUsingGlobalSlider)
                    HapticManager.instance.StopContinousHaptic();

                HapticManager.instance.HeavyHaptic();
                //SoundManager.instance.StopScissor();
                //sliderAnim.SetFloat("sliderSpeed", 0);
                localSpeed = 0;
                CutClothes.instance.MainScissorAnimTrigger("idle");

            }

            slideValue = Mathf.Clamp01( slideValue + (Time.deltaTime * (info.sliderSpeed / 1.3f) * localSpeed));
            CutClothes.instance.SetHandGuideMotionTime(slideValue);
            lineRenderer.material.SetFloat("_Progress", slideValue);
            Debug.Log(localSpeed);
            
            if (slideValue >= 0.99f && !isDone)
            {
                slideValue = 0.99f;
                isActivated = false;
                localSpeed = 0;
                lineRenderer.material.SetFloat("_Progress", 1);
                CutClothes.instance.MainScissorAnimTrigger("idle");
                //SoundManager.instance.StopScissor();
                StartCoroutine(DoneRoutine());
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseDown = true;
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0) && mouseDown)
            {
                mouseDown = false;
                isActivated = false;
                StartCoroutine(DoneRoutine());
            }

            if (mouseDown)
            {
                if (info.controlX != ControlDirectionsX.none)
                {
                    slider.value = Mathf.Clamp01(slider.value +
                        (((Input.mousePosition.x - lastMousePosition.x) * Time.fixedDeltaTime * 0.15f * info.sliderSensitivity))
                        * (info.controlX == ControlDirectionsX.right ? 1 : -1));
                }

                if (info.controlY != ControlDirectionsY.none)
                {
                    slider.value = Mathf.Clamp01(slider.value +
                        (((Input.mousePosition.y - lastMousePosition.y) * Time.fixedDeltaTime * 0.15f * info.sliderSensitivity))
                        * (info.controlY == ControlDirectionsY.up ? 1 : -1));
                }
            }

            lastMousePosition = Input.mousePosition;
        }

    }

    private void SetupResizer()
    {
        if (info.handGuide != null)
            handModel = LevelManager.instance.HandModel;

        if (info.sliderParent != null)
        {
            slider = info.sliderParent.GetComponentInChildren<Slider>();
            sliderAnim = info.sliderParent.GetComponent<Animator>();
            sliderParent = info.sliderParent;
            sliderHandle1 = info.sliderParent.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(0).GetComponent<Image>();
            sliderHandle2 = info.sliderParent.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(1).GetComponent<Image>();

            Debug.Log(sliderHandle1);
            Debug.Log(sliderHandle2);

            GameManager.instance.ShowTutorial("scissor");
        }
        else
        {// using global slider / resizer
            slider = LevelManager.instance.GlobalSliderParent.GetComponentInChildren<Slider>();
            sliderAnim = LevelManager.instance.GlobalSliderParent.GetComponent<Animator>();
            sliderParent = LevelManager.instance.GlobalSliderParent.gameObject;
            GameManager.instance.SetGlobalSliderSprite((int)info.sliderSprite);
            isUsingGlobalSlider = true;

            GameManager.instance.ShowTutorial("scissor");
        }

        if (info.freeHandControl)
            sliderAnim.SetTrigger("move");

        slider.value = 0;
        slideValue = 0;
        CutClothes.instance.SetHandGuideMotionTime(0);

        if (info.pin != null)
            info.pin.SetActive(false);

        sliderParent.SetActive(false);
    }

    public void StartResizer(CutClothes triggerer)
    {
        if (triggerer != null)
            triggererScript = triggerer;

        SetupResizer();
        StartCoroutine(StartResizerRoutine());
    }

    [ContextMenu("Force start resizer")]
    public void StartResizer()
    {
        SetupResizer();
        StartCoroutine(StartResizerRoutine());
    }

    private IEnumerator DoneRoutine()
    {
        isDone = true;
        localSpeed = 0;
        HapticManager.instance.StopContinousHaptic();

        CutClothes.instance.MainScissorBackToOrigin();
        StartCoroutine(FlyRoutine());

        //Debug.Log("IS CORRECT : " + CheckIfCorrectSliderValue());
        bool isCorrect = CheckIfCorrectSliderValue();

        //GameManager.instance.TriggerFeedback(isCorrect, currentResult);
        SoundManager.instance.TriggerSound(1);

        LevelManager.instance.headAnim.SetTrigger(isCorrect ? "happy" : "sad");

        if (CheckIfCorrectSliderValue() == false)
        {
            GameManager.instance.numOfWrongAdjustments++;
            //info.emojiParent.GetChild(0).GetComponent<ParticleSystem>().Play();

            //if (info.resizerToDisableHideIfFail != null)
            //    info.resizerToDisableHideIfFail.disableHide = true;
        }
        else
        {
            Debug.Log("Correct");
            //info.emojiParent.GetChild(1).GetComponent<ParticleSystem>().Play();
        }

        sliderParent.gameObject.SetActive(false);

        if (info.pin != null)
            info.pin.SetActive(true);

        yield return new WaitForSeconds(1);

        if (!disableHide)
        {
            foreach (GameObject o in objectsToHideAfter)
                o.SetActive(false);

            foreach (GameObject o in objectsToShowAfter)
                o.SetActive(true);
        }



        triggererScript.CheckIfDone();
    }

    private IEnumerator FlyRoutine()
    {
        float flyValue = 0;
        while (flyValue <= 1f)
        {
            flyRenderer.SetBlendShapeWeight(cutFlyBlendshapeIndex, Mathf.Lerp(cutFlyFromTo.x, cutFlyFromTo.y, flyValue));
            flyValue += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator StartResizerRoutine()
    {
        lineRenderer.transform.parent.gameObject.SetActive(true);
        Utility.instance.SetCinemachineCameraSolo(info.resizerCam);

        //set correct target's position based on correctSlider value
        if (info.correctTargetType == CorrectTargetType.Star)
        {
            RectTransform correctTarget;
            correctTarget = sliderParent.GetComponentInChildren<ClotheResizerCorrectTarget>(false).GetComponent<RectTransform>();
            correctTarget.anchoredPosition = new Vector2(sliderParent.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x * ((info.greenValueMinMax.x + info.greenValueMinMax.y) / 2), 0);
        }
        else
        {
            RectTransform correctTargetGreen = sliderParent.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();
            correctTargetGreen.anchoredPosition = new Vector2(sliderParent.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x * (info.greenValueMinMax.x), 0);
            correctTargetGreen.sizeDelta = new Vector2(sliderParent.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x - (sliderParent.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x * info.greenValueMinMax.y), 0);
        }


        if (handModel != null)
            handModel.GetComponent<Tweener>().UpdateTarget(info.handGuide);

        CutClothes.instance.mainScissor.SetSpeed(5);

        if (scissorAnimator != null)
            CutClothes.instance.mainScissor.UpdateTarget(scissorAnimator);

        yield return new WaitForSeconds(1);


        //sliderParent.SetActive(true);


        sliderAnim.SetBool("mirror", info.sliderDirection == AutomaticSliderDirection.rightToLeft);

        if (info.freeHandControl)
            sliderAnim.enabled = false;

        if (!info.freeHandControl && isUsingGlobalSlider)
        {
            sliderAnim.SetFloat("sliderSpeed", 0);
            yield return new WaitForSeconds(0); // DELAY BEFORE STARTING
            sliderAnim.SetFloat("sliderSpeed", info.sliderSpeed);
        }

        CutClothes.instance.mainScissor.SetSpeed(20);
        isActivated = true;

        //if (handModel != null)
        //    handModel.GetComponent<Tweener>().UpdateTarget(null);
    }

    public bool CheckIfCorrectSliderValue()
    {
        //float result = Mathf.Abs(slider.value - ((info.greenValueMinMax.x + info.greenValueMinMax.y) / 2));

        //if (result <= 0.03f)
        currentResult = "green";
        //else if (result <= 0.12f)
        //    currentResult = "yellow";
        //else
        //    currentResult = "red";

        //return result <= 0.12f;
        return true;
        //return slider.value >= info.greenValueMinMax.x && slider.value <= info.greenValueMinMax.y;
    }
}
