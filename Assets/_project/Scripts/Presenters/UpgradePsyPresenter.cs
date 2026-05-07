using R3;
using System;
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
        private bool _isFreeEdit;
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
            _cd.Dispose();
        }

        public void Initialize()
        {
            _view.OnButtonBuyClick.Subscribe(_ => { BuyPsyPower(); }).AddTo(_cd);
            _view.OnButtonCancelClick.Subscribe(_ => { CancelBuy(); }).AddTo(_cd);
            _view.OnButtonNextClick.Subscribe(_ => { GoNext(); }).AddTo(_cd);
            _view.OnButtonPrevClick.Subscribe(_ => { GoPrev(); }).AddTo(_cd);
            _view.OnButtonNextSchoolClick.Subscribe(_ => { _audioManager.PlayClick(); NextSchool(); }).AddTo(_cd);
            _view.OnButtonPrevSchoolClick.Subscribe(_ => { _audioManager.PlayClick(); PrevSchool(); }).AddTo(_cd);
            _view.ShowPsyClicked.Subscribe(name => { _audioManager.PlayClick(); ShowPsy(name); }).AddTo(_cd);
        }

        public void SetFreeEdit(bool isFreeEdit)
        {
            _isFreeEdit = isFreeEdit;
        }

        private void GoNext()
        {
            _audioManager.PlayClick();
            _view.HideAndDestroyToLeft(() => _nextClicked.OnNext(_character));
        }

        private void GoPrev()
        {
            _audioManager.PlayClick();
            _prevClicked.OnNext(_character);
            _view.HideAndDestroyToRight();
        }

        private void BuyPsyPower()
        {
            if (_isFreeEdit)
            {
                var cmdFree = new FreeUpgradePsyPowerCommand(_character, _currentPsy);
                var okFree = _character.CharacteristicHistory.Do(cmdFree);
                if (!okFree)
                    _audioManager.PlayError();
                else
                {
                    _view.SetExperience(_character.Experience.Value.experiencePoints);
                    _audioManager.PlayClick();
                    _psyDatas[_currentPsyIdSchool].Remove(_currentPsy);
                    ConvertPsyToStringAndShow();
                }
                return;
            }

            int cost = 100;
            if (_currentPsy.isLesser)
                cost = 60;

            var cmd = new UpgradePsyPowerCommand(_character, _currentPsy, cost);
            var ok = _character.CharacteristicHistory.Do(cmd);
            if (!ok)
                _audioManager.PlayError();
            else
            {
                _view.SetExperience(_character.Experience.Value.experiencePoints);
                _audioManager.PlayClick();
                _psyDatas[_currentPsyIdSchool].Remove(_currentPsy);
                ConvertPsyToStringAndShow();
            }
        }

        private void CancelBuy()
        {
            _character.CharacteristicHistory.Undo();
            _view.SetExperience(_character.Experience.Value.experiencePoints);
        }

        private void ShowPsy(string name)
        {
            foreach (var item in _psyDatas[_currentPsyIdSchool])            
                if(string.Compare(item.name, name, true) == 0)
                {
                    _currentPsy = item;
                    _view.ShowPsy(item);
                    break;
                }                                
        }

        public void SetCharacter(Character character)
        {
            _character = character;
            if(IsCharacterHasNotPsyPower("Психический удар"))
            {
                var psyPower = _psyCreator.PsyPowerByName("Психический удар");
                _character.PsyPowers.Add(psyPower);
            }
            SetPsyPowers();
            _currentPsyIdSchool = 0;
            ConvertPsyToStringAndShow();
            _view.SetFree(character);
            _view.SetExperience(character.Experience.Value.experiencePoints);
            
        }

        private void SetPsyPowers()
        {
            _psyDatas.Clear();
            _psyDatas.Add(new List<PsyData>());
            _psyDatas.Add(new List<PsyData>());
            _psyDatas.Add(new List<PsyData>());
            _psyDatas.Add(new List<PsyData>());
            _psyDatas.Add(new List<PsyData>());
            SetPsyInList("Биомантия", _psyDatas[0]);
            SetPsyInList("Прорицание", _psyDatas[1]);
            SetPsyInList("Пиромантия", _psyDatas[2]);
            SetPsyInList("Телекинез", _psyDatas[3]);
            SetPsyInList("Телепатия", _psyDatas[4]);
        }

        private void SetPsyInList(string school, List<PsyData> psyDatas)
        {
            var list = _psyCreator.PsyPowers.Where(psy => string.Compare(psy.specialization, school, true) == 0);
            foreach (var psyData in list)
            {
                if (IsCharacterHasNotPsyPower(psyData.name) && (_isFreeEdit || IsCharacterHasSpecialization(school) || psyData.isLesser))
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
                if (string.Compare(name, item.name, true) == 0)
                    return false;

            return true;
        }
        
        private bool IsCharacterHasSpecialization(string name)
        {
            return _character.HasPsyDisciplineAccess(name);
        }

        private void NextSchool(int delta = 0)
        {
            
            _currentPsyIdSchool = (_currentPsyIdSchool + 1 + delta) % _psyDatas.Count;
            ConvertPsyToStringAndShow();
        }

        private void PrevSchool(int delta = 0)
        {

            _currentPsyIdSchool = (_currentPsyIdSchool - 1 + _psyDatas.Count) % _psyDatas.Count;
            ConvertPsyToStringAndShow();
        }

        private void ConvertPsyToStringAndShow()
        {
            var list = new List<string>();
            foreach (var item in _psyDatas[_currentPsyIdSchool])
                list.Add(item.name);

            _view.SetPsyPowers(list, _schools[_currentPsyIdSchool]);
        }
    }

    public sealed class UpgradePsyPowerCommand : IGameCommand
    {
        private readonly Character _character;
        private readonly PsyData _psy;
        private readonly int _xpCost;
        private int _prevXP;
        private int _prevExpSpent;
        private int _prevFreeSmallPoints;
        private int _prevFreePoints;
        private bool _applied;

        public UpgradePsyPowerCommand(Character player, PsyData ch, int xpCost)
        {
            _character = player;
            _psy = ch;
            _xpCost = xpCost;
        }

        public bool Execute()
        {
            bool enoughExp = _character.Experience.Value.experiencePoints >= _xpCost;
            bool freeLesser = false;
            bool freeBig = false;
            if(_psy.isLesser && _character.FreeSmallPsyPower.Value > 0)
                freeLesser = true;

            if(!_psy.isLesser && _character.FreePsyPower.Value > 0)
                freeBig = true;

            if (!enoughExp && !freeLesser && !freeBig) return false;

            if (freeLesser)
            {
                _prevXP = _character.Experience.Value.experiencePoints;
                _prevExpSpent = _character.Experience.Value.experienceSpent;
                _prevFreeSmallPoints = _character.FreeSmallPsyPower.Value;
                _prevFreePoints = _character.FreePsyPower.Value;
                _character.FreeSmallPsyPower.Value -= 1;
                _character.PsyPowers.Add(_psy);
            }
            else if (freeBig)
            {
                _prevXP = _character.Experience.Value.experiencePoints;
                _prevExpSpent = _character.Experience.Value.experienceSpent;
                _prevFreePoints = _character.FreePsyPower.Value;
                _prevFreeSmallPoints = _character.FreeSmallPsyPower.Value;
                _character.FreePsyPower.Value -= 1;
                _character.PsyPowers.Add(_psy);
            }
            else
            {
                _prevXP = _character.Experience.Value.experiencePoints;
                _prevExpSpent = _character.Experience.Value.experienceSpent;
                _prevFreePoints = _character.FreePsyPower.Value;
                _prevFreeSmallPoints = _character.FreeSmallPsyPower.Value;
                _character.Experience.Value.experiencePoints -= _xpCost;
                _character.Experience.Value.experienceSpent += _xpCost;
                _character.PsyPowers.Add(_psy);
            }                

            _applied = true;
            return true;
        }

        public void Undo()
        {
            if (!_applied) return;
            _character.PsyPowers.Remove(_psy);
            _character.Experience.Value.experiencePoints = _prevXP;
            _character.Experience.Value.experienceSpent = _prevExpSpent;
            _character.FreePsyPower.Value = _prevFreePoints;
            _character.FreeSmallPsyPower.Value = _prevFreeSmallPoints;
            _applied = false;
        }
    }

    public sealed class FreeUpgradePsyPowerCommand : IGameCommand
    {
        private readonly Character _character;
        private readonly PsyData _psy;
        private bool _applied;

        public FreeUpgradePsyPowerCommand(Character player, PsyData psy)
        {
            _character = player;
            _psy = psy;
        }

        public bool Execute()
        {
            _character.PsyPowers.Add(_psy);
            _applied = true;
            return true;
        }

        public void Undo()
        {
            if (!_applied) return;
            _character.PsyPowers.Remove(_psy);
            _applied = false;
        }
    }
}

