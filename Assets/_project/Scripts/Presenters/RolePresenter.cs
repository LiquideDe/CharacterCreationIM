using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CharacterCreation.Background
{
    public class RolePresenter : ICharacterPresenter
    {
        private readonly Subject<Character> _nextClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        private readonly AudioManager _audioManager;
        private readonly RoleView _view;
        private readonly List<IDisposable> _subscriptions = new();
        [Inject] private SkillCreator _skillCreator;
        [Inject] private FinderData _finderData;
        [Inject] private EquipmentParser _equipmentParser;
        [Inject] private RoleCreator _roleCreator;
        private RoleData _currentRole;
        private int _currentRoleIndex = -1;
        private Character _character;

        public RolePresenter(AudioManager audioManager, RoleView view)
        {
            _view = view;
            _audioManager = audioManager;            
        }

        public void Dispose()
        {
            _nextClicked.Dispose();
            foreach (var sub in _subscriptions)
                sub.Dispose();
            _subscriptions.Clear();
        }

        public void Initialize()
        {
            _subscriptions.Add(
                _view.OnNextButtonClick.Subscribe(_ =>
                {
                    SetRoleAndGoNext();
                })
            );

            _subscriptions.Add(
                _view.OnNextItemButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    NextRole();
                })
            );

            _subscriptions.Add(
                _view.OnPrevItemButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    PrevRole();
                })
            );

            _subscriptions.Add(
                _view.OnRandomButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    RandomRole();
                })
            );
            NextRole();
        }

        public void SetCharacter(Character character)
        {
            _character = character;
        }

        private void RandomRole()
        {
            NextRole();
            _character.Experience.Value.experiencePoints += 50;
        }

        private void NextRole()
        {
            
            var roles = _roleCreator.Roles;
            if (roles == null || roles.Count == 0)
                return;

            _currentRoleIndex = (_currentRoleIndex - 1 + roles.Count) % roles.Count;
            _currentRole = roles[_currentRoleIndex];

            ShowRole();
        }

        private void PrevRole()
        {
            var roles = _roleCreator.Roles;
            if (roles == null || roles.Count == 0)
                return;

            _currentRoleIndex = (_currentRoleIndex + 1) % roles.Count;
            _currentRole = roles[_currentRoleIndex];

            ShowRole();
        }

        private void ShowRole()
        {
            bool isMystic = string.Compare(_currentRole.roleName, "Мистик", true) == 0;

            _view.SetSheet(_currentRole.roleName, _currentRole.description);
            _view.SetText("Бонусы:", false);
            if(isMystic)            
                _view.SetText("Вы получаете талант Псайкер, если ещё не имели его. Если у вас уже был талант Псайкер, вы получаете одну малую психосилу, а также одну психосилу из известной вам дисциплины", false);
            
            _view.SetList(_currentRole.talents, $"Выберите {_currentRole.amountTalents} таланта:", _currentRole.amountTalents);
            _view.SetSkills(_currentRole.skill_upgrades, 2);
            var listSpecializations = new List<string>();
            if(!isMystic)
            foreach (var item in _currentRole.specialization)            
                listSpecializations.AddRange(_skillCreator.GetSpecializations(item));
            else
            {
                foreach (var item in _currentRole.specialization)
                    listSpecializations.Add(item);
            }

            foreach (var item in _currentRole.equipments)
            {
                if (item.Count > 1)
                    _view.SetList(item, "Выберите следующую экипировку:", 1);
                else
                    _view.SetText(item[0], true);
            }

            _view.SetSpecializations(listSpecializations, _currentRole.specializationAmount, 1);
        }

        private void SetRoleAndGoNext()
        {
            if (_view.IsCountEmpty())
            {
                _audioManager.PlayConfirm();
                _character.Role.Value = _currentRole.roleName;
                if (string.Compare(_currentRole.roleName, "Мистик", true) == 0)
                {                    
                    if (_character.Talents.FirstOrDefault(t =>
                string.Equals(t.name, "Псайкер", StringComparison.OrdinalIgnoreCase)) != null)
                    {
                        _character.FreePsyPower.Value = 1;
                        _character.FreeSmallPsyPower.Value = 1;
                    }
                    else
                    {
                        _character.Talents.Add(new TalentData() { name = "Псайкер" });
                    }
                }
                                   
                var skills = _view.GetSkills();
                foreach (var skill in skills)
                    if (skill.Level > 0)
                    {
                        SkillData skillPrefab = null;
                        bool isCharacterSkill = false;
                        foreach (var item in _character.Skills)                        
                            if(string.Compare(item.name, skill.NameSkill) == 0)
                            {
                                skillPrefab = item;
                                isCharacterSkill = true;
                                skillPrefab.level += skill.Level;
                                break;
                            } 

                        if(skillPrefab == null)
                            skillPrefab = _skillCreator.SkillByName(skill.NameSkill);

                        if(skillPrefab != null && isCharacterSkill == false)
                        {
                            _character.Skills.Add(new SkillData()
                            {
                                name = skillPrefab.name,
                                characteristic = skillPrefab.characteristic,
                                level = skill.Level
                            });
                        }                            
                        else if(!isCharacterSkill)
                        {
                            var specializationPrefab = _skillCreator.SpecializationByName(skill.NameSkill);
                            if (specializationPrefab != null)
                            {
                                _character.Specializations.Add(new SpecializationData()
                                {
                                    name = specializationPrefab.name,
                                     skill = specializationPrefab.skill,
                                     level = skill.Level
                                });
                            }
                            else
                                Debug.LogAssertion($"Не найден навык или специализация с именем '{skill.NameSkill}'");
                        }
                    }

                var choseList = _view.GetCanChosen();
                foreach (var item in choseList)
                {
                    if (item.IsSelected)
                        foreach (var name in item.Talents)
                        {
                            if (_finderData.TryGet(name, out TalentData talentData))
                                _character.Talents.Add(talentData);

                            else if (_equipmentParser.TryGetEquipment(name) is EquipmentData equipmentData)
                                _character.Equipments.Add(equipmentData);

                        }
                }
            }
            else
            {
                _audioManager.PlayError();
                return;
            }

            _view.HideAndDestroyToLeft();
            _nextClicked?.OnNext(_character);
        }
    }
}

