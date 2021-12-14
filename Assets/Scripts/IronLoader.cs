using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IronLoader : MonoBehaviour
{
    [Range(0,1)] public float value;
    [SerializeField] private float speedMultiplier = 1;
    [SerializeField] private Image filler;
    [SerializeField] private GameObject fireFX;
    [SerializeField] private Transform fireSpawnPoint;
    [SerializeField] private Painter painterScript;

    public bool isActivated;
    private float speed = 1;
    private Vector3 lastMousePos;
    private void Update()
    {
        if (!isActivated)
            return;

        if (lastMousePos == Input.mousePosition)
            speed = 0.5f;
        else
            speed = -0.5f;

        value = Mathf.Clamp01(value + speed * Time.deltaTime * speedMultiplier);
        filler.fillAmount = value;

        lastMousePos = Input.mousePosition;

        if (value >= 0.99f)
        {
            value = 0;
            GameManager.instance.numOfWrongAdjustments++;
            GameObject o = Instantiate(fireFX, null, false);
            o.transform.position = fireSpawnPoint.position;
            LevelManager.instance.headAnim.SetTrigger("mad");
            painterScript.ResetPainter();
        }
    }
}
