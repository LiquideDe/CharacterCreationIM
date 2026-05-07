using CharacterCreation;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class StartMediator : IDisposable, IInitializable
    {
        private PresenterViewFactory _factory;
        private MainMenuPresenter _mainMenuPresenter;
        private NewPlayerMediator _newPlayerMediator;
        private UpgradeMediator _upgradeMediator;
        private EditCharacterMediator _editCharacterMediator;
        private CompositeDisposable _cd = new CompositeDisposable();
        private CompositeDisposable _ppcd = new CompositeDisposable();
        [Inject] private PrintCharacterPresenter _printPresenter;

        public StartMediator(PresenterViewFactory factory, NewPlayerMediator newPlayerMediator
        , UpgradeMediator upgradeMediator, EditCharacterMediator editCharacterMediator)
        {
            _factory = factory;
            _newPlayerMediator = newPlayerMediator;
            _upgradeMediator = upgradeMediator;
            _editCharacterMediator = editCharacterMediator;
        }

        public void ShowMainMenu()
        {
            _cd = new CompositeDisposable();
            _mainMenuPresenter = (MainMenuPresenter)_factory.Create<MainMenuView>();
            if (_mainMenuPresenter != null)
            {
                _cd.Clear();
                _mainMenuPresenter.CreatePlayerClicked.Subscribe(_ => OnCreatePlayerClicked()).AddTo(_cd);
                _mainMenuPresenter.CreatePatronClicked.Subscribe(_ => OnCreatePatronClicked()).AddTo(_cd);
                _mainMenuPresenter.EditCharacterClicked.Subscribe(_ => OnEditCharacterClicked()).AddTo(_cd);
                _mainMenuPresenter.DevelopCharacterClicked.Subscribe(_ => OnUpgradeCharacterClicked()).AddTo(_cd);
                _mainMenuPresenter.PrintCharacterClicked.Subscribe(_ => OnPrintCharacterClicked()).AddTo(_cd);
                _mainMenuPresenter.ExitClicked.Subscribe(_ => OnExitClicked()).AddTo(_cd);
            }
            else
            {
                Debug.LogError("Failed to create MainMenuPresenter.");
            }
        }

        private void OnCreatePlayerClicked()
        {
            _newPlayerMediator.NewPlayerIsDone.Take(1).Subscribe(_ => ShowMainMenu());
            _newPlayerMediator.ShowNewCharacteristic();
        }

        private void OnCreatePatronClicked()
        {

        }

        private void OnEditCharacterClicked()
        {
            _editCharacterMediator.CharacterEdited.Take(1).Subscribe(_ => ShowMainMenu());
            _editCharacterMediator.ShowLoads();
        }

        private void OnUpgradeCharacterClicked()
        {
            _upgradeMediator.CharacterUpgraded.Take(1).Subscribe(_ => ShowMainMenu());
            _upgradeMediator.ShowLoads();
        }

        private void OnPrintCharacterClicked()
        {
            ShowLoads();
        }

        private void OnExitClicked()
        {

        }

        private void ShowLoads()
        {
            LoadCharacterPresenter presenter = (LoadCharacterPresenter)_factory.Create<LoadCharacterView>();
            presenter.LoadedCharacter.Subscribe(character => { _printPresenter.PrintCharacter(character); }).AddTo(_cd);
            presenter.ShowSaves();
        }

        public void Dispose()
        {
            _cd.Dispose();
            _ppcd.Dispose();
        }

        public void Initialize()
        {
            _printPresenter.WorkIsFinished.Subscribe(_ => ShowMainMenu()).AddTo(_ppcd);
        }
    }
}

