using UnityEngine;

public class Step : MonoBehaviour
{
    [Header("Step Configurations")]
    public UnityEngine.UI.Slider progressSlider;
    public bool skipStep;
    [SerializeField] private float delayBeforeEndingStep;

    public float Delay => delayBeforeEndingStep;

    public virtual void OnStepStart()
    {
        Debug.Log(gameObject.name + " Step start");
    }

    public virtual void OnStepEnd()
    {
        Debug.Log(gameObject.name + " Step end");
    }

    public virtual void OnStepEnd2()
    {

    }

    public void UpdateProgress(float progress) // 0f - 1f
    {
        Debug.Log("Update Progress - " + progress);
        progressSlider.value = progress;

        if (progress >= 1)
            StepsManager.instance.NextStep();
    }
}
