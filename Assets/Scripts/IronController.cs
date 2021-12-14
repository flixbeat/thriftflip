using UnityEngine;

public class IronController : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private Vector3 _axis = Vector3.up * .5f;
    [SerializeField] private float _radius = .3f;
    [SerializeField] private float _speed = .5f;

    [Header("Displacement")] [SerializeField]
    private Vector3[] _displacedVertices;

    Mesh deformingMesh;
    Vector3[] originalVertices;


    private bool _emit;

    private void UpdateMesh()
    {
        deformingMesh.vertices = _displacedVertices;
        deformingMesh.RecalculateNormals();
    }

    // private void Update()
    // {
    //     if (_isIroning) direction = 0;
    //     else if (Input.GetMouseButtonDown(0)) direction = 1;
    //     else if (Input.GetMouseButtonDown(1)) direction = -1;
    //
    //     if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) HandleInput();
    // }

    public void AddDeformingForce(Vector3 point)
    {
        for (var i = 0; i < _displacedVertices.Length; i++) AddForceToVertex(i, point);
        UpdateMesh();
    }

    private void AddForceToVertex(int i, Vector3 point)
    {
        var pointToVertex = originalVertices[i] - transform.InverseTransformPoint(point);
        if (pointToVertex.sqrMagnitude > _radius) return;
        //var velocity = pointToVertex.sqrMagnitude / _radius * Time.deltaTime;
        _displacedVertices[i] = Vector3.Lerp(_displacedVertices[i], originalVertices[i] + _axis, _speed * Time.deltaTime);
    }

    public void ResetMesh()
    {
        _displacedVertices = originalVertices;
        UpdateMesh();
    }

    private void BuildMesh()
    {
        deformingMesh = _meshFilter.mesh;
        originalVertices = deformingMesh.vertices;

        if (_displacedVertices.Length == originalVertices.Length)
        {
            Debug.Log("Updated Mesh");
            UpdateMesh();
            return;
        }

        Debug.Log("New Mesh");
        _displacedVertices = new Vector3[originalVertices.Length];
        for (var i = 0; i < originalVertices.Length; i++)
        {
            _displacedVertices[i] = originalVertices[i];
        }
    }

    public void SetTargets(SkinnedMeshRenderer[] preTargets, SkinnedMeshRenderer postTarget)
    {
        postTarget.sharedMesh = _meshFilter.sharedMesh;
        BuildMesh();
        var postMesh = Instantiate(deformingMesh);
        foreach (var preTarget in preTargets) preTarget.sharedMesh = postMesh;
    }
}