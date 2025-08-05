using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using R3;
using System;
using Zenject;

namespace CharacterCreation
{
    public class MainMenuPresenter : IDisposable, IPresenter, IInitializable
    {
        private readonly MainMenuView _view;
        private readonly AudioManager _audioManager;
        private readonly List<IDisposable> _subscriptions = new();

        // События для внешней логики
        public event Action CreatePlayerClicked;
        public event Action CreatePatronClicked;
        public event Action EditCharacterClicked;
        public event Action DevelopCharacterClicked;
        public event Action PrintCharacterClicked;
        public event Action ExitClicked;

        public MainMenuPresenter(MainMenuView view, AudioManager audioManager)
        {
            _view = view;
            _audioManager = audioManager;            
        }

        public void Dispose()
        {
            foreach (var sub in _subscriptions)
                sub.Dispose();
            _subscriptions.Clear();
        }

        public void Initialize()
        {
            _subscriptions.Add(_view.OnCreatePlayerClicked.Subscribe(_ =>
            {
                _audioManager.PlayClick();
                CreatePlayerClicked?.Invoke();
                _view.HideAndDestroyToLeft();
            }));
            _subscriptions.Add(_view.OnCreatePatronClicked.Subscribe(_ =>
            {
                _audioManager.PlayClick();
                CreatePatronClicked?.Invoke();
                _view.HideAndDestroyToLeft();
            }));
            _subscriptions.Add(_view.OnEditCharacterClicked.Subscribe(_ =>
            {
                _audioManager.PlayClick();
                EditCharacterClicked?.Invoke();
                _view.HideAndDestroyToLeft();
            }));
            _subscriptions.Add(_view.OnDevelopCharacterClicked.Subscribe(_ =>
            {
                _audioManager.PlayClick();
                DevelopCharacterClicked?.Invoke();
                _view.HideAndDestroyToLeft();
            }));
            _subscriptions.Add(_view.OnPrintCharacterClicked.Subscribe(_ =>
            {
                _audioManager.PlayClick();
                PrintCharacterClicked?.Invoke();
                _view.HideAndDestroyToLeft();
            }));
            _subscriptions.Add(_view.OnExitClicked.Subscribe(_ =>
            {
                _audioManager.PlayClick();
                ExitClicked?.Invoke();
                _view.HideAndDestroyToLeft();
            }));
        }
    }
}

