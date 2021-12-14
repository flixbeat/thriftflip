using UnityEngine;
using UnityEngine.UI;

public class StepManager : MonoBehaviour
{
    [SerializeField] private Transform _toggles;
    private int _step;

    public void SetTotalSteps(int total)
    {
        Utils.SetChildrenVisible(_toggles, false, total);
    }

    public void SetStep(int current)
    {
        _step = current;
        var toggles = _toggles.GetComponentsInChildren<Toggle>();
        for (var i = 0; i < toggles.Length; i++) toggles[i].isOn = i < current;
//            Utils.SetChildVisible(toggle.transform.GetChild(1), true);
    }

    public void NextStep()
    {
        SetStep(_step + 1);
    }
}