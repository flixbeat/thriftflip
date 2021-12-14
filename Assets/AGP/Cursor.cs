using System.Collections;
using Game;
using UnityEngine;

namespace AGP
{
    public class Cursor : MonoBehaviour
    {
        [SerializeField] private CursorState _state;
        [SerializeField] private float _duration;

        [SerializeField] private float _awakeDelay;
//        [SerializeField] private bool _followPosition;

        [Header("UI")] [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDuration;

        private bool _finished;

        private void Awake()
        {
           StartCoroutine(LateAwake());
            //else enabled = false;
        }

        private IEnumerator LateAwake()
        {
            _finished = true;
            var animator = GetComponent<Animator>();
            animator.enabled = false;
            animator.SetTrigger(_state.ToString());

            var time = 0f;
            while ((time += Time.deltaTime) < _awakeDelay) yield return null;

            animator.enabled = true;
            SetVisible(true);
            _finished = false;
        }

        private void Update()
        {
            if(_finished) return;
            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
            {
                _duration -= Time.deltaTime;
                if (_duration < 0)
                {
                    StopAllCoroutines();
                    SetVisible(false, false);
                    _finished = true;
                }
            }
        }

        private void SetVisible(bool isVisible, bool isEnabled = true)
        {
            StartCoroutine(SetVisible(isVisible ? 1 : 0, isEnabled));
        }

        private IEnumerator SetVisible(float value, bool isEnabled)
        {
            var time = 0f;
            while (time < _fadeDuration)
            {
                time += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, value, time / _fadeDuration);
                yield return null;
            }

            _canvasGroup.alpha = value;
            enabled = isEnabled;
        }
    }

    public enum CursorState
    {
        Hold,
        Swipe,
        SwipeOne,
        Infinity,
        Slide
    }
}