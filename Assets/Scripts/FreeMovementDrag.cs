using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FreeMovementDrag : MonoBehaviour
{
    [SerializeField] private Vector3 mouseDelta;
    [SerializeField] private float XMovementSpeed, YMovementSpeed = 1, YrotationSpeed = 1, XrotationSpeed = 0;
    [SerializeField] private Vector2 yPosMinMax = new Vector2(-1.15f, 0.42f), xPosMinMax = new Vector2(-0.4f, 0.4f);
    [SerializeField] private Transform modelToReposition, modelToRotate;
    [SerializeField] private bool lookAtRaycast = true;
    [SerializeField] private LayerMask layerToHit;

    [SerializeField] private Vector2[] YpositionsMinMax;
    [SerializeField] private Vector2[] XpositionsMinMax;
    private bool isDragging;
    private Vector3 lastMousePos;
    Vector3 newPos;

    void Update()
    {
        if (Input.touchCount > 0 && (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))
        {
            mouseDelta = Vector3.zero;
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            mouseDelta = Vector3.zero;
            return;
        }

        HandleInput();
        Move();

        if (lookAtRaycast)
            LookAtRaycast();
    }

    public void SetMinMax(int index)
    {
        yPosMinMax = YpositionsMinMax[index];
        xPosMinMax = XpositionsMinMax[index];
    }

    private void LookAtRaycast()
    {
        if (Physics.Raycast(modelToReposition.position, modelToReposition.forward, out RaycastHit hitInfo, 100, layerToHit))
        {
            Debug.Log(hitInfo.collider.name);

            if (modelToRotate != null)
                modelToRotate.up = -hitInfo.normal;

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(modelToReposition.position, modelToReposition.forward * 100);
    }

    private void Move()
    {
        if (modelToReposition != null)
        {
            modelToReposition.position += ((Vector3.right * mouseDelta.x * XMovementSpeed ) + (Vector3.up * mouseDelta.y * YMovementSpeed)) * Time.fixedDeltaTime * 0.07f;
            modelToReposition.localPosition = new Vector3(Mathf.Clamp(modelToReposition.localPosition.x, xPosMinMax.x, xPosMinMax.y), Mathf.Clamp(modelToReposition.localPosition.y, yPosMinMax.x, yPosMinMax.y), modelToReposition.localPosition.z);
        }
            
        //if (modelToRotate != null)
        //    modelToRotate.eulerAngles += ((Vector3.up * mouseDelta.x * YrotationSpeed) + (Vector3.right * mouseDelta.y * XrotationSpeed)) * Time.deltaTime * 20;
    }

    private void HandleInput()
    {



        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            mouseDelta = Vector3.zero;
        }


        if (isDragging)
            mouseDelta = Input.mousePosition - lastMousePos;

        lastMousePos = Input.mousePosition;
    }
}
