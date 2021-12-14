using UnityEngine;

public class LevelStep : MonoBehaviour
{
    public static int Level { get; set; }

    private void Start()
    {
        var step = transform.GetChild(Level);
        if(step.childCount == 0) CameraController.Instance.NextStep();
        else Utils.SetChildVisible(step, true);
    }
}