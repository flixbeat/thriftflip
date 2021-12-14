using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LineFollower : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;

    [Header("Cursor")] [SerializeField] private Transform _cursor;
    [SerializeField] private bool _swipe;
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] [Range(0, 1)] private float _value;

    [Header("Cut")] [SerializeField] private Transform _cut;

    // [SerializeField] private float _dashFrequency = 4;
    [SerializeField] private bool _isFinal;
    [SerializeField] private float _finishDelay;
    [SerializeField] private UnityEvent _onFinish;

    private Vector3[] _positions;
    private float[] _distances;
    private float _length;
    private MaterialPropertyBlock _propertyBlock;

    private Result _result;
    private Animator _cursorAnimator;
    private Vector3 _mousePosition;

    private void Start()
    {
        _cursorAnimator = _cursor.GetComponentInChildren<Animator>();
        // _cursor.GetComponentInChildren<TrailRenderer>().emitting = true;
        SetLineRenderer(_lineRenderer);
    }

    private void SetLineRenderer(LineRenderer lineRenderer)
    {
        _value = 0;

        _positions = new Vector3[lineRenderer.positionCount + (lineRenderer.loop ? 1 : 0)];
        lineRenderer.GetPositions(_positions);
        if (lineRenderer.loop) _positions[_positions.Length - 1] = _positions[0];

        _distances = new float[_positions.Length];
        for (var i = 1; i < _distances.Length; i++) _distances[i] = _distances[i - 1] + Vector3.Distance(_positions[i], _positions[i - 1]);

        _length = _distances[_distances.Length - 1];
        for (var i = 0; i < _distances.Length; i++) _distances[i] /= _length;

        _propertyBlock = new MaterialPropertyBlock();
        lineRenderer.GetPropertyBlock(_propertyBlock);
        // _propertyBlock.SetFloat("_DashFrequency", _dashFrequency);
        // _lineRenderer.SetPropertyBlock(_propertyBlock);

        _lineRenderer = lineRenderer;
    }

    private void OnEnable()
    {
        _lineRenderer.enabled = enabled;
    }

    private void OnDisable()
    {
        _lineRenderer.enabled = enabled;
    }

    private void Update()
    {
        var holding = false;
        var moving = false;

        if (_swipe)
        {
            if (Input.GetMouseButtonDown(0)) _mousePosition = Input.mousePosition;
            else if (Input.GetMouseButton(0)) moving = (Input.mousePosition.y - _mousePosition.y) < 0;
        }
        else moving = Input.GetMouseButton(0);


        if (moving)
        {
            _value = Mathf.Clamp01(_value + _speed * Time.deltaTime);
            holding = true;
        }

        if (_cursorAnimator != null) _cursorAnimator.SetBool("holding", holding);

        _propertyBlock.SetFloat("_Progress", _value);
        _lineRenderer.SetPropertyBlock(_propertyBlock);

        var result = Evaluate(_value);
        _cursor.position = result.Position;
        _cursor.rotation = Quaternion.Lerp(_cursor.rotation, result.Rotation, _rotationSpeed * Time.deltaTime);
        _cursor.gameObject.SetActive(true);

        if (_value >= 1) Finish();
    }

    private void Finish()
    {
        enabled = false;
        // _cursor.GetComponentInChildren<TrailRenderer>().emitting = false;

        StartCoroutine(LateFinish());
    }

    private IEnumerator LateFinish()
    {
        _cursor.gameObject.SetActive(false);
        _cut.GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(_finishDelay);

        _onFinish.Invoke();
        if (_cursorAnimator != null) _cursorAnimator.SetBool("holding", false);
        if (_isFinal) GetComponentInParent<CuttingManager>().OnCut();
    }

    private Result Evaluate(float value)
    {
        if (_result == null) _result = new Result();

        if (value <= 0)
        {
            _result.Position = _positions[0];
            _result.Rotation = Quaternion.LookRotation(_positions[1] - _positions[0], Vector3.forward);
        }

        else if (value >= 1)
        {
            _result.Position = _positions[_positions.Length - 1];
            _result.Rotation = Quaternion.LookRotation(_positions[_positions.Length - 1] - _positions[_positions.Length - 2], Vector3.forward);
        }

        else
        {
            for (var i = 0; i < _distances.Length; i++)
            {
                if (_distances[i] <= value) continue;
                _result.Position = Vector3.Lerp(_positions[i - 1], _positions[i], (value - _distances[i - 1]) / (_distances[i] - _distances[i - 1]));
                _result.Rotation = Quaternion.LookRotation(_positions[i] - _positions[i - 1], Vector3.forward);
                break;
            }
        }

        return _result;
    }
}

internal class Result
{
    public Vector3 Position;
    public Quaternion Rotation;
}