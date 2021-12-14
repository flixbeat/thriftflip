using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using UnityEditor;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [Header("Scene references")]
    [SerializeField] private Transform decorationsParent;
    [SerializeField] private Transform wallParent, floorParent;
    [SerializeField] private Transform upgradeEnvironment, mainGameEnvironment;
    [SerializeField] private Renderer floorRenderer, wallRenderer;
    [SerializeField] private GameObject gameMainObject;
    [SerializeField] private CinemachineVirtualCamera menuOriginalCam, menuUpgradeCam;
    [SerializeField] private Decorations decorationScript, wallDecorationScript, floorDecorationScript;

    [Header("UI")]
    [SerializeField] private GameObject persistentPanel;
    [SerializeField] private GameObject upgradeUIPanel;
    [SerializeField] private GameObject fader;
    [SerializeField] private TextMeshProUGUI balancetext, floorLevelHeader, wallLevelHeader, decorHeaderLevel;
    [SerializeField] private GameObject floorPriceObject, wallPriceObject, decorPriceObject; // getchild(0) for price text
    [SerializeField] private CanvasGroup floorBuyButton, wallBuyButton, decorBuyButton;
    [SerializeField] private GameObject notif;

    [Header("Current data")]
    [SerializeField] private int balance;
    [SerializeField] private int floorLevel, wallLevel, decorLevel;
    [SerializeField] private int currentFloorPrice, currentWallPrice, currentDecorPrice;

    private void Awake()
    {
        instance = this;
        Setup();
    }

    [ContextMenu("Add many cash")]
    public void AddManyCash()
    {
        Debug.Log("ADDED 99999 CASH");
        PlayerPrefs.SetInt("balance", 99999);
    }

    private void Setup()
    {
        if (!PlayerPrefs.HasKey("floorLevel"))
            SetupPlayerPrefs();

        //upgradeEnvironment.transform.position = mainGameEnvironment.transform.position;
        //upgradeEnvironment.transform.eulerAngles = mainGameEnvironment.transform.eulerAngles;
        //upgradeEnvironment.transform.localEulerAngles = mainGameEnvironment.transform.localEulerAngles;

        SyncLocalVariables();
        UpdateDecorations(false);
        UpdateUI();
        ToggleEnvironment(false);
    }

    private void UpdateUI()
    {
        balancetext.text = balance.ToString();

        floorLevelHeader.text = $"Level {floorLevel}";
        wallLevelHeader.text = $"Level {wallLevel}";
        decorHeaderLevel.text = $"Level {decorLevel}";

        
        // check if max already
        if (floorLevel >= floorParent.GetComponent<TextureList>().textures.Length - 1)
        {
            floorPriceObject.SetActive(false);
            floorBuyButton.blocksRaycasts = false;
            floorBuyButton.alpha = 0.5f;
            floorLevelHeader.text = $"MAX";
        }

        if (wallLevel >= wallParent.GetComponent<TextureList>().textures.Length - 1)
        {  
            wallPriceObject.SetActive(false);
            wallBuyButton.blocksRaycasts = false;
            wallBuyButton.alpha = 0.5f;
            wallLevelHeader.text = $"MAX";
        }

        if (decorLevel >= decorationScript.DecorationsCount - 1)
        {
            decorPriceObject.SetActive(false);
            decorBuyButton.blocksRaycasts = false;
            decorBuyButton.alpha = 0.5f;
            decorHeaderLevel.text = $"MAX";
        }

        // set price
        if (floorLevelHeader.text.CompareTo("MAX") != 0)
        {
            currentFloorPrice = floorParent.GetComponent<UpgradePrices>().prices[floorLevel + 1];
            floorPriceObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentFloorPrice.ToString();

            if (currentFloorPrice <= balance)
                notif.SetActive(true);
        }

        if (wallLevelHeader.text.CompareTo("MAX") != 0)
        {
            currentWallPrice = wallParent.GetComponent<UpgradePrices>().prices[wallLevel + 1];
            wallPriceObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentWallPrice.ToString();

            if (currentFloorPrice <= balance)
                notif.SetActive(true);
        }

        if (decorHeaderLevel.text.CompareTo("MAX") != 0)
        {
            currentDecorPrice = decorationsParent.GetComponent<UpgradePrices>().prices[decorLevel + 1];
            decorPriceObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentDecorPrice.ToString();

            if (currentFloorPrice <= balance)
                notif.SetActive(true);
        }

        // check if can buy
        decorBuyButton.interactable = balance >= currentDecorPrice;
        wallBuyButton.interactable = balance >= currentWallPrice;
        floorBuyButton.interactable = balance >= currentFloorPrice;
    }

    public void Buy(string item)
    {
        SoundManager.instance.TriggerSound(0);
        HapticManager.instance.ButtonHaptic();

        if (item.CompareTo("decor") == 0 && balance >= currentDecorPrice)
        {
            balance -= currentDecorPrice;
            decorLevel++;
        }
        else if (item.CompareTo("wall") == 0 && balance >= currentWallPrice)
        {
            balance -= currentWallPrice;
            wallLevel++;
        }
        else if (item.CompareTo("floor") == 0 && balance >= currentFloorPrice)
        {
            balance -= currentFloorPrice;
            floorLevel++;
        }
        else
            return;

        SaveLocalVariables();
        UpdateDecorations(true, item);
        UpdateUI();

    }
    private void UpdateDecorations(bool showSparkle = true, string item = "all")
    {
        floorRenderer.material = floorParent.GetComponent<TextureList>().textures[floorLevel];
        wallRenderer.material = wallParent.GetComponent<TextureList>().textures[wallLevel];


        if (decorHeaderLevel.text.CompareTo("MAX") != 0 && item.CompareTo("all") == 0 || item.CompareTo("decor") == 0)
            decorationScript.UpdateDecorations(decorLevel, showSparkle);

        if (wallLevelHeader.text.CompareTo("MAX") != 0 && item.CompareTo("all") == 0 || item.CompareTo("wall") == 0)
            wallDecorationScript.UpdateDecorations(wallLevel, showSparkle);

        if (floorLevelHeader.text.CompareTo("MAX") != 0 && item.CompareTo("all") == 0 || item.CompareTo("floor") == 0)
            floorDecorationScript.UpdateDecorations(floorLevel, showSparkle);

        //Utility.instance.DeactivateChildrenExceptIndex(decorationsParent, decorLevel);
    }

    private void SetupPlayerPrefs()
    {
        PlayerPrefs.SetInt("floorLevel", 0);
        PlayerPrefs.SetInt("wallLevel", 0);
        PlayerPrefs.SetInt("decorLevel", 0);
        PlayerPrefs.SetInt("balance", 0);
    }

    [ContextMenu("Reset upgrade level")]
    public void ResetUpgradeLevel()
    {
        Debug.Log("UPGRADED LEVELS RESET TO 0");
        PlayerPrefs.SetInt("floorLevel", 0);
        PlayerPrefs.SetInt("wallLevel", 0);
        PlayerPrefs.SetInt("decorLevel", 0);
    }

    private void SyncLocalVariables()
    {
        floorLevel = PlayerPrefs.GetInt("floorLevel");
        wallLevel = PlayerPrefs.GetInt("wallLevel");
        decorLevel = PlayerPrefs.GetInt("decorLevel");
        balance = PlayerPrefs.GetInt("balance");
    }

    private void SaveLocalVariables()
    {
        PlayerPrefs.SetInt("floorLevel", floorLevel);
        PlayerPrefs.SetInt("wallLevel", wallLevel);
        PlayerPrefs.SetInt("decorLevel", decorLevel);
        PlayerPrefs.SetInt("balance", balance);
    }

    public void ToUpgradePanel()
    {
        StartCoroutine(ToUpgradePanelRoutine());
    }

    private IEnumerator ToUpgradePanelRoutine()
    {
        gameMainObject.SetActive(false);
        persistentPanel.SetActive(false);
        upgradeUIPanel.SetActive(true);
        fader.SetActive(false);
        notif.SetActive(false);
        yield return new WaitForEndOfFrame();
        fader.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        Utility.instance.SetCinemachineCameraSolo(menuUpgradeCam);
    }

    public void ToMainMenupanel()
    {
        StartCoroutine(ToMainMenuPanelRoutine());
    }

    private IEnumerator ToMainMenuPanelRoutine()
    {
        gameMainObject.SetActive(true);
        persistentPanel.SetActive(true);
        upgradeUIPanel.SetActive(false);
        notif.SetActive(false);
        fader.SetActive(false);
        yield return new WaitForEndOfFrame();
        fader.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        Utility.instance.SetCinemachineCameraSolo(menuOriginalCam);
    }

    public void ToggleEnvironment(bool mainEnvironment)
    {
        mainGameEnvironment.gameObject.SetActive(false);
        upgradeEnvironment.gameObject.SetActive(false);

        if (mainEnvironment)
            mainGameEnvironment.gameObject.SetActive(true);
        else
            upgradeEnvironment.gameObject.SetActive(true);
    }
}
