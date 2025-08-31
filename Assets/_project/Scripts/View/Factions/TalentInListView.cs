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
        [SerializeField] private Toggle _toggle;
        [SerializeField] private InfoButtonView _infoButtonView;
        [Inject] private AudioManager _audioManager;
        private List<string> _talents = new List<string>();   
        private IDisposable _disposable;

        public List<string> Talents => _talents;
        public bool IsSelected => _toggle.isOn;

        private void Start()
        {
            _disposable = _toggle.OnValueChangedAsObservable().Subscribe(val =>
            {
                _audioManager.PlayClick();
            }).AddTo(this);
        }

        public void AddTalent(string nameTalent)
        {
            _talents.Add(nameTalent);
            _infoButtonView.SetInfo(nameTalent);
            SetText();
        }

        public void SetToggleGroup(ToggleGroup toggleGroup) => _toggle.group = toggleGroup;

        private void OnDestroy()
        {
            _disposable?.Dispose();
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

