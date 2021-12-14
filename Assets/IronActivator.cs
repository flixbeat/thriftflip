using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronActivator : MonoBehaviour
{
    [SerializeField] private Ironing ironingScript;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ENTER");
        ironingScript.activated = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("EXIT!");
        ironingScript.activated = false;
    }
}
