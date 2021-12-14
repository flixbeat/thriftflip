using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;
public class PaintDress : Step
{
    [SerializeField] private P3dPaintSphere painter;
    [SerializeField] private P3dHitBetween hitBetween;
    [SerializeField] private Texture2D[] textures;
    [SerializeField] private GameObject UISelector;
    [SerializeField] private int correctIndex;

    private int currentIndex;
    private bool isDone;
    public void ChangePaint(int index)
    {
        painter.TileTexture = textures[index];
        currentIndex = index;
    }
    
    public void Update()
    {
        if (!isDone)
            hitBetween.enabled = Input.GetMouseButton(0);
    }

    public void Confirm()
    {
        if (currentIndex != correctIndex)
        {
            GameManager.instance.numOfWrongAdjustments++;
            GameManager.instance.TriggerFeedback(false);
        }

        Debug.Log($"Is correct : {currentIndex != correctIndex}");
        UISelector.SetActive(false);
        painter.Radius = 100;
        isDone = true;
        hitBetween.enabled = true;

        Invoke(nameof(NextStep), 1);
    }

    public void NextStep()
    {
        StepsManager.instance.NextStep(); ;
    }
}
