using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;
using UnityEngine.UI;

public class Coloring : Step
{
    [SerializeField] private Color[] paintColors;
    [SerializeField] private int correctIndex;
    [SerializeField] private CameraFocus camFocus;
    [SerializeField] private Transform cameraAnglesParent;
    [SerializeField] private P3dPaintSphere painterScript;
    [SerializeField] private Renderer toolRenderer, particleRenderer;
    [SerializeField] private GameObject[] objectsToAffect;

    [SerializeField] private Transform toolTarget, toolTargetOutPos;
    [SerializeField] private Tweener toolTweener;
    [SerializeField] private FreeMovementDrag toolTargetScript;
    [SerializeField] private Image tipImage;
    [SerializeField] private Transform groupSelectorParent;

    [SerializeField] private Transform toolOriginParent;
    [SerializeField] private Transform girlTarget, girlCenterTarget;

    private bool blink;
    private float floatLerper;
    private bool isWin;
    private bool isDone;
    private int currentIndex;
    public override void OnStepStart()
    {
        base.OnStepStart();
        toolTargetScript.SetMinMax((int)camFocus);
        isDone = false;
        ChangeCameraAngle();
        ChangePaintColor(0);
        tipImage.sprite = groupSelectorParent.GetChild(0).GetChild(0).GetChild(0).GetChild(correctIndex).GetChild(0).GetComponent<Image>().sprite;
        
        foreach (GameObject o in objectsToAffect)
        {
            o.GetComponents<P3dPaintableTexture>()[2].enabled = true;

        }

        if (objectsToAffect.Length == 1)
        {
            painterScript.TargetTexture = objectsToAffect[0].GetComponents<P3dPaintableTexture>()[2];
        }
        //StartCoroutine(BlinkRoutine());

        RecenterTool();
    }

    public void RecenterTool()
    {
        Vector3 pos = toolOriginParent.GetChild((int)camFocus).transform.position;
        girlTarget.transform.position = pos;
        girlCenterTarget.transform.position = new Vector3(pos.x, pos.y, -0.626f);
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
            o.GetComponents<P3dPaintableTexture>()[2].enabled = false;
    }

    public void ChangePaintColor(int index)
    {
        Debug.Log("Change color " + index);
        RecenterTool();
        currentIndex = index;
        if (painterScript)
        {
            painterScript.Color = paintColors[index];
        }

        if (toolRenderer != null)
            toolRenderer.material.color = paintColors[index];

        if (particleRenderer != null)
            particleRenderer.material.color = paintColors[index];
    }

    public void CheckIfCorrect()
    {
        if (isWin)
            return;

        isWin = currentIndex == correctIndex;
    }

    public void Confirm()
    {
        if (!isDone)
            StartCoroutine(DoneRoutine());
    }

    private IEnumerator DoneRoutine()
    {
        SoundManager.instance.TriggerSound(0);
        HapticManager.instance.LightHaptic();

        isDone = true;
        toolTarget.GetComponent<FollowRaycast>().Activate(false);
        LevelManager.instance.headAnim.SetTrigger(isWin? "happy" : "sad");

        if (!isWin)
            GameManager.instance.numOfWrongAdjustments += 2;

        toolTweener.SetSpeed(1.5f);
        toolTarget.position = toolTargetOutPos.position;
        yield return new WaitForSeconds(1);
        StepsManager.instance.NextStep();
    }

}


public enum CameraFocus
{
    wholeBody,
    tops,
    bottoms,
    forSleeves,
    forSkirts,
    forDressAOC
}
