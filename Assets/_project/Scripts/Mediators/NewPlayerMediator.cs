using System;
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
            _nextClickedSubscription?.Dispose();            
            _characterPresenter = null;

        }
    }
}

