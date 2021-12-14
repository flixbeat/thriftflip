using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectClotheDecoration : Step
{
    [SerializeField] private GameObject[] decorations;
    [SerializeField] private GameObject selectDecorationUI;
    [SerializeField] private int correctIndex;

    private int currentIndex;

    public override void OnStepStart()
    {
        base.OnStepStart();
        foreach (GameObject o in decorations)
            o.SetActive(false);
    }

    public void SelectDecoration(int index)
    {
        foreach (GameObject o in decorations)
            o.SetActive(false);

        decorations[index].SetActive(true);
        currentIndex = index;

    }

    public void Confirm()
    {

        if (correctIndex != currentIndex)
        {
            GameManager.instance.numOfWrongAdjustments++;
            GameManager.instance.TriggerFeedback(false);
        }

        //selectDecorationUI.SetActive(false);
        StepsManager.instance.NextStep();
    }
}
