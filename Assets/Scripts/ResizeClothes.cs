using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeClothes : Step
{
    public static ResizeClothes instance;
    [SerializeField] private Transform clotheResizerParent;
    [SerializeField] private BlendshapeDefaults[] blendShapeDefaults;
    [SerializeField] private Animator handGuideAnimator;

    private void Awake()
    {
        instance = this;
    }

    public override void OnStepStart()
    {
        base.OnStepStart();
        SetupBlendShapeDefaultValues();
        CheckForSkipResizers();
        
        if (LevelManager.instance.showHandModel)
            LevelManager.instance.HandModel.SetActive(true);

        Invoke(nameof(CheckIfDone), 0.5f);
    }

    public void SetHandGuideMotionTime(float newMotionTime)
    {
        handGuideAnimator.SetFloat("motionTime", newMotionTime);
    }

    private void CheckForSkipResizers()
    {
        for(int i = 0; i < clotheResizerParent.childCount; i++)
        {
            if (clotheResizerParent.GetChild(i).GetComponent<ClotheResizer>().skip || !clotheResizerParent.GetChild(i).gameObject.activeInHierarchy)
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
            if (clotheResizerParent.GetChild(i).GetComponent<ClotheResizer>().isDone == false)
            {
                clotheResizerParent.GetChild(i).GetComponent<ClotheResizer>().StartResizer(this);
                return;
            }
        }

        LevelManager.instance.HandModel.SetActive(false);
        Debug.Log("ALL RESIZERS DONE");
        //StepsManager.instance.StepCameraSolo();
        StepsManager.instance.NextStep();
    }
}

[System.Serializable]
public class BlendshapeDefaults
{
    public SkinnedMeshRenderer renderer;
    public int index;
    public float value;

    public void Setup()
    {
        renderer.SetBlendShapeWeight(index, value);
    }
}