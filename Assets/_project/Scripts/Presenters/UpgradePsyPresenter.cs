using R3;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class UpgradePsyPresenter : ICharacterPresenter
    {
        private readonly Subject<Character> _nextClicked = new();
        private readonly Subject<Character> _prevClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        public Observable<Character> PrevClicked => _prevClicked;
        private readonly AudioManager _audioManager;
        private UpgradePsyView _view;
        private readonly CompositeDisposable _cd = new CompositeDisposable();
        private Character _character;
        [Inject] private PsycanaCreator _psyCreator;
        private PsyData _currentPsy;
        private int _currentPsyIdSchool;
        private List<string> _schools = new List<string>() { "Биомантия", "Прорицание", "Пиромантия", "Телекинез", "Телепатия" };
        private List<List<PsyData>> _psyDatas = new List<List<PsyData>>();

        public UpgradePsyPresenter(AudioManager audioManager, UpgradePsyView view)
        {
            _audioManager = audioManager;
            _view = view;
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public void SetCharacter(Character character)
        {
            _character = character;
            SetPsyPowers();
        }

        private void SetPsyPowers()
        {
            _psyDatas.Add(new List<PsyData>());
            _psyDatas.Add(new List<PsyData>());
            _psyDatas.Add(new List<PsyData>());
            _psyDatas.Add(new List<PsyData>());
            _psyDatas.Add(new List<PsyData>());
            SetPsyInList("Биомантия", _psyDatas[0]);
            SetPsyInList("Прорицание", _psyDatas[0]);
            SetPsyInList("Пиромантия", _psyDatas[0]);
            SetPsyInList("Телекинез", _psyDatas[0]);
            SetPsyInList("Телепатия", _psyDatas[0]);
        }

        private void SetPsyInList(string school, List<PsyData> psyDatas)
        {
            var list = _psyCreator.PsyPowers.Where(psy => string.Compare(psy.specialization, school, true) == 0);
            foreach (var psyData in list)
            {
                if (IsCharacterHasNotPsyPower(psyData.name))
                {
                    psyDatas.Add(new PsyData()
                    {
                        description = psyData.description,
                        name = psyData.name,
                        warpCharge = psyData.warpCharge,
                        duration = psyData.duration,
                        isLesser = psyData.isLesser,
                        isObvious = psyData.isObvious,
                        range = psyData.range,
                        specialization = psyData.specialization,
                        target = psyData.target,
                        testDifficulty = psyData.testDifficulty,
                    });
                }
            }
        }

        private bool IsCharacterHasNotPsyPower(string name)
        {
            foreach (var item in _character.PsyPowers)            
                if(string.Compare(name, item.name, true) == 0)
                    return false;
            
            return true;
        }
    }
}

