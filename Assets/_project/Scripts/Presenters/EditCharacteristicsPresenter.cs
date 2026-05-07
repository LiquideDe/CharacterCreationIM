using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreation
{
    public class EditCharacteristicsPresenter : ICharacterPresenter
    {
        private readonly Subject<Character> _nextClicked = new();
        private readonly Subject<Character> _prevClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        public Observable<Character> PrevClicked => _prevClicked;
        private readonly AudioManager _audioManager;
        private readonly EditCharacteristicsView _view;
        private readonly CompositeDisposable _cd = new CompositeDisposable();
        private Character _character;
        private readonly Dictionary<Characteristic, CharacteristicEditPanel> _map = new();

        public EditCharacteristicsPresenter(AudioManager audioManager, EditCharacteristicsView view)
        {
            _audioManager = audioManager;
            _view = view;
        }

        public void Initialize()
        {
            _view.OnButtonNextClick.Subscribe(_ => { GoNext(); }).AddTo(_cd);
            _view.OnButtonPrevClick.Subscribe(_ => { GoPrev(); }).AddTo(_cd);
        }

        public void SetCharacter(Character character)
        {
            _character = character;
            BuildList();
        }

        public void Dispose()
        {
            _cd.Dispose();
        }

        private void BuildList()
        {
            _view.Clear();
            _map.Clear();
            foreach (var ch in _character.Characteristics)
            {
                var panel = _view.AddPanel();
                panel.SetName(ch.Name);
                panel.SetValue(ch.Level);
                panel.OnPlusClick.Subscribe(_ => { Adjust(ch, +1); }).AddTo(_cd);
                panel.OnMinusClick.Subscribe(_ => { Adjust(ch, -1); }).AddTo(_cd);
                _map[ch] = panel;
            }
        }

        private void Adjust(Characteristic ch, int delta)
        {
            int newValue = Mathf.Clamp(ch.Level + delta, 0, 100);
            ch.Level = newValue;
            ch.BaseLevel = newValue;
            ch.EmitCurrentLevel();

            if (_map.TryGetValue(ch, out var panel))
                panel.SetValue(newValue);
        }

        private void GoNext()
        {
            _audioManager.PlayClick();
            _view.HideAndDestroyToLeft();
            _nextClicked.OnNext(_character);
        }

        private void GoPrev()
        {
            _audioManager.PlayClick();
            _view.HideAndDestroyToRight();
            _prevClicked.OnNext(_character);
        }
    }
}
