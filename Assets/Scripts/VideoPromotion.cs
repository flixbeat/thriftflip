using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Altplay/GameVideoPromotion")]
public class VideoPromotion : ScriptableObject
{
    public string gameName;
    public string IOS_URL, android_URL;
    public VideoClip videoClip;
}