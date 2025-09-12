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
        private IDisposable _prevClicked;
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
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<RoleView>();
            _characterPresenter.SetCharacter(_character);
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => SetAppearance(character));
        }

        private void SetAppearance(Character character)
        {
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<LookView>();
            _characterPresenter.SetCharacter(_character);
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => SetTargets(character));
        }

        private void SetTargets(Character character)
        {
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<TargetView>();
            _characterPresenter.SetCharacter(_character);
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => SetConnections(character));
        }

        private void SetConnections(Character character)
        {
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<ConnectionView>();
            _characterPresenter.SetCharacter(_character);
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => TenQuestions(character));
        }

        private void TenQuestions(Character character)
        {
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<QuestionsView>();
            _characterPresenter.SetCharacter(_character);
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => UpgradeCharacteristic(character));
        }

        private void UpgradeCharacteristic(Character character)
        {
            Debug.LogAssertion($"UpgradeCharacteristic");
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<CharacteristicUpgradeView>();
            _characterPresenter.SetCharacter(_character);
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => UpgradeSkills(character));
        }

        private void UpgradeSkills(Character character)
        {
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<SkillUpgradeView>();
            _characterPresenter.SetCharacter(_character);
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => UpgradeTalent(character));
            var pres = _characterPresenter as SkillUpgradePresenter;
            _prevClicked = pres.PrevClicked.Subscribe(character => UpgradeCharacteristic(character));
        }

        private void UpgradeTalent(Character character)
        {

        }

        private void Reset()
        {
            _nextClickedSubscription?.Dispose();
            _nextClickedSubscription = null;
            _characterPresenter?.Dispose();
            _characterPresenter = null;
            _prevClicked?.Dispose();
            _prevClicked = null;
        }
    }
}

