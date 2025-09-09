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
        private CharacterBackgroundView _characterBackground;

        public NewPlayerMediator(PresenterViewFactory factory, CharacterBackgroundView characterBackground)
        {
            _factory = factory;
            _characterBackground = characterBackground;
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
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<RoleView>();
            _characterPresenter.SetCharacter(_character);
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => SetAppearance(character));
        }

        private void SetAppearance(Character character)
        {
            _characterBackground.ClearList();
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<LookView>();
            _characterPresenter.SetCharacter(_character);
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => SetTargets(character));
        }

        private void SetTargets(Character character)
        {
            _characterBackground.ClearList();
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<TargetView>();
            _characterPresenter.SetCharacter(_character);
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => SetConnections(character));
        }

        private void SetConnections(Character character)
        {
            _characterBackground.ClearList();
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<ConnectionView>();
            _characterPresenter.SetCharacter(_character);
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => Upgrade(character));
        }

        private void Upgrade(Character character)
        {

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

