using System.Collections;
using TMPro;
using UnityEngine;

public class NumberAnimator : MonoBehaviour
{
    [SerializeField] private Vector2 _value;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private float _duration;

    private void Start()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        var time = 0f;
        var value = Random.Range(_value.x, _value.y);
        while (time < _duration)
        {
            time += Time.deltaTime;
            SetNumber(value * _curve.Evaluate(time / _duration));
            yield return null;
        }

        SetNumber(_value.y);
        yield return null;
    }

    private void SetNumber(float value)
    {
        _text.SetText(Mathf.RoundToInt(value).ToString());
    }
}