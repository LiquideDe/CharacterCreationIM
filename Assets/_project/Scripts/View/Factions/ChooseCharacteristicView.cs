using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using R3;

namespace CharacterCreation
{
    public class ChooseCharacteristicView : GarantedCharacteristic
    {
        
        [field: SerializeField] public Toggle Toggle;
        [Inject] private AudioManager _audioManager;
        public bool IsSelected => Toggle.isOn;
        private IDisposable _disposable;
        private bool _isFirstTime = true;

        private void Start()
        {
            _disposable = Toggle.OnValueChangedAsObservable().Subscribe(val =>
            {
                if (!_isFirstTime && val)
                {
                    _audioManager.PlayClick();
                    _isFirstTime = false;
                }
            }).AddTo(this);
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}

