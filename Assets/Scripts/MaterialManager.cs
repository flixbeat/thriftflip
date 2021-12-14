using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] _meshRenderers;
    [SerializeField] private Material[] _materials;
    [SerializeField] private Transform[] _parents;

    public void SetMaterial(int index)
    {
        if (_parents != null && _parents.Length > 0)
        {
            foreach (var parent in _parents)
            {
                Utils.SetChildVisible(parent.GetChild(index), true);
            }
            return;
        }

        foreach (var meshRenderer in _meshRenderers)
        {
            meshRenderer.sharedMaterial = _materials[index];
        }
    }
}