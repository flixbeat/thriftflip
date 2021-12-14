using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class HapticManager : MonoBehaviour
{
    public static HapticManager instance;

    [SerializeField] private HapticTypes buttonHaptic;

    [Header("ContinousHaptic")]
    [SerializeField] private HapticTypes continousHapticType;
    [Range(0, 1)]
    [SerializeField] private float continousHapticInterval;
    //[SerializeField] private float continousHapticDuration;

    private Coroutine continousHapticRoutine;
    public bool isBlocked;

    private void Awake()
    {
        instance = this;
    }

    public void ToggleHaptic()
    {
        isBlocked = !isBlocked;
        Debug.Log("HAPTIC BLOCKED : " + isBlocked);
        PlayerPrefs.SetInt("hapticBlocked", isBlocked ? 1 : 0);
    }

    public void ButtonHaptic()
    {
        Debug.Log("Button haptic");
        if (isBlocked) 
            return;

        MMVibrationManager.Haptic(buttonHaptic);
    }

    public void LightHaptic()
    {
        Debug.Log("Light Haptic");
        if (isBlocked)
            return;

        MMVibrationManager.Haptic(HapticTypes.LightImpact);
    }
    public void HeavyHaptic()
    {
        Debug.Log("Heavy haptic");
        if (isBlocked)
            return;

        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
    }

    public void StartContinousHaptic()
    {
        Debug.Log("Start continous Haptic");
        if (isBlocked)
            return;

        continousHapticRoutine = StartCoroutine(ContinousHaptic());
    }

    private IEnumerator ContinousHaptic()
    {
        while (true)
        {
            MMVibrationManager.Haptic(continousHapticType);
            yield return new WaitForSeconds(continousHapticInterval);
        }
    }

    public void StopContinousHaptic()
    {
        Debug.Log("Stop continous Haptic");
        if (isBlocked)
            return;

        StopAllCoroutines();
    }

    //private IEnumerator ContinousHapticStopper()
    //{

    //    yield return new WaitForSeconds(continousHapticDuration);

    //    if (continousHapticRoutine != null)
    //        StopCoroutine(continousHapticRoutine);
    //}
}
