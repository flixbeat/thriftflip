using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutClothes : Step
{
    public static CutClothes instance;
    public Tweener mainScissor;
    public Animator mainScissorAnimator;
    public Transform scissorModelOrigin;
    [SerializeField] private Transform clotheResizerParent;
    [SerializeField] private BlendshapeDefaults[] blendShapeDefaults;
    [SerializeField] private Animator ScissorAnimator;

    private void Awake()
    {
        instance = this;
    }

    public override void OnStepStart()
    {
        base.OnStepStart();
        SetupBlendShapeDefaultValues();
        CheckForSkipResizers();
        LevelManager.instance.HandModel.SetActive(true); // THIS CAN BE THE SCISSOR
        Invoke(nameof(CheckIfDone), 0.1f);
    }

    public void SetHandGuideMotionTime(float newMotionTime)
    {
        ScissorAnimator.SetFloat("motionTime", newMotionTime);
    }

    public void MainScissorAnimTrigger(string triggerName)
    {
        mainScissorAnimator.SetTrigger(triggerName);
    }

    public void MainScissorBackToOrigin()
    {
        mainScissor.UpdateTarget(scissorModelOrigin);
    }

    private void CheckForSkipResizers()
    {
        for (int i = 0; i < clotheResizerParent.childCount; i++)
        {
            if (clotheResizerParent.GetChild(i).GetComponent<ClotheCutter>().skip || !clotheResizerParent.GetChild(i).gameObject.activeInHierarchy)
                Destroy(clotheResizerParent.GetChild(i).gameObject);
        }
    }

    private void SetupBlendShapeDefaultValues()
    {
        foreach (BlendshapeDefaults b in blendShapeDefaults)
            b.Setup();
    }

    public void CheckIfDone()
    {
        for (int i = 0; i < clotheResizerParent.childCount; i++)
        {
            if (clotheResizerParent.GetChild(i).GetComponent<ClotheCutter>().isDone == false)
            {
                clotheResizerParent.GetChild(i).GetComponent<ClotheCutter>().StartResizer(this);
                return;
            }
        }

        LevelManager.instance.HandModel.SetActive(false);
        Debug.Log("ALL RESIZERS DONE");
        //StepsManager.instance.StepCameraSolo();
        StepsManager.instance.NextStep();
    }
}
