using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Configurations")]
    public bool skipIntro;
    public bool VIPLevel;
    public bool showHandModel;
    public bool showUpgradePanel;
    public int coinsAfterWinning = 100;
    [SerializeField] private Color[] clotheColorChoices;
    [SerializeField] private GameObject characterObject;
    [SerializeField] private StepsManager stepManagerScript;
    [SerializeField] private CharacterBuilder characterBuildingScript;
    public Animator headAnim, characterAnim;

    private GameObject handModel;
    private Transform globalSliderParent;

    public Sprite targetPicture;
    public GameObject HandModel => handModel;
    public GameObject CharacterObject => characterObject;
    public Transform GlobalSliderParent => globalSliderParent;

    private void Awake()
    {
        instance = this;

        if (globalSliderParent == null)
            globalSliderParent = GameManager.instance.GlobalSlider;

        if (handModel == null)
            handModel = GameManager.instance.HandModel;
    }

    public void StartLevel()
    {
        characterBuildingScript.BuildCharacter();
        stepManagerScript.StartLevel();

        if (!showHandModel)
            handModel.SetActive(false);
    }

    public void Restart()
    {
        GameManager.instance.RestartLevel();
    }
}

public enum Unlockable
{
    painter,
    texturePainter,
    iron,
    XL_Tag,
}
