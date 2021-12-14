using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public class GroupSelector : MonoBehaviour
    {
        [SerializeField] private UnlockType unlockType;
        [SerializeField] private Transform _parent;
        [SerializeField] private Sprite[] _backgrounds;
        [SerializeField] private bool _animateClick;
        [SerializeField] private UnityEvent _onConfirmation;
        [SerializeField] private IntEvent _onIndexChange;
        [SerializeField] private bool isMoldStep;

        private int _level;
        private void Start()
        {
            //_level = Prefs.GetUpgrade(name);

            // UNCOMMENT THIS TO ENABLE THE LOCKING MECHANISM
            //_level = UnlockSystem.instance.CountUnlockType(unlockType) + 4;
            _level = 999;

            var toggles = GetComponentsInChildren<Toggle>();
            for (var i = 0; i < toggles.Length; i++)
            {
                var toggle = toggles[i];
                toggle.interactable = i <= _level;
                toggle.transform.Find("Lock").gameObject.SetActive(!toggle.interactable);

                toggle.onValueChanged.AddListener(isOn => OnClick(toggle.transform, isOn));
            }
            //GetComponentInChildren<Scrollbar>().value = 0;
        }

        public Sprite[] GetBackgrounds()
        {
            return _backgrounds;
        }


        public void OnClick(Transform selector, bool isOn)
        {
            if(!isOn) return;
            var index = selector.GetSiblingIndex();
            if (_parent != null)
            {
                var parent = _parent.GetChild(index);
                //Utils.SetChildVisible(parent, true);
                _onIndexChange.Invoke(index);
            }
            else _onIndexChange.Invoke(index);

            if (_animateClick && _backgrounds.Length > 1)
            {
                var value = index * 1f / (_backgrounds.Length - 1);
                GetComponentInChildren<Scrollbar>().value = Mathf.Abs(value);
            }
        }

        public void Confirm()
        {
            _onConfirmation.Invoke();
        }

        public Sprite GetNextIcon()
        {
            return _level < _backgrounds.Length - 1 ? _backgrounds[_level + 1] : null;
        }

        public void UnlockLevel()
        {
            _level++;
            //Prefs.Upgrade(name);
        }
    }

    [Serializable]
    public class IntEvent : UnityEvent<int>
    {
    }
}