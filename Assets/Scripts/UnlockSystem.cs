using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockSystem : MonoBehaviour
{
    public static UnlockSystem instance;
    [SerializeField] private float currentLevelIndex;
    

    [Header("Current data")]
    public Vector2 fromTo;
    public Sprite shadowSprite, fillSprite;
    public bool allIsUnlocked;

    [Space(50)]
    [SerializeField] private UnlockInfo[] unlockables;


    [SerializeField] public float currentUnlockableIndex;

    private void Awake()
    {
        instance = this;
    }
    
    public void SetLevel(int index)
    {
        currentLevelIndex = index;
        GetCurrentUnlockable();
    }

    public UnlockInfo GetCurrentUnlockableInfo()
    {
        return unlockables[(int) currentUnlockableIndex];
    }

    [ContextMenu("GetCurrentLockable")]
    public void GetCurrentUnlockable()
    {
        for (int i = 0; i < unlockables.Length; i++)
        {
            if (unlockables[i].levelIndexToUnlock >= currentLevelIndex)
            {
                currentUnlockableIndex = i;
                HandleCurrentUnlockable(i);
                return;
            }
        }

        Debug.Log("All items already unlocked");
        allIsUnlocked = true;
    }

    public void HandleCurrentUnlockable(int index)
    {
        float dividend;
        float fromPercentage;
        float toPercentage;

        if (index == 0)
        {
            fromPercentage = 0;
            toPercentage = 1;
            dividend = 0;
        }
        else
        {
            dividend = (unlockables[index].levelIndexToUnlock) - (unlockables[Mathf.Max(0, index - 1)].levelIndexToUnlock);
            fromPercentage = ((currentLevelIndex - 1) - unlockables[index - 1].levelIndexToUnlock) / dividend;
            toPercentage = (currentLevelIndex - unlockables[index - 1].levelIndexToUnlock) / dividend;
        }

        Debug.Log($"Index {index}");
        Debug.Log($"{fromPercentage}% -> {toPercentage}%");

        fromTo = new Vector2(fromPercentage * 100, toPercentage * 100);
        shadowSprite = unlockables[index].unlockShadowSprite;
        fillSprite = unlockables[index].unlockFillSprite;
    }

    public int CountUnlockType(UnlockType type)
    {
        int count = 0;
        for (int i = 0; i < currentUnlockableIndex; i++)
        {
            if (unlockables[i].unlockType == type)
            {
                count++;
            }
        }

        return count;
    }

}

[System.Serializable]
public class UnlockInfo
{
    public int levelIndexToUnlock;
    public Sprite unlockFillSprite;
    public Sprite unlockShadowSprite;
    public UnlockType unlockType;
}

public enum UnlockType
{
    resizeTool,
    painterTool,
    printerTool,
    steamerTool,
    painterColor,
    printerTexture
}
