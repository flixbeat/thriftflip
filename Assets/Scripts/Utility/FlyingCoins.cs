using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCoins : MonoBehaviour
{

    [SerializeField] private RectTransform target;
    [SerializeField] private float tweenSpeed = 10;
    [SerializeField] private Outro outroScript;

    private RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (target != null)
        {
            rect.position = Vector3.Lerp(rect.position, target.position /*+ (Vector3.right * rect.sizeDelta.x)*/, Time.deltaTime * tweenSpeed);
            if (Vector3.Distance(rect.position, target.position) <= 53f)
            {
                OnGemSpawnDone();
                target = null;
            }
        }
    }

    private void OnGemSpawnDone()
    {
        outroScript.AddCoin();
        HapticManager.instance.LightHaptic();
        SoundManager.instance.TriggerSound(4);
        Destroy(gameObject);
    }

    public void SetTarget(RectTransform newTarget, Outro outroScript)
    {
        target = newTarget;
        this.outroScript = outroScript;
    }
}
