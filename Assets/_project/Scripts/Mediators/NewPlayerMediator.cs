using System;
using System.IO;
using CharacterCreation.Background;
using R3;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class NewPlayerMediator
    {
        private PresenterViewFactory _factory;
        private ICharacterPresenter _characterPresenter;
        private IDisposable _nextClickedSubscription;
        private IDisposable _prevClicked;
        private Character _character;
        private PrintCharacterPresenter _printCharacter;
        private IFactory<ExitMenu> _exitMenuFactory;
        private readonly UpgradeItemsListPresenter _upgradeItemsListPresenter;
        private Subject<Unit> _newPlayerDone = new Subject<Unit>();
        public Observable<Unit> NewPlayerIsDone => _newPlayerDone;

        public NewPlayerMediator(PresenterViewFactory factory, PrintCharacterPresenter printCharacter,
            UpgradeItemsListPresenter upgradeItemsListPresenter, IFactory<ExitMenu> exitMenuFactory)
        {
            _factory = factory;
            _printCharacter = printCharacter;
            _upgradeItemsListPresenter = upgradeItemsListPresenter;
            _exitMenuFactory = exitMenuFactory;
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
            _upgradeItemsListPresenter.BeginUpgradeSession();
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
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<TalentUpgradeView>();
            var pres = _characterPresenter as TalentUpgradePresenter;
            pres.SetCharacterCreation(true);
            _characterPresenter.SetCharacter(_character);
            if(character.IsPsyker)
                _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => { UpgradePsyPowers(character); }, ex => Debug.LogException(ex.Exception));
            else
                _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => { SetName(character); }, ex => Debug.LogException(ex.Exception));
            _prevClicked = pres.PrevClicked.Subscribe(character => UpgradeSkills(character));
        }

        private void UpgradePsyPowers(Character character)
        {
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<UpgradePsyView>();
            _characterPresenter.SetCharacter(_character);
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(character => SetName(character));
            var pres = _characterPresenter as UpgradePsyPresenter;
            _prevClicked = pres.PrevClicked.Subscribe(character => UpgradeTalent(character));
        }

        private void SetName(Character character)
        {
            _upgradeItemsListPresenter.EndUpgradeSession();
            _character = character;
            Reset();
            _characterPresenter = (ICharacterPresenter)_factory.Create<SetNameView>();
            _characterPresenter.SetCharacter(_character);
            _nextClickedSubscription = _characterPresenter.NextClicked.Subscribe(ShowExitMenu);
        }

        private void ShowExitMenu(Character character)
        {
            _character = character;
            Reset();
            var exitMenu = _exitMenuFactory.Create();
            exitMenu.SaveAndReturnToMenuClicked.Take(1).Subscribe(_ =>
            {
                exitMenu.Close();
                SaveAndPrint(returnToMenu: true);
            });
            exitMenu.ReturnToMenuClicked.Take(1).Subscribe(_ =>
            {
                exitMenu.Close();
                ReturnToMenu();
            });
            exitMenu.SaveAndExitClicked.Take(1).Subscribe(_ =>
            {
                exitMenu.Close();
                SaveAndPrint(returnToMenu: false);
            });
            exitMenu.ExitClicked.Take(1).Subscribe(_ =>
            {
                exitMenu.Close();
                ApplicationExit.OpenCharactersFolderAndQuit();
            });
        }

        private void SaveAndPrint(bool returnToMenu)
        {
            if (!Directory.Exists($"{Application.dataPath}/StreamingAssets/Персонажи/{_character.Name.Value}"))
                Directory.CreateDirectory($"{Application.dataPath}/StreamingAssets/Персонажи/{_character.Name.Value}");
            var path = Path.Combine(Application.streamingAssetsPath, $"Персонажи/{_character.Name.Value}/{_character.Name.Value}.json");
            CharacterStorage.SaveToFile(_character, path);
            _printCharacter.WorkIsFinished.Take(1).Subscribe(_ =>
            {
                if (returnToMenu)
                    ReturnToMenu();
                else
                    ApplicationExit.OpenCharactersFolderAndQuit();
            });
            _printCharacter.PrintCharacter(_character);
        }

        private void ReturnToMenu()
        {
            _upgradeItemsListPresenter.EndUpgradeSession();
            _newPlayerDone.OnNext(Unit.Default);
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

