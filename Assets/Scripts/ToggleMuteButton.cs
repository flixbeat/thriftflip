using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleMuteButton : MonoBehaviour
{
    [SerializeField] private Sprite sprite_NotMuted, sprite_Muted;

    public void Setup()
    {
        UpdateSprite();

        if (PlayerPrefs.HasKey("isMuted"))
        {
            if (PlayerPrefs.GetInt("isMuted") == 1 && !SoundManager.instance.muted || PlayerPrefs.GetInt("isMuted") == 0 && SoundManager.instance.muted)
            {
                Debug.Log("MUTED");
                SoundManager.instance.ToggleMusicMuted();
                UpdateSprite();
            }
        }
            
        GetComponent<Button>().onClick.AddListener(
            delegate 
                { 
                    SoundManager.instance.ToggleMusicMuted(); 
                    UpdateSprite(); 
                }
            );
    }

    public void UpdateSprite()
    {
        GetComponent<Image>().sprite = SoundManager.instance.muted ? sprite_Muted : sprite_NotMuted;
    }
}
