using UnityEngine;
using UnityEngine.Events;

public class ObjectSnapper : MonoBehaviour
{
    [SerializeField] private Transform[] _snapParents;
    [SerializeField] private float _snapDistance;
    [SerializeField] private float _snapSpeed;
    [SerializeField] private UnityEvent _onSnap;
    [SerializeField] private UnityEvent _onFinish;

    private int _index;
//
//    private Transform GetParent()
//    {
//        foreach (var parent in _snapParents)
//        {
//            if (parent.childCount == 0 && Vector3.Distance(transform.position, parent.position) < _snapDistance)
//            {
//                transform.position = parent.position;
//                _parents.Remove(parent);
//                _onSnap.Invoke();
//
//                Debug.Log("Snapped", parent);
//                return parent;
//            }
//        }
//
//        return null;
//    }

    private void Update()
    {
        if (_index >= _snapParents.Length)
        {
            Finish();
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        var parent = _snapParents[_index];
        transform.position = Vector3.Lerp(transform.position, parent.position, _snapSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, parent.rotation, _snapSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, parent.position) < _snapDistance)
        {
            _onSnap.Invoke();
            Utils.SetChildVisible(transform.GetChild(_index).GetChild(0), false);
            _index++;
        }
    }

    private void Finish()
    {
        Debug.Log("Snapped");
        enabled = false;
        GetComponentInParent<SnappingManager>().OnSnap();
    }
}