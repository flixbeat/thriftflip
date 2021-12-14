using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level20Ads : MonoBehaviour
{
    [Header("Configurations")]
    [SerializeField] private bool activateAds;

    [Header("Scene references")]
    [SerializeField] private Outro outroScript;
    [SerializeField] private GameObject textAds;
    // Start is called before the first frame update
    void Start()
    {
        if (activateAds)
        {
            textAds.SetActive(true);
            outroScript.useSquidGameLose = true;
        }
        else
        {
            textAds.SetActive(false);
            outroScript.useSquidGameLose = false;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
