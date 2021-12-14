using UnityEngine;
using UnityEngine.Events;

public class CuttingManager : MonoBehaviour
{
    [SerializeField] private UnityEvent _onFinish;

    public void OnCut()
    {
        _onFinish.Invoke();
    }
}