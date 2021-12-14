using UnityEngine;
using UnityEngine.Events;

public class SnappingManager : MonoBehaviour
{
    [SerializeField] private UnityEvent _onFinish;

    public void OnSnap()
    {
        _onFinish.Invoke();
    }
}