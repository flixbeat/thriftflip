using PaintIn3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ironing : Step
{
    [SerializeField] private float progressSpeed = 1;
    [SerializeField] private GameObject[] objectsToAffect;
    [SerializeField] private Transform cameraAnglesParent;
    [SerializeField] private CameraFocus camFocus;
    [SerializeField] private P3dPaintDecal painterScript;
    [SerializeField] private Texture doneTexture;
    [SerializeField] private ParticleSystem starFX;
    [SerializeField] private Transform toolTarget, toolTargetOutPos;
    [SerializeField] private Tweener toolTweener;
    [SerializeField] private FreeMovementDrag toolTargetScript;

    [SerializeField] private Transform toolOriginParent;
    [SerializeField] private Transform girlTarget, girlCenterTarget;

    private bool blink;
    private float floatLerper;
    private Vector3 lastMousePos;
    public bool activated;

    public override void OnStepStart()
    {
        base.OnStepStart();

        toolTargetScript.SetMinMax((int)camFocus);

        ChangeCameraAngle();
        foreach (GameObject o in objectsToAffect)
        {
            o.GetComponents<P3dPaintableTexture>()[1].enabled = true;
        }

        if (objectsToAffect.Length == 1)
        {
            painterScript.TargetTexture = objectsToAffect[0].GetComponents<P3dPaintableTexture>()[1];
        }

        progressSpeed /= objectsToAffect.Length;
        //StartCoroutine(BlinkRoutine());

        RecenterTool();
    }
    
    public void RecenterTool()
    {
        Vector3 pos = toolOriginParent.GetChild((int)camFocus).transform.position;
        girlTarget.transform.position = pos;
        girlCenterTarget.transform.position = new Vector3(pos.x, pos.y , -0.626f); 
    }

    private IEnumerator BlinkRoutine()
    {
        blink = true;
        yield return new WaitForSeconds(2);
        blink = false;
    }

    public void ChangeCameraAngle()
    {
        StepsManager.instance.GetCurrentStepCamera().transform.position = cameraAnglesParent.GetChild((int)camFocus).position;
        StepsManager.instance.GetCurrentStepCamera().transform.eulerAngles = cameraAnglesParent.GetChild((int)camFocus).eulerAngles;
    }

    public override void OnStepEnd()
    {

        foreach (GameObject o in objectsToAffect)
            o.GetComponents<P3dPaintableTexture>()[1].enabled = false;
        

        base.OnStepEnd();

    }

    private bool isDone;
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


        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && !isDone && lastMousePos != Input.mousePosition && activated)
        {
            progressSlider.value += Time.deltaTime * 0.5f * progressSpeed;

            if (progressSlider.value >= 0.99f && !isDone)
            {

                StartCoroutine(DoneRoutine());
            }
        }

        lastMousePos = Input.mousePosition;
    }

    private IEnumerator DoneRoutine()
    {
        SoundManager.instance.TriggerSound(0);

        isDone = true;

        starFX.Play();
        foreach (GameObject o in objectsToAffect)
            o.GetComponent<Renderer>().material.SetTexture("_BumpMap", doneTexture);

        toolTarget.GetComponent<FollowRaycast>().Activate(false);

        toolTweener.SetSpeed(1.5f);
        toolTarget.position = toolTargetOutPos.position;
        LevelManager.instance.headAnim.SetTrigger("happy");
        yield return new WaitForSeconds(2);
        StepsManager.instance.NextStep();
    }
}
