using UnityEngine;

namespace Deformation
{
    public class MeshReceiver : MonoBehaviour
    {
        public MeshDeformer meshDeformer;
        public float springForce;

        private void Awake()
        {
            if (meshDeformer == null) meshDeformer = GetComponent<MeshDeformer>();
        }

        public void AddDeformingForce(Vector3 point, float force)
        {
            meshDeformer.AddDeformingForce(point, force, springForce);
        }
    }
}