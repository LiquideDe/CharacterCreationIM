using System;
using CharacterCreation.Background;
using R3;
using UnityEngine;

namespace CharacterCreation
{
    public class NewPlayerMediator
    {
        private PresenterViewFactory _factory;
        private ICharacterPresenter _characterPresenter;
        private IDisposable _nextClickedSubscription;
        private Character _character;

        public NewPlayerMediator(PresenterViewFactory factory)
        {
            _factory = factory;
        }

        public void ShowNewCharacteristic()
        {
            _characterPresenter = (ICharacterPresenter)_factory.Create<CharacteristicView>();
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => ShowOrigins(character));
        }

        private void ShowOrigins(Character character)
        {
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<OriginView>();
            _characterPresenter.SetCharacter(_character);
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character =>
            {                
                ShowFaction(character);
            });
        }

        private void ShowFaction(Character character)
        {
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<FactionView>();
            _characterPresenter.SetCharacter(_character);
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => ShowRole(character));
        }

        private void ShowRole(Character character)
        {
            Debug.Log("ShowRole");
        }

        private void Reset()
        {
            _nextClickedSubscription?.Dispose();
            _nextClickedSubscription = null;
            _characterPresenter?.Dispose();
            _characterPresenter = null;
        }
    }
}

