using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AwakeEvent : MonoBehaviour
{
    [SerializeField] private float _delay;
    [SerializeField] private UnityEvent _onAwake;

    private void Start()
    {
        Debug.Log(name + " " + _delay, this);
        if(_delay>0) StartCoroutine(Call());
        else _onAwake.Invoke();
    }

    private IEnumerator Call()
    {
        Debug.Log(name + " Start", this);
        yield return new WaitForSeconds(_delay);
        _onAwake.Invoke();
        Debug.Log(name + " End", this);
    }
}