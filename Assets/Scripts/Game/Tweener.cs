using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 10;
    [SerializeField] private float scaleSpeed = 10;

    private void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
            //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, target.eulerAngles, Time.deltaTime * speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * speed);
            transform.localScale = Vector3.Lerp(transform.localScale, target.localScale , Time.deltaTime * scaleSpeed);

        }
    }

    public void UpdateTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetScaleSpeed(float newSpeed)
    {
        scaleSpeed = newSpeed;
    }
}
