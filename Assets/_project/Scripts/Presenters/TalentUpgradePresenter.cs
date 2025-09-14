using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class TalentUpgradePresenter : ICharacterPresenter
    {
        private readonly Subject<Character> _nextClicked = new();
        private readonly Subject<Character> _prevClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        public Observable<Character> PrevClicked => _prevClicked;
        private readonly AudioManager _audioManager;
        private TalentUpgradeView _view;
        private readonly CompositeDisposable _cd = new CompositeDisposable();
        private Character _character;
        [Inject] private TalentCreator _talentCreator;
        private List<TalentDecorator> _talents = new List<TalentDecorator>();
        private TalentDecorator _currentTalent;
        private bool _lastToggle;

        public TalentUpgradePresenter(AudioManager audioManager, TalentUpgradeView view)
        {
            _audioManager = audioManager;
            _view = view;
        }

        public void SetCharacter(Character character)
        {
            _character = character;
            try
            {
                SetTalents();
                Debug.Log("[TalentUpgradePresenter] SetTalents OK");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return;
            }

            try
            {
                SetListToView(true);
                Debug.Log("[TalentUpgradePresenter] SetListToView OK");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return;
            }

            try
            {
                _view.SetExperience(_character.Experience.Value.experiencePoints);
                Debug.Log("[TalentUpgradePresenter] SetExperience OK");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return;
            }
        }

        public void Initialize()
        {
            Debug.Log("[TalentUpgradePresenter] Initialize");
            _view.OnButtonBuyClick.Subscribe(_ => { BuyTalent(); }).AddTo(_cd);
            _view.OnButtonCancelClick.Subscribe(_ => { _audioManager.PlayClick(); CancelUpgrade(); }).AddTo(_cd);
            _view.OnButtonNextClick.Subscribe(_ => { Debug.Log("[TalentUpgradePresenter] Next button click"); _audioManager.PlayConfirm(); GoNext(); }).AddTo(_cd);
            _view.OnButtonPrevClick.Subscribe(_ => { _audioManager.PlayClick(); GoPrev(); }).AddTo(_cd);
            _view.ShowTalentClicked.Subscribe(name => { _audioManager.PlayClick(); ShowTalent(name); }).AddTo(_cd);
            _view.ToggleShowAvailableClicked.Subscribe(avalaible => 
            { 
                _audioManager.PlayClick(); 
                SetListToView(avalaible); 
                _lastToggle = avalaible; 
            }).AddTo(_cd);
        }

        private void GoNext()
        {
            Debug.Log($" GoNext emit. HasObservers? {_nextClicked}");
            _nextClicked.OnNext(_character);
            _view.HideAndDestroyToLeft();                        
        }

        private void GoPrev()
        {
            _prevClicked.OnNext(_character);
            _view.HideAndDestroyToRight();                       
        }

        private void BuyTalent()
        {
            var cmd = new UpgradeTalentCommand(_character, _currentTalent.TalentData,100);
            var ok = _character.CharacteristicHistory.Do(cmd);
            if (!ok)
                _audioManager.PlayError();
            else
            {
                _view.SetExperience(_character.Experience.Value.experiencePoints);
                _audioManager.PlayClick();
                SetTalents();
                SetListToView(_lastToggle);
            }
        }

        private void CancelUpgrade()
        {
            _character.CharacteristicHistory.Undo();
            _view.SetExperience(_character.Experience.Value.experiencePoints);
        }

        public void Dispose()
        {
            Debug.LogAssertion("Dispose");
            _cd.Dispose();
        }

        private void SetTalents()
        {
            _talents.Clear();
            foreach (var item in _talentCreator.Talents)
            {
                if (IsCharacterHasNotTalent(item.name))
                {
                    TalentDecorator decorator = new TalentDecorator();
                    if (item.isMultiple)
                        decorator.TalentData = item;
                    else
                        decorator.TalentData = new TalentData() { 
                            name = item.name,
                            description = item.description,
                            character_creation_only = item.character_creation_only,
                            isMultiple = item.isMultiple,
                            maxMultiple = item.maxMultiple,
                            requirements = item.requirements,
                            uniqeText = item.uniqeText,                            
                        };
                    decorator.IsAvailable = IsTalentRequireDone(item);
                    _talents.Add(decorator);
                }
            }
        }

        private bool IsCharacterHasNotTalent(string name)
        {
            TalentData data = null;
            foreach (var item in _character.Talents)
            {
                if(string.Compare(item.name, name, true) == 0)
                {
                    data = item; break;
                }
            }

            if (data == null) return true;

            if(data.isMultiple && data.maxMultiple > data.currentMultiple)
                return true;

            return false;
        }

        private bool IsTalentRequireDone(TalentData talent)
        {
            bool answer = true;
            if (talent.requirements == null)
                return true;

            foreach (var item in talent.requirements)            
                answer &= CheckRequirement(item);            

            return answer;
        }

        private bool CheckRequirement(TalentRequirement requirement)
        {
            if (requirement == null) return true;
            switch (requirement.type)
            {
                case "specialization_improvement":
                    return CheckSpecialization(requirement.specialization, requirement.amount);

                case "skill_improvement":
                    return CheckSkill(requirement.specialization, requirement.amount); 

                case "attribute_min":
                    return CheckCharacteristic(requirement.attribute, requirement.value);

                case "attribute_max":
                    return CheckCharacteristic(requirement.attribute, requirement.value);

                case "no_improvement":
                    return NoSkill(requirement.skill);

                case "requirement_talent":
                    return IsTalents(requirement.talents);

                case "exclusive_with":
                    return IsNoTalent(requirement.talents[0]);

                default:
                    throw new Exception($"Не нашли requirement.type {requirement.type}");
            }
        }

        private bool CheckSpecialization(string name, int level)
        {
            foreach (var item in _character.Specializations)            
                if(string.Compare(item.name, name, true) == 0)
                    if(item.level >= level)
                        return true;
            
            return false;
        }

        private bool CheckSkill(string name, int level)
        {
            foreach (var item in _character.Skills)
                if (string.Compare(item.name, name, true) == 0)
                    if (item.level >= level)
                        return true;

            return false;
        }

        private bool CheckCharacteristic(string name, int level)
        {
            foreach (var item in _character.Characteristics)
            
                if(string.Compare(name, item.Name, true) == 0)
                    if(item.Level >= level)
                        return true;
            
            return false;
        }

        private bool NoSkill(string name)
        {
            foreach (var item in _character.Skills)            
                if (string.Compare(name, item.name, true) == 0)
                    return false;
            
            return true;
        }

        private bool IsTalents(List<string> talents)
        {
            if (talents == null)
                return true;

            int count = 0;
            foreach (var talent in talents)            
                foreach (var item in _character.Talents)
                {
                    if (string.Compare(talent, item.name, true) == 0)
                    {
                        count++;
                        break;
                    }                        
                }
            
            if(count == talents.Count)
                return true;

            return false;
        }

        private bool IsNoTalent(string name)
        {
            foreach (var item in _character.Talents)           
                if(string.Compare(name, item.name, true) == 0)
                    return false;
            
            return true;
        }

        private void SetListToView(bool showOnlyAvailable)
        {
            List<string> strings = new List<string>();
            foreach (var item in _talents)  
                if(!showOnlyAvailable)
                    strings.Add(item.TalentData.name);
                else if(item.IsAvailable)
                    strings.Add(item.TalentData.name);

            _view.SetTalents(strings);
        }

        private void ShowTalent(string name)
        {
            foreach (var item in _talents)            
                if(string.Compare(name, item.TalentData.name, true) == 0)
                    _currentTalent = item;
            
            _view.ShowTalent(_currentTalent.TalentData, _currentTalent.IsAvailable);
        }
    }

    public class TalentDecorator
    {
        public TalentData TalentData { get; set; }
        public bool IsAvailable {  get; set; }
    }

    public sealed class UpgradeTalentCommand : IGameCommand
    {
        private readonly Character _character;
        private readonly TalentData _talent; 
        private readonly int _xpCost;
        private int _prevXP;
        private int _prevExpSpent;
        private bool _applied;

        public UpgradeTalentCommand(Character player, TalentData ch, int xpCost)
        {
            _character = player;
            _talent = ch;
            _xpCost = xpCost;
        }

        public bool Execute()
        {
            if (_character.Experience.Value.experiencePoints < _xpCost) return false;

            _prevXP = _character.Experience.Value.experiencePoints;
            _prevExpSpent = _character.Experience.Value.experienceSpent;
            _character.Experience.Value.experiencePoints -= _xpCost;
            _character.Experience.Value.experienceSpent += _xpCost;
            _character.Talents.Add(_talent);

            _applied = true;
            return true;
        }

        public void Undo()
        {
            if (!_applied) return;
            _character.Talents.Remove(_talent);
            _character.Experience.Value.experiencePoints = _prevXP;
            _character.Experience.Value.experienceSpent = _prevExpSpent;
            _applied = false;
        }
    }
}

