using System;
using System.Collections;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public bool _isRotating, enable;
    [SerializeField] private float _angle;
    [SerializeField] private float _speed;
    [SerializeField] private Vector2 _limit;
    [SerializeField] private Animator animToToggleWhenRotating;

    [SerializeField] private Transform _target;

    [SerializeField] private Quaternion _rotation;
    [SerializeField] private Vector3 _mousePosition;
    [SerializeField] private float _currentAngle;
    [SerializeField] private float direction;

    private void Start()
    {
        direction = 0.03f;
    }

    public void Setup()
    {
        StopAllCoroutines();
        Resetval();
        StartCoroutine(EnableRoutine());
    }

    public void Resetval()
    {
        enable = false;
        _currentAngle = 0;
        _isRotating = false;
        _rotation = Quaternion.Euler(_target.eulerAngles);
        transform.rotation = Quaternion.Euler(_target.eulerAngles);
    }

    private IEnumerator EnableRoutine()
    {
        yield return new WaitForSeconds(1);
        _isRotating = false;
        enable = true;
    }


    public void SetRotating(bool isRotating)
    {
        _isRotating = isRotating;
        if (!isRotating) _rotation = Quaternion.identity;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isRotating = true;
            _mousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isRotating = false;
        }

        if (_isRotating)
        {
            if (enable)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _mousePosition = Input.mousePosition;
                }
                else if (Input.GetMouseButton(0))
                {
                    // _currentAngle = _target.eulerAngles.y;
                    _currentAngle += (Input.mousePosition - _mousePosition).normalized.x * _angle * _speed * Time.deltaTime;
                    _mousePosition = Input.mousePosition;
                }
            }

        }
        else
        {
            if (enable)
            {
                _currentAngle += direction * _angle * _speed * Time.deltaTime;

                if (_currentAngle >= _limit.y || _currentAngle <= _limit.x)
                    direction *= -1;
            }
           
        }

        if (enable)
        {
            _currentAngle = Mathf.Clamp(_currentAngle, _limit.x, _limit.y);
            _rotation = Quaternion.Lerp(_rotation, Quaternion.Euler(_target.eulerAngles + _currentAngle * Vector3.up), _speed * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        if (enable)
            transform.rotation = Quaternion.Lerp(transform.rotation, /*_isRotating ?*/ _rotation /*: _target.rotation*/, 5 * Time.deltaTime);
    }
}