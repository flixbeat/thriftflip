using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Utility : MonoBehaviour
{
    public static Utility instance;

    void Awake()
    {
        // singleton
        instance = this;
    }

    public void ChangeCinemachineCamera(Transform parent, int index)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).TryGetComponent(out CinemachineVirtualCamera cam))
                cam.Priority = index == i ? 11 : 10;
            else
                Debug.Log($"{parent.GetChild(i).name} does not have cinemachine camera component");
        }

    }

    public void SetCinemachineCameraSolo(CinemachineVirtualCamera camera)
    {
        foreach (CinemachineVirtualCamera cam in FindObjectsOfType(typeof(CinemachineVirtualCamera)))
            cam.Priority = cam == camera ? 11 : 10;
    }

    public void DeactivateChildrenExceptIndex(Transform parent, int index)
    {
        for (int i = 0; i < parent.childCount; i++) 
            parent.GetChild(i).gameObject.SetActive(i == index);
    }
}
