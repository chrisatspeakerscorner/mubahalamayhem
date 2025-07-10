using DG.Tweening;
using Mubahala.Controller;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mubahala.View
{
    public class OptionView : MonoBehaviour
    {
        public OptionData OptionData => _optionData;

        public TextMeshProUGUI Text => _text;

        public Action<GameObject> OnOptionSelected;
        public Action<GameObject> OnOptionUnselected;

        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private TextMeshProUGUI _stampText;
        [SerializeField] private Button _button;
        [SerializeField] private Image _stamp;

        private OptionData _optionData;

        private bool _selected;
        private Tween _animationTween;

        private Coroutine _coroutine;

        private void Awake()
        {
            _button.onClick.AddListener(HandleButtonPressed);
        }

        public void Init(OptionData optionData)
        {
            _optionData = optionData;

            _stamp.gameObject.SetActive(false);

            _text.text = optionData.Text;

            _selected = false;

            if (null != _coroutine)
            {
                StopCoroutine(_coroutine);
            }
        }

        public void SetSelectedFirst()
        {
            _stamp.color = Color.cyan;
            _stamp.gameObject.SetActive(true);
            _stampText.text = "1";
        }

        public void SetSelectedSecond()
        {
            _stamp.color = Color.yellow;
            _stamp.gameObject.SetActive(true);
            _stampText.text = "2";
        }

        public void SetSelectedThird()
        {
            _stamp.color = Color.green;
            _stamp.gameObject.SetActive(true);
            _stampText.text = "3";
        }

        public void SetUnselected()
        {
            _stamp.color = Color.white;
            _stamp.gameObject.SetActive(false);
        }

        public void SetInteractable(bool state)
        {
            _button.interactable = state;
        }

        public void SetState(bool state, float delay)
        {
            if(!state)
            {
                _animationTween.Kill();

                gameObject.SetActive(false);
            }
            else
            {
                _animationTween = DOVirtual.DelayedCall(delay, () =>
                {
                    gameObject.SetActive(state);
                }).SetLink(gameObject);
            }
        }

        private void HandleButtonPressed()
        {
            if(_selected)
            {
                OnOptionUnselected?.Invoke(this.gameObject);
            }
            if (!_selected)
            {
                OnOptionSelected?.Invoke(this.gameObject);
            }

            _selected = !_selected;
        }
    }
}