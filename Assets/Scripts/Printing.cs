using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;
using UnityEngine.UI;

public class Printing : Step
{
    [SerializeField] private Texture[] texture;
    [SerializeField] private int correctIndex;
    [SerializeField] private Transform cameraAnglesParent;
    [SerializeField] private CameraFocus camFocus;
    [SerializeField] private P3dPaintSphere painterScript;
    [SerializeField] private Renderer toolRenderer;
    [SerializeField] private GameObject[] objectsToAffect;

    [SerializeField] private Transform toolTarget, toolTargetOutPos;
    [SerializeField] private Tweener toolTweener;
    [SerializeField] private FreeMovementDrag toolTargetScript;
    [SerializeField] private Image tipImage;
    [SerializeField] private Transform groupSelectorParent;

    [SerializeField] private Transform toolOriginParent;
    [SerializeField] private Transform girlTarget, girlCenterTarget;
    [SerializeField] private GameObject confirmationButton;
    [SerializeField] private Animator UIAnimator;
    [SerializeField] private Painter painter;
    [SerializeField] private P3dHitBetween undoPrinter;

    private bool blink;
    private float floatLerper;
    private bool isDone;
    private int currentIndex;

    public override void OnStepStart()
    {
        base.OnStepStart();
        toolTargetScript.SetMinMax((int)camFocus);

        tipImage.sprite = groupSelectorParent.GetChild(0).GetChild(0).GetChild(0).GetChild(correctIndex).GetChild(0).GetComponent<Image>().sprite;

        ChangeCameraAngle();
        ChangeDecal(0);
        foreach (GameObject o in objectsToAffect)
        {
            o.GetComponents<P3dPaintableTexture>()[0].enabled = true;
        }

        if (objectsToAffect.Length == 1)
        {
            painterScript.TargetTexture = objectsToAffect[0].GetComponents<P3dPaintableTexture>()[0];
        }

        isDone = false;
        //StartCoroutine(BlinkRoutine());
        RecenterTool();
        confirmationButton.SetActive(false);
    }

    public void UndoPrint()
    {
        confirmationButton.SetActive(false);
        UIAnimator.SetTrigger("in");
        painter.UIAnimator = UIAnimator;
        undoPrinter.ManuallyHitNow();
    }

    public void RecenterTool()
    {
        Vector3 pos = toolOriginParent.GetChild((int)camFocus).transform.position;
        girlTarget.transform.position = pos;
        girlCenterTarget.transform.position = new Vector3(pos.x, pos.y, -0.626f);
    }

    public void ShowConfirmationButton()
    {
        confirmationButton.SetActive(true);
    }

    private IEnumerator BlinkRoutine()
    {
        blink = true;
        yield return new WaitForSeconds(2);
        blink = false;
    }

    private void Update()
    {
        if (blink)
        {
            floatLerper = Mathf.Lerp(floatLerper, 1, Time.deltaTime * 2);
        }
        else
        {
            floatLerper = Mathf.Lerp(floatLerper, 0, Time.deltaTime * 2);
        }

        //foreach (GameObject o in objectsToAffect)
        //{
        //    o.GetComponent<Renderer>().materials[2].SetFloat("_visibility", floatLerper);
        //}
    }

    public void ChangeCameraAngle()
    {
        StepsManager.instance.GetCurrentStepCamera().transform.position = cameraAnglesParent.GetChild((int)camFocus).position;
        StepsManager.instance.GetCurrentStepCamera().transform.eulerAngles = cameraAnglesParent.GetChild((int)camFocus).eulerAngles;

    }

    public override void OnStepEnd()
    {
        base.OnStepEnd();

        foreach (GameObject o in objectsToAffect)
            o.GetComponents<P3dPaintableTexture>()[0].enabled = false;
    }

    public void ChangeDecal(int index)
    {
        if (painterScript == null)
            return;

        RecenterTool();

        currentIndex = index;
        var blendMode = painterScript.BlendMode;
        blendMode.Texture = texture[index];

        painterScript.BlendMode = blendMode;

        if (toolRenderer != null)
            toolRenderer.material.mainTexture = texture[index];


    }

    public void Confirm()
    {
        SoundManager.instance.TriggerSound(0);
        HapticManager.instance.LightHaptic();

        if (!isDone)
            StartCoroutine(DoneRoutine());
    }

    private IEnumerator DoneRoutine()
    {
        isDone = true;
        toolTarget.GetComponent<FollowRaycast>().Activate(false);

        toolTweener.SetSpeed(1.5f);
        toolTarget.position = toolTargetOutPos.position;
        LevelManager.instance.headAnim.SetTrigger(currentIndex == correctIndex ? "happy" : "sad");
        
        if (currentIndex != correctIndex)
            GameManager.instance.numOfWrongAdjustments += 2;

        yield return new WaitForSeconds(1);
        StepsManager.instance.NextStep();
    }
}
