using System.IO;
using R3;
using UnityEngine;

namespace CharacterCreation
{
    public class EditCharacterMediator
    {
        private readonly PresenterViewFactory _factory;
        private readonly PrintCharacterPresenter _printCharacter;
        private ICharacterPresenter _characterPresenter;
        private Character _character;
        private readonly CompositeDisposable _cd = new CompositeDisposable();
        private readonly Subject<Unit> _characterEdited = new Subject<Unit>();
        public Observable<Unit> CharacterEdited => _characterEdited;

        public EditCharacterMediator(PresenterViewFactory factory, PrintCharacterPresenter printCharacter)
        {
            _factory = factory;
            _printCharacter = printCharacter;
        }

        public void ShowLoads()
        {
            var presenter = (LoadCharacterPresenter)_factory.Create<LoadCharacterView>();
            presenter.LoadedCharacter.Subscribe(character => { ShowEditBase(character); }).AddTo(_cd);
            presenter.ReturnToMenu.Subscribe(_ => _characterEdited.OnNext(Unit.Default)).AddTo(_cd);
            presenter.ShowSaves();
        }

        private void ShowEditBase(Character character)
        {
            _cd?.Clear();
            _character = character;
            _characterPresenter = (ICharacterPresenter)_factory.Create<EditCharacterView>();
            _characterPresenter.SetCharacter(_character);
            _characterPresenter.NextClicked.Subscribe(c => ShowEditCharacteristics(c)).AddTo(_cd);
        }

        private void ShowEditCharacteristics(Character character)
        {
            _cd?.Clear();
            _character = character;
            _characterPresenter = (ICharacterPresenter)_factory.Create<EditCharacteristicsView>();
            _characterPresenter.SetCharacter(_character);
            _characterPresenter.NextClicked.Subscribe(c => ShowEditSkills(c)).AddTo(_cd);
            var pres = _characterPresenter as EditCharacteristicsPresenter;
            pres.PrevClicked.Subscribe(c => ShowEditBase(c)).AddTo(_cd);
        }

        private void ShowEditSkills(Character character)
        {
            _cd?.Clear();
            _character = character;
            _characterPresenter = (ICharacterPresenter)_factory.Create<SkillUpgradeView>();
            var pres = _characterPresenter as SkillUpgradePresenter;
            pres.SetFreeEdit(true);
            _characterPresenter.SetCharacter(_character);
            _characterPresenter.NextClicked.Subscribe(c => ShowEditTalents(c)).AddTo(_cd);
            pres.PrevClicked.Subscribe(c => ShowEditCharacteristics(c)).AddTo(_cd);
        }

        private void ShowEditTalents(Character character)
        {
            _cd?.Clear();
            _character = character;
            _characterPresenter = (ICharacterPresenter)_factory.Create<TalentUpgradeView>();
            var pres = _characterPresenter as TalentUpgradePresenter;
            pres.SetFreeEdit(true);
            _characterPresenter.SetCharacter(_character);
            _characterPresenter.NextClicked.Subscribe(c => ShowEditPsyPowers(c)).AddTo(_cd);
            pres.PrevClicked.Subscribe(c => ShowEditSkills(c)).AddTo(_cd);
        }

        private void ShowEditPsyPowers(Character character)
        {
            _cd?.Clear();
            _character = character;
            _characterPresenter = (ICharacterPresenter)_factory.Create<UpgradePsyView>();
            var pres = _characterPresenter as UpgradePsyPresenter;
            pres.SetFreeEdit(true);
            _characterPresenter.SetCharacter(_character);
            _characterPresenter.NextClicked.Subscribe(c => Print(c)).AddTo(_cd);
            pres.PrevClicked.Subscribe(c => ShowEditTalents(c)).AddTo(_cd);
        }

        private void Print(Character character)
        {
            _cd?.Clear();
            _character = character;
            if (!Directory.Exists($"{Application.dataPath}/StreamingAssets/Персонажи/{_character.Name.Value}"))
                Directory.CreateDirectory($"{Application.dataPath}/StreamingAssets/Персонажи/{_character.Name.Value}");
            var path = Path.Combine(Application.streamingAssetsPath, $"Персонажи/{character.Name.Value}/{character.Name.Value}.json");
            CharacterStorage.SaveToFile(_character, path);
            _printCharacter.WorkIsFinished.Take(1).Subscribe(_ => _characterEdited.OnNext(Unit.Default));
            _printCharacter.PrintCharacter(character);
        }
    }
}
