using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public bool muted;
    [SerializeField] private GameObject[] sounds;
    [SerializeField] private GameObject[] girlhappySounds;
    [SerializeField] private GameObject[] girlSadSounds;

    [SerializeField] private GameObject steamerSFX, painterSFX, printerSFX;
    private GameObject steamer, painter, printer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void TriggerSound(int index)
    {
        GameObject sfx = Instantiate(sounds[index], transform, false);
        Destroy(sfx, 3);
    }

    public void GirlHappySound()
    {
        GameObject sfx = Instantiate(girlhappySounds[Random.Range(0, girlhappySounds.Length)], transform, false);
        Destroy(sfx, 3);
    }

    public void GirlSadSound()
    {
        GameObject sfx = Instantiate(girlSadSounds[Random.Range(0, girlSadSounds.Length)], transform, false);
        Destroy(sfx, 3);
    }

    public void StartSteamer()
    {
        steamer = Instantiate(steamerSFX);
    }

    public void StopSteamer()
    {
        if (steamer != null) Destroy(steamer);
    }
    public void StartPainter()
    {
        painter = Instantiate(painterSFX);
    }

    public void StopPainter()
    {
        if (painter != null) Destroy(painter);
    }

    public void StartPrinter()
    {
        printer = Instantiate(printerSFX);
    }

    public void StopPrinter()
    {
        if (printer != null) Destroy(printer);
    }

    public void ToggleMusicMuted()
    {
        muted = !muted;
        Debug.Log("IS MUTED : " + muted);
        PlayerPrefs.SetInt("isMuted", muted ? 1 : 0);
        AudioListener.volume = muted ? 0 : 1;
        //Camera.main.GetComponent<AudioListener>().enabled = !muted;
    }
}
