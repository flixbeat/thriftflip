using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeOutfit : MonoBehaviour
{
    [SerializeField]
    private GameObject player, hanger;
    private Transform[] childs;

    public static ChangeOutfit instance;

    private void Awake()
    {
        instance = this;
    }

    public void initialClothes()
    {
        Debug.Log("Testtt");
        childs = new Transform[player.transform.childCount];
        for (int i = 0; i < player.transform.childCount; i++)
        {
            childs[i] = player.transform.GetChild(i).transform;
        }

        foreach (Transform children in childs)
        {
            if (children.tag.Equals("DefaultCloth"))
            {
                children.gameObject.SetActive(true);
            }
            if (children.tag.Equals("ActualCloth"))
            {
                children.gameObject.SetActive(false);
            }
        }

    }
    public void changeClothes()
    {
        childs = new Transform[player.transform.childCount];
        for (int i = 0; i < player.transform.childCount; i++)
        {
            childs[i] = player.transform.GetChild(i).transform;
        }

        foreach (Transform children in childs)
        {
            if (children.tag.Equals("DefaultCloth"))
            {
                children.gameObject.SetActive(false);
            }
            if (children.tag.Equals("ActualCloth"))
            {
                children.gameObject.SetActive(true);
            }
        }
        hanger.SetActive(false);

    }
}
