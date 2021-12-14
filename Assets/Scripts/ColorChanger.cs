using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] private Renderer rendererToChangeColor;
    [SerializeField] private ShaderType shaderType;
    [SerializeField] private int materialIndex;
    [SerializeField] private Color correctColor;
    [SerializeField] private ParticleSystem colorChangeFX;
    [SerializeField] private GameObject arrow;

    private ClothesColorChanger triggererScript;
    private bool isDone;
    private string shaderParameter;

    public bool IsDone => isDone;

    private void Start()
    {
        arrow.SetActive(false);
    }

    public void SetupClotheChanger(ClothesColorChanger triggerer)
    {
        triggererScript = triggerer;
        //GameManager.instance.SetCurrentClotheColorChanger(this);
        shaderParameter = shaderType == ShaderType.URP ? "_BaseColor" : "_Color";
        arrow.SetActive(true);
    }

    public void ExitClotheChanger()
    {
        //GameManager.instance.SetCurrentClotheColorChanger(null);
        arrow.SetActive(false);
        isDone = true;
        triggererScript.CheckIfDone();
    }

    public void ChangeColor(Color newColor)
    {
        rendererToChangeColor.materials[materialIndex].SetColor(shaderParameter, newColor);
        colorChangeFX.Play();
        Debug.Log($"Is correct : {newColor == correctColor}");

        if (newColor == correctColor) // correct
            ExitClotheChanger();
    }
}

public enum ShaderType
{
    URP,
    ToonColors
}
