using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleHapticButton : MonoBehaviour
{
    [SerializeField] private Sprite sprite_NotMuted, sprite_Muted;

    public void Setup()
    {
        UpdateSprite();

        if (PlayerPrefs.HasKey("hapticBlocked"))
        {
            if (PlayerPrefs.GetInt("hapticBlocked") == 1)
            {
                HapticManager.instance.ToggleHaptic();
                UpdateSprite();
            }
        }

        GetComponent<Button>().onClick.AddListener(
            delegate
            {
                HapticManager.instance.ToggleHaptic();
                UpdateSprite();
            }
            );
    }

    public void UpdateSprite()
    {
        GetComponent<Image>().sprite = HapticManager.instance.isBlocked ? sprite_Muted : sprite_NotMuted;
    }
}
