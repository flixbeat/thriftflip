using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private Image slider;

    private void Start()
    {
        StartLoading();
    }


    private void StartLoading()
    {
        StartCoroutine(LoadingRoutine());
    }

    private IEnumerator LoadingRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            //Output the current progress
            loadingText.text = "Loading progress: " + (asyncOperation.progress * 100) + "%";
            slider.fillAmount = asyncOperation.progress;

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                loadingText.text = "Loading progress: 100%";
                slider.fillAmount = 1;

                //Activate the Scene
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
        
    }

}
