using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using R3;

namespace CharacterCreation.Background
{
    public class TalentInListView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [field:SerializeField] public Toggle Toggle;
        [SerializeField] private InfoButtonView _infoButtonView;
        [Inject] private AudioManager _audioManager;
        private List<string> _talents = new List<string>();   
        private IDisposable _disposable;

        public List<string> Talents => _talents;
        public bool IsSelected => Toggle.isOn;
        private bool _isFirstTime = true;

        private void Start()
        {
            _disposable = Toggle.OnValueChangedAsObservable().Subscribe(val =>
            {
                if (!_isFirstTime && val)
                {
                    _audioManager.PlayClick();
                }
                else { _isFirstTime = false; }
            }).AddTo(this);
        }

        public void AddTalent(string nameTalent)
        {
            _talents.Add(nameTalent);
            _infoButtonView.SetInfo(nameTalent);
            SetText();
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
            Toggle.onValueChanged.RemoveAllListeners();
        }

        private void SetText()
        {
            _nameText.text = string.Empty;
            foreach (var item in _talents)
            {
                _nameText.text += item;
                if(item != _talents[^1])
                    _nameText.text += ", ";
            }
        }

    }
}

