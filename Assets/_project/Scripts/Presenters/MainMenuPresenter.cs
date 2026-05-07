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
        public Observable<Unit> CreatePlayerClicked => _createPlayer;
        public Observable<Unit> CreatePatronClicked => _createPatron;
        public Observable<Unit> EditCharacterClicked => _editCharacter;
        public Observable<Unit> DevelopCharacterClicked => _developCharacter;
        public Observable<Unit> PrintCharacterClicked => _printCharacter;
        public Observable<Unit> ExitClicked => _exit;

        private Subject<Unit> _createPlayer = new Subject<Unit>();
        private Subject<Unit> _createPatron = new Subject<Unit>();
        private Subject<Unit> _editCharacter = new Subject<Unit>();
        private Subject<Unit> _developCharacter = new Subject<Unit>();
        private Subject<Unit> _printCharacter = new Subject<Unit>();
        private Subject<Unit> _exit = new Subject<Unit>();

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
                _createPlayer.OnNext(Unit.Default);
                _view.HideAndDestroyToLeft();
            }));
            _subscriptions.Add(_view.OnCreatePatronClicked.Subscribe(_ =>
            {
                _audioManager.PlayClick();
                _createPatron.OnNext(Unit.Default);
                _view.HideAndDestroyToLeft();
            }));
            _subscriptions.Add(_view.OnEditCharacterClicked.Subscribe(_ =>
            {
                _audioManager.PlayClick();
                _editCharacter.OnNext(Unit.Default);
                _view.HideAndDestroyToLeft();
            }));
            _subscriptions.Add(_view.OnDevelopCharacterClicked.Subscribe(_ =>
            {
                _audioManager.PlayClick();
                _developCharacter.OnNext(Unit.Default);
                _view.HideAndDestroyToLeft();
            }));
            _subscriptions.Add(_view.OnPrintCharacterClicked.Subscribe(_ =>
            {
                _audioManager.PlayClick();
                _view.HideAndDestroyToLeft(() => _printCharacter.OnNext(Unit.Default));
            }));
            _subscriptions.Add(_view.OnExitClicked.Subscribe(_ =>
            {
                _audioManager.PlayClick();
                _exit.OnNext(Unit.Default);
                _view.HideAndDestroyToLeft();
            }));
        }
    }
}

