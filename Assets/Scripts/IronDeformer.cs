using UnityEngine;
using UnityEngine.Events;

public class IronDeformer : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] _preTargets;
    [SerializeField] private SkinnedMeshRenderer _postTarget;
    [SerializeField] private IronController _ironController;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Transform _cursor;
    [SerializeField] private bool _normalize;
    [SerializeField] private float _duration;
    [SerializeField] private float _forceOffset = 0.1f;
    [SerializeField] private UnityEvent _onCompleted;

    private Vector3 _lastPosition;
    private Animator _cursorAnimator;

    private bool _isCompleted;

    public void OnGameStart()
    {
        Debug.Log("OnGameStart");
        _ironController.SetTargets(_preTargets, _postTarget);
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.R)) _ironController.ResetMesh();
        else if (Input.GetMouseButtonUp(0)) SetParticlesActive(false);
        else if (Input.GetMouseButton(0))
        {
            HandleInput();
        }

        if (_cursor == null) return;
        SetActive(Vector3.Distance(_lastPosition, _cursor.position) > .1f);
        _lastPosition = _cursor.position;
    }

    private void OnEnable()
    {
        if (_cursor == null) return;
        _cursorAnimator = _cursor.GetComponentInChildren<Animator>();
        _lastPosition = _cursor.position;
        _cursor.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (_cursor != null) _cursor.gameObject.SetActive(false);
    }

    private void HandleInput()
    {
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
        {
            SetParticlesActive(false);
            return;
        }

        var ironController = hit.collider.transform == _ironController.transform ? _ironController : null;
        if (_cursor != null)
        {
            _cursor.position = hit.point;
            if (_normalize) _cursor.up = hit.normal;
        }

        if (ironController == null) return;

        _duration -= Time.deltaTime;

        if (_duration < 0 && !_isCompleted)
        {
            _isCompleted = true;
            _onCompleted.Invoke();
        }
        SetParticlesActive(true);
        var point = hit.point;
        point += hit.normal * _forceOffset;
        ironController.AddDeformingForce(point);
    }

    private void SetActive(bool active)
    {
        if (_cursorAnimator != null) _cursorAnimator.SetBool("contact", active);
    }

    private void SetParticlesActive(bool active)
    {
        if (_particleSystem != null && _particleSystem.emission.enabled != active)
        {
            var particleSystemEmission = _particleSystem.emission;
            particleSystemEmission.enabled = active;
        }
    }
}