using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepsManager : MonoBehaviour
{
    public static StepsManager instance;
    [Header("Step")]
    [SerializeField] private int currentStepIndex;
    [SerializeField] private Step currentStep;

    [Header("Scene References")]
    [SerializeField] private Transform cameraParent;
    [SerializeField] private Transform stepParent, stepUI, macroStepsParent;

    public Transform NextStepParent => stepParent.GetChild(currentStepIndex + 1).transform;
    public Transform CameraParent => cameraParent;

    private void Awake()
    {
        // singleton
        instance = this;
    }

    public void StartLevel()
    {
        GameManager.instance.SetStepUI(stepUI);
        currentStepIndex = -1;
        DestroySkippedSteps();
        NextStep();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            NextStep();
    }

    public void DestroySkippedSteps()
    {
        for (int i = 0; i < stepParent.childCount; i++)
            if (stepParent.GetChild(i).TryGetComponent(out Step step))
                if (step.skipStep)
                {
                    Destroy(stepParent.GetChild(i).gameObject);
                    Destroy(cameraParent.GetChild(i).gameObject);
                    Destroy(stepUI.GetChild(i).gameObject);

                    if (macroStepsParent != null)
                        Destroy(macroStepsParent.GetChild(i).gameObject);
                }
    }
    
    public void NextStep()
    {
        // if there is an existing step, perform its end function
        if (currentStep != null)
            currentStep.OnStepEnd();

        currentStepIndex++;

        if (macroStepsParent != null)
            for (int i = 0; i < macroStepsParent.childCount; i++)
                macroStepsParent.GetChild(i).GetComponent<Toggle>().interactable = i <= currentStepIndex - 1;

        Utility.instance.DeactivateChildrenExceptIndex(stepParent, currentStepIndex);
        Utility.instance.DeactivateChildrenExceptIndex(stepUI, currentStepIndex);
        //Utility.instance.ChangeCinemachineCamera(cameraParent, currentStepIndex);
        StepCameraSolo();

        // perform step's start function
        currentStep = stepParent.GetChild(currentStepIndex).GetComponent<Step>();
        currentStep.progressSlider = stepUI.GetChild(currentStepIndex).GetComponentInChildren<UnityEngine.UI.Slider>();
        currentStep.OnStepStart();
    }

    public void StepCameraSolo()
    {
        Utility.instance.SetCinemachineCameraSolo(cameraParent.GetChild(currentStepIndex).GetComponent<Cinemachine.CinemachineVirtualCamera>());
    }

    public Transform GetCurrentStepCamera()
    {
        return cameraParent.GetChild(currentStepIndex);
    }
}
