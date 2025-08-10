using CharacterCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreation
{
    public class StartMediator
    {
        private PresenterViewFactory _factory;
        private MainMenuPresenter _mainMenuPresenter;
        private NewPlayerMediator _newPlayerMediator;

        public StartMediator(PresenterViewFactory factory, NewPlayerMediator newPlayerMediator)
        {
            _factory = factory;
            _newPlayerMediator = newPlayerMediator;
        }

        public void ShowMainMenu()
        {
            _mainMenuPresenter = (MainMenuPresenter)_factory.Create<MainMenuView>();
            if (_mainMenuPresenter != null)
            {
                _mainMenuPresenter.CreatePlayerClicked += OnCreatePlayerClicked;
                _mainMenuPresenter.CreatePatronClicked += OnCreatePatronClicked;
                _mainMenuPresenter.EditCharacterClicked += OnEditCharacterClicked;
                _mainMenuPresenter.DevelopCharacterClicked += OnDevelopCharacterClicked;
                _mainMenuPresenter.PrintCharacterClicked += OnPrintCharacterClicked;
                _mainMenuPresenter.ExitClicked += OnExitClicked;
            }
            else
            {
                Debug.LogError("Failed to create MainMenuPresenter.");
            }
        }

        private void UnsubscribeMainMenuPresenter()
        {
            if (_mainMenuPresenter != null)
            {
                _mainMenuPresenter.CreatePlayerClicked -= OnCreatePlayerClicked;
                _mainMenuPresenter.CreatePatronClicked -= OnCreatePatronClicked;
                _mainMenuPresenter.EditCharacterClicked -= OnEditCharacterClicked;
                _mainMenuPresenter.DevelopCharacterClicked -= OnDevelopCharacterClicked;
                _mainMenuPresenter.PrintCharacterClicked -= OnPrintCharacterClicked;
                _mainMenuPresenter.ExitClicked -= OnExitClicked;
                _mainMenuPresenter.Dispose();
                _mainMenuPresenter = null;
            }
        }

        private void OnCreatePlayerClicked()
        {
            UnsubscribeMainMenuPresenter();
            _newPlayerMediator.ShowNewCharacteristic();
        }

        private void OnCreatePatronClicked()
        {
            UnsubscribeMainMenuPresenter();
        }

        private void OnEditCharacterClicked()
        {
            UnsubscribeMainMenuPresenter();
        }

        private void OnDevelopCharacterClicked()
        {
            UnsubscribeMainMenuPresenter();
        }

        private void OnPrintCharacterClicked()
        {
            UnsubscribeMainMenuPresenter();
        }

        private void OnExitClicked()
        {
            UnsubscribeMainMenuPresenter();
        }
    }
}

