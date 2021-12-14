using System;
using UnityEngine;
using UnityEngine.Events;

public class BlendShapesController : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField] private float _blendSpeed;
    [SerializeField] private int _rightIndex;
    [SerializeField] private int _leftIndex;
    [SerializeField] private UnityEvent _onFinish;

    private Vector3 _position;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) _position = Input.mousePosition;
        else if (Input.GetMouseButton(0))
        {
            var delta = Input.mousePosition - _position;
            if (delta.x > 0) SwipeRight();
            else if (delta.x < 0) SwipeLeft();
        }
    }

    private void SwipeLeft()
    {
        var value = _skinnedMeshRenderer.GetBlendShapeWeight(_leftIndex) + _blendSpeed * Time.deltaTime;
        if (value > 99.9f)
        {
            value = 100;
            Check(_rightIndex);
        }
        _skinnedMeshRenderer.SetBlendShapeWeight(_leftIndex, value);
        
    }

    private void SwipeRight()
    {
        var value = _skinnedMeshRenderer.GetBlendShapeWeight(_rightIndex) + _blendSpeed * Time.deltaTime;
        if (value > 99.9f)
        {
            value = 100;
            Check(_leftIndex);
        }
        _skinnedMeshRenderer.SetBlendShapeWeight(_rightIndex, value);
    }

    private void Check(int index)
    {
        if (_skinnedMeshRenderer.GetBlendShapeWeight(index) > 99.9f)
        {
            enabled = false;
            _onFinish.Invoke();
        }
    }
}