using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using R3;
using System.IO;

namespace CharacterCreation
{
    public class UpgradeMediator
    {
        private PresenterViewFactory _factory;
        private ICharacterPresenter _characterPresenter;
        private Character _character;
        private PrintCharacterPresenter _printCharacter;
        private IFactory<ExitMenu> _exitMenuFactory;
        private readonly UpgradeItemsListPresenter _upgradeItemsListPresenter;
        private CompositeDisposable _cd = new CompositeDisposable();
        private Subject<Unit> _characterUpgraded = new Subject<Unit>();
        public Observable<Unit> CharacterUpgraded => _characterUpgraded;

        public UpgradeMediator(PresenterViewFactory factory, PrintCharacterPresenter printCharacter,
            UpgradeItemsListPresenter upgradeItemsListPresenter, IFactory<ExitMenu> exitMenuFactory)
        {
            _factory = factory;
            _printCharacter = printCharacter;
            _upgradeItemsListPresenter = upgradeItemsListPresenter;
            _exitMenuFactory = exitMenuFactory;
        }

        public void ShowLoads()
        {
            LoadCharacterPresenter presenter = (LoadCharacterPresenter)_factory.Create<LoadCharacterView>();
            presenter.LoadedCharacter.Subscribe(character => { ShowAddExperience(character); }).AddTo(_cd);
            presenter.ReturnToMenu.Subscribe(_ => _characterUpgraded.OnNext(Unit.Default)).AddTo(_cd);
            presenter.ShowSaves();
        }

        private void ShowAddExperience(Character character)
        {
            AddExperiencePresenter presenter = (AddExperiencePresenter)_factory.Create<AddExperienceView>();
            presenter.NextClicked.Take(1).Subscribe(UpgradeCharacteristics);
            presenter.SetCharacter(character);
        }

        private void UpgradeCharacteristics(Character character)
        {
            _upgradeItemsListPresenter.BeginUpgradeSession();
            _cd?.Clear();
            _character = character;
            _characterPresenter = (ICharacterPresenter)_factory.Create<CharacteristicUpgradeView>();
            _characterPresenter.SetCharacter(_character);
            _characterPresenter.NextClicked.Subscribe(character => UpgradeSkills(character)).AddTo(_cd);
        }

        private void UpgradeSkills(Character character)
        {
            _cd?.Clear();
            _character = character;
            _characterPresenter = (ICharacterPresenter)_factory.Create<SkillUpgradeView>();
            _characterPresenter.SetCharacter(_character);
            _characterPresenter.NextClicked.Subscribe(character => UpgradeTalent(character)).AddTo(_cd);
            var pres = _characterPresenter as SkillUpgradePresenter;
            pres.PrevClicked.Subscribe(character => UpgradeCharacteristics(character)).AddTo(_cd);
        }

        private void UpgradeTalent(Character character)
        {
            _cd?.Clear();
            _character = character;
            _characterPresenter = (ICharacterPresenter)_factory.Create<TalentUpgradeView>();
            _characterPresenter.SetCharacter(_character);
            _characterPresenter.NextClicked.Subscribe(character => { UpgradePsyPowers(character); }).AddTo(_cd);
            var pres = _characterPresenter as TalentUpgradePresenter;
            pres.PrevClicked.Subscribe(character => UpgradeSkills(character)).AddTo(_cd);
        }

        private void UpgradePsyPowers(Character character)
        {
            _cd?.Clear();
            if (_character.IsPsyker)
            {
                _character = character;
                _characterPresenter = (ICharacterPresenter)_factory.Create<UpgradePsyView>();
                _characterPresenter.SetCharacter(_character);
                _characterPresenter.NextClicked.Subscribe(ShowExitMenu);
                var pres = _characterPresenter as UpgradePsyPresenter;
                pres.PrevClicked.Subscribe(character => UpgradeTalent(character)).AddTo(_cd);
            }
            else
            {
                ShowExitMenu(character);
            }
            
        }
        
        private void ShowExitMenu(Character character)
        {
            _upgradeItemsListPresenter.EndUpgradeSession();
            _cd?.Clear();
            _character = character;
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
            _characterUpgraded.OnNext(Unit.Default);
        }

    }
}

