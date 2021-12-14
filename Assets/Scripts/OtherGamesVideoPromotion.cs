using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class OtherGamesVideoPromotion : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI gameNameText;
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Promotion data")]
    [SerializeField] private VideoPromotion[] promotions;
    [SerializeField] private VideoPromotion currentPromotion;
    [SerializeField] private int randomIndex;
    void Start()
    {
        StartCoroutine(ShowPromotion());
        //videoPlayer.loopPointReached += delegate { StartCoroutine(ShowPromotion()); } ;
    }

    private IEnumerator ShowPromotion(VideoPlayer vp = null)
    {
        randomIndex = Random.Range(0, promotions.Length);
        currentPromotion = promotions[randomIndex];

        gameNameText.text = currentPromotion.gameName;
        videoPlayer.clip = currentPromotion.videoClip;

        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return new WaitForEndOfFrame();

        videoPlayer.Play();
    }

    public void OpenPromotionURL()
    {
        if (currentPromotion == null)
            return;

#if UNITY_ANDROID
        Application.OpenURL(currentPromotion.android_URL);
        return;
#endif

#if UNITY_IOS
        Application.OpenURL(currentPromotion.IOS_URL);
        return;
#endif

        // if other platforms...
        Application.OpenURL(currentPromotion.android_URL);
    }

}
