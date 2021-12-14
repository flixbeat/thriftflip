using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class FollowRaycast : MonoBehaviour
{
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private bool isActivated = true, faceTowardsHitReflection;
    [SerializeField] private float speed = 1;
    [SerializeField] private Vector3 offset;
    private Vector3 targetPos, targetRotationUp;

    void Start()
    {
        ResetTarget();
    }

    void Update()
    {

        if (Input.touchCount > 0 && (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))
            return;

        if (EventSystem.current.IsPointerOverGameObject() || !isActivated) 
            return;

        if (Input.GetMouseButton(0))
        {
            UpdateTarget();
            UpdatePosition();
        }
    }

    public void Activate(bool isActivated)
    {
        this.isActivated = isActivated;
    }

    private void ResetTarget()
    {
        targetPos = transform.position;
    }

    private void UpdateTarget()
    {
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + (Vector3.up * Screen.height * 0.1f));

        if (Physics.Raycast(raycastOrigin.transform.position, raycastOrigin.forward, out RaycastHit hitInfo, Mathf.Infinity))
        {
            targetPos = hitInfo.point;
            targetRotationUp = -hitInfo.normal;
        }
    }

    public void SetTarget(Transform newTarget, bool alsoUpdateRotationUp = false)
    {
        targetPos = newTarget.position;

        if (alsoUpdateRotationUp)
            targetRotationUp = newTarget.transform.up;
    }

    private void UpdatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos + offset, Time.deltaTime * speed);

        if (faceTowardsHitReflection)
            transform.up = Vector3.Lerp(transform.up, targetRotationUp, Time.deltaTime * speed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(raycastOrigin.position, raycastOrigin.position + raycastOrigin.forward * 100);
    }
}
