using CharacterCreation;
using R3;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace CharacterCreation
{
    public class InfoPanelView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI _text = null;
        [SerializeField] private Button _closeButton = null;

        [Inject] private AudioManager _audioManager = null;
        private CompositeDisposable _disposable = new CompositeDisposable();
        public void SetText(string text)
        {
            _text.text = text;
        }

        protected override void Awake()
        {
            base.Awake();
            _closeButton.onClick
            .AsObservable()
            .Subscribe(_ =>
            {
                _audioManager.PlayClick();
                HideAndDestroyToRight();
            })
            .AddTo(_disposable);
        }

        private void Start()
        {
            Show();
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}

