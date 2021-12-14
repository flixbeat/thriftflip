using UnityEngine;

public class CameraController : MonoBehaviour
{
    static public CameraController Instance { get; private set; }


    [SerializeField] private int _step;
    [SerializeField] private bool _isSmooth = true;
    [SerializeField] private Transform _steps;
    [SerializeField] private Transform[] _stepsParents;
    [SerializeField] private Transform[] _stepsGroup;
    [SerializeField] private float _followSpeed = 5;
    [SerializeField] private float _rotationSpeed = 2;

    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _offsetSpeed = 5;
    [SerializeField] private float _offsetValue = .2f;


    private Transform _target;
    private Quaternion _rotation;
    private Vector3 _mousePosition;
    private float _currentAngle;

    private void Awake()
    {
        SetStep(_step);
        Instance = this;
    }


    public void SetSmooth(bool isSmooth)
    {
        _isSmooth = isSmooth;
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        transform.position = _isSmooth ? Vector3.Lerp(transform.position, _target.position + _offset, _followSpeed * Time.deltaTime) : _target.position + _offset;
        transform.rotation = _isSmooth ? Quaternion.Lerp(transform.rotation, _target.rotation, _rotationSpeed * Time.deltaTime) : _target.rotation;
    }

    public void NextStep()
    {
        SetStep(_step + 1);
    }

    public void SetSubStep(int substep)
    {
        SetStep(_step, true, substep);
    }

    public void SetSubStepDirect(int substep)
    {
        SetStep(_step, false, substep);
    }

    public void NextStepDirect()
    {
        SetStep(_step + 1, false);
    }

    public void ToStep(int step)
    {
        SetStep(step);
    }

    public void ToStepDirect(int step)
    {
        SetStep(step, false);
    }

    public void SetStep(int step, bool animated = true, int substep = -1)
    {
        _step = Mathf.Clamp(step, 0, _steps.childCount - 1);
        if (_stepsGroup.Length > 0)
        {
            foreach (var gr in _stepsGroup) gr.gameObject.SetActive(false);
            _stepsGroup[_step].gameObject.SetActive(true);
        }

        foreach (var parent in _stepsParents) Utils.SetChildVisible(parent.GetChild(_step), true);

        _target = _steps.GetChild(_step);
        if (substep > -1)
        {
            _target = _target.GetChild(substep);
            _target.gameObject.SetActive(true);
        }

        if (animated) return;
        transform.position = _target.position;
        transform.rotation = _target.rotation;
    }

    public void PrevStep()
    {
        SetStep(_step - 1);
    }

    public void SetFinalStep()
    {
        SetStep(_steps.childCount - 1);
    }

    public void SetDirection(Vector3 mouseDirection)
    {
        _offset = Vector3.Lerp(_offset, mouseDirection.normalized * _offsetValue, _offsetSpeed * Time.deltaTime);
    }
}