using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothesColorChanger : Step
{
    [SerializeField] private Transform colorChangerParent;
    public override void OnStepStart()
    {
        base.OnStepStart();
        //GameManager.instance.GlobalColorChanger.SetActive(true);
        CheckIfDone();
    }

    public void CheckIfDone()
    {
        for (int i = 0; i < colorChangerParent.childCount; i++)
        {
            if (colorChangerParent.GetChild(i).GetComponent<ColorChanger>().IsDone == false)
            {
                colorChangerParent.GetChild(i).GetComponent<ColorChanger>().SetupClotheChanger(this);
                return;
            }
        }

        Done();
    }

    private void Done()
    {
        //GameManager.instance.GlobalColorChanger.SetActive(false);
        Debug.Log("Color Done!");
        StepsManager.instance.NextStep();
    }
}
