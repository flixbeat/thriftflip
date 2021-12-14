using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class Decorations : MonoBehaviour
{
    [SerializeField] private Decoration[] decorationList;
    [SerializeField] private GameObject objectShownEffects;
    [SerializeField] private CinemachineVirtualCamera originCam;
    [SerializeField] private GameObject buyButtons, backButton;


    public int DecorationsCount => decorationList.Length;


    public void UpdateDecorations(int index, bool showSparkle = true)
    {
        for (int i = 0; i < decorationList.Length; i++)
        {
            if (i > index)
                return;


            foreach (GameObject o in decorationList[i].objectsToHide)
                o.SetActive(false);

            foreach (GameObject o in decorationList[i].objectsToShow)
            {

                if (showSparkle && i == index)
                {
                    StopAllCoroutines();
                    StartCoroutine(ShowDecorationRoutine(i));
                    continue;
                }
                else
                {
                    o.SetActive(true);
                    originCam.GetComponent<CameraRotate>().enable = true;

                }

            }

        }

    }

    private IEnumerator ShowDecorationRoutine(int i)
    {
        if (i >= decorationList.Length)
            yield break;


        if (decorationList[i].cam != null)
        {
            originCam.GetComponent<CameraRotate>().enable = false;
            Utility.instance.SetCinemachineCameraSolo(decorationList[i].cam);
            Debug.Log(gameObject.name);

            if (decorationList[i].cam.transform.eulerAngles != originCam.transform.eulerAngles)
            {
                buyButtons.GetComponent<CanvasGroup>().interactable = false;
                backButton.SetActive(false);
                yield return new WaitForSeconds(1);
            }
        }
        else
            originCam.GetComponent<CameraRotate>().Setup();

        foreach (GameObject o in decorationList[i].objectsToShow)
        {
            o.gameObject.SetActive(true);
            GameObject sparkle = Instantiate(objectShownEffects, transform, false);
            sparkle.transform.position = o.transform.position;
            Destroy(sparkle, 2);
        }

        if (decorationList[i].cam != null)
        {
            if (decorationList[i].cam.transform.eulerAngles != originCam.transform.eulerAngles)
                yield return new WaitForSeconds(1);

            originCam.GetComponent<CameraRotate>().Resetval();
            Utility.instance.SetCinemachineCameraSolo(originCam);
        }
        else
            originCam.GetComponent<CameraRotate>().Setup();



        backButton.SetActive(true);
        buyButtons.GetComponent<CanvasGroup>().interactable = true;
    }
}

[System.Serializable]
public class Decoration
{
    public GameObject[] objectsToShow;
    public GameObject[] objectsToHide;
    public CinemachineVirtualCamera cam;
}
