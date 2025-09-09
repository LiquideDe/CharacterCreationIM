using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreation
{
    public class ConnectionPresenter : ICharacterPresenter
    {
        private readonly Subject<Character> _nextClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        private readonly AudioManager _audioManager;
        private ConnectionView _view;
        private readonly List<IDisposable> _subscriptions = new();
        private Character _character;

        public ConnectionPresenter(AudioManager audioManager, ConnectionView view)
        {
            _audioManager = audioManager;
            _view = view;
        }

        public void SetCharacter(Character character)
        {
            _character = character;
        }

        public void Initialize()
        {
            _subscriptions.Add(
                _view.OnNextButtonClick.Subscribe(_ =>
                {
                    SetConnection();
                })
            );
        }

        public void Dispose()
        {
            _nextClicked.Dispose();
            foreach (var sub in _subscriptions)
                sub.Dispose();
            _subscriptions.Clear();
        }

        private void SetConnection()
        {
            _character.Connections.Value = _view.Text;
            _nextClicked.OnNext(_character);
            _view.HideAndDestroyToLeft();
        }
    }
}

