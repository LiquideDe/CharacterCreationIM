using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

namespace CharacterCreation.Background
{
    public class FactionPresenter : ICharacterPresenter
    {
        private readonly Subject<Character> _nextClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        private readonly AudioManager _audioManager;
        private readonly FactionView _view;
        private readonly List<IDisposable> _subscriptions = new();
        [Inject] private FactionCreator _factionCreator = null;
        [Inject] private SkillCreator _skillCreator;
        [Inject] private FinderData _finderData;
        [Inject] private EquipmentParser _equipmentParser;
        private FactionData _currentFaction;
        private TemplateFaction _currentTemplate;
        private int _currentFactionIndex = -1;
        private int _currentTemplateIndex = -1;
        private Character _character;
        private bool _isFactionShow = true;

        public FactionPresenter(AudioManager audioManager, FactionView factionView)
        {
            _audioManager = audioManager;
            _view = factionView;
        }

        public void Initialize()
        {
            _subscriptions.Add(
                _view.OnNextButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    SetFactionAndGoNext();
                })
            );

            _subscriptions.Add(
                _view.OnNextItemButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    NextFaction();
                })
            );

            _subscriptions.Add(
                _view.OnPrevItemButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    PrevFaction();
                })
            );

            _subscriptions.Add(
                _view.OnRandomButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    RandomFaction();
                })
            );

            _subscriptions.Add(
                _view.OnChooseTemplateButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    SwitchTemplate();
                })
                );

            NextFaction();
        }

        private void SwitchTemplate()
        {
            _isFactionShow = !_isFactionShow;
            NextFaction(-1);
        }

        private void RandomFaction()
        {
            var rand = new System.Random();
            _currentFaction = _factionCreator.GetFactionForRoll(_character.Origin.Value, rand.Next(1, 101));
            ShowFaction(false);
        }

        private void PrevFaction()
        {
            if (_isFactionShow)
            {
                var factions = _factionCreator.Factions;
                if (factions == null || factions.Count == 0)
                    return;

                // Переход к предыдущему индексу по кругу
                _currentFactionIndex = (_currentFactionIndex - 1 + factions.Count) % factions.Count;
                _currentFaction = factions[_currentFactionIndex];

                ShowFaction();
            }
            else
            {
                var templates = _currentFaction.templates;
                if (templates == null || templates.Count == 0)
                    return;
                // Переход к предыдущему индексу по кругу
                _currentTemplateIndex = (_currentTemplateIndex - 1 + templates.Count) % templates.Count;
                _currentTemplate = templates[_currentTemplateIndex];
                ShowTemplate();
            }

        }

        private void NextFaction(int delta = 0)
        {
            if (_isFactionShow)
            {
                var faction = _factionCreator.Factions;
                if (faction == null || faction.Count == 0)
                    return;

                // Переход к следующему индексу по кругу
                _currentFactionIndex = (_currentFactionIndex + 1 + delta) % faction.Count;
                _currentFaction = faction[_currentFactionIndex];

                ShowFaction();
            }
            else
            {
                var templates = _currentFaction.templates;
                if (templates == null || templates.Count == 0)
                    return;
                // Переход к следующему индексу по кругу
                _currentTemplateIndex = (_currentTemplateIndex + 1) % templates.Count;
                _currentTemplate = templates[_currentTemplateIndex];
                ShowTemplate();
            }

        }

        private void ShowFaction(bool canChangeFaction = true)
        {
            _view.SetSheet(_currentFaction.serviceName, _currentFaction.description, canChangeFaction);
            _view.SetText("Бонусы:");
            _view.SetGaranted(_currentFaction.fixed_bonus, "Характеристики:");
            _view.SetChoose(_currentFaction.selectable_bonuses, 1);
            _view.SetSkills(_currentFaction.skill_upgrades, 2);
            if(_currentFaction.talents.Count > 0)
                foreach (var item in _currentFaction.talents)                
                    if (string.Compare(item.type,"fixed",true) == 0)
                    {
                        _view.SetList(item.talents, "Таланты", 1);
                    }
                    else
                    {
                        _view.SetChooseGroup(item.choices, "Выберите один из следующих талантов", 1);
                    }

            if (_currentFaction.influence_bonus.amount != 0)
                _view.SetText($"Вы получаете +{_currentFaction.influence_bonus.amount} к {_currentFaction.influence_bonus.faction}");

            if (_currentFaction.gear.items.Count > 0)
            {
                _view.SetText($"Вы получаете следующую экипировку:");

                foreach (var item in _currentFaction.gear.items)
                    _view.SetText(item);

                if (_currentFaction.gear.money > 0)
                    _view.SetText($"Вы получаете {_currentFaction.gear.money} соляров");
            }

            if (_currentFaction.gear.choice != null && _currentFaction.gear.choice.Count > 0)            
                _view.SetList(_currentFaction.gear.choice, "Выберите следующую экипировку:", _currentFaction.gear.amount_choice);
            

            if (_currentFaction.implants_data != null)
            {
                _view.SetList(_currentFaction.implants_data.first_implants, "Выберите следующий имплант:", 1);
                _view.SetList(_currentFaction.implants_data.second_implants, "Выберите следующий имплант:", 1);
            }
        }

        private void ShowTemplate()
        {
            _view.SetSheet(_currentTemplate.templateName, _currentTemplate.description, true);
            _view.SetText("Бонусы:");
            _view.SetGaranted(_currentTemplate.fixed_bonus, "Характеристики");
            _view.SetText("Навыки:");
            _view.SetGaranted(_currentTemplate.skill_upgrades.upgrades, "");

            if (_currentTemplate.talents != null && _currentTemplate.talents.Count > 0)
                _view.SetList(_currentTemplate.talents[0].talents, "Таланты:", 1);
            if (_currentTemplate.influence_bonus.amount != 0)
                _view.SetText($"Вы получаете +{_currentTemplate.influence_bonus.amount} к {_currentTemplate.influence_bonus.faction}");
            if (_currentTemplate.gear.items.Count > 0)
            {
                _view.SetText($"Вы получаете следующую экипировку:");

                foreach (var item in _currentTemplate.gear.items)
                    _view.SetText(item);

                if (_currentTemplate.gear.money > 0)
                    _view.SetText($"Вы получаете {_currentTemplate.gear.money} соляров");
            }

            if (_currentTemplate.implants != null)
            {
                _view.SetList(_currentTemplate.implants, "Импланты", 1);
            }
        }

        private void SetFactionAndGoNext()
        {  
            if (_isFactionShow && _view.IsCountEmpty())
            {
                ApplyBonusesCharacterictis(_character, _currentFaction.fixed_bonus);
                var charactristics = _view.GetGarantedCharacteristics();
                foreach (var item in charactristics)
                    if (item is ChooseCharacteristicView choose)
                        if (choose.IsSelected)
                            ApplyBonusesCharacterictis(_character, new Dictionary<string, int> { { choose.Characteristic.Name, choose.Characteristic.Level } });
                var skills = _view.GetSkills();
                foreach (var skill in skills)                
                    if (skill.Level > 0)
                    {
                        var skillPrefab = _skillCreator.SkillByName(skill.NameSkill);
                        _character.Skills.Add(new SkillData()
                        {
                            name = skillPrefab.name,
                            characteristic = skillPrefab.characteristic
                        });
                    }

                foreach (var item in _currentFaction.gear.items)
                    _character.Equipments.Add(_equipmentParser.TryGetEquipment(item));

                _character.Money.Value += _currentFaction.gear.money;

                var choseList = _view.GetCanChosen();
                foreach (var item in choseList)
                {
                    if(item.IsSelected)
                        foreach (var name in item.Talents)
                        {
                            if (_finderData.TryGet(name, out TalentData talentData))
                                _character.Talents.Add(talentData);

                            else if(_equipmentParser.TryGetEquipment(name) is EquipmentData equipmentData)
                                _character.Equipments.Add(equipmentData);

                            else if(_finderData.TryGet(name, out AugmeticData augmeticData))
                                _character.Augmetics.Add(augmeticData);
                        }
                }
                _character.Influence.TryAdd(_currentFaction.influence_bonus.faction, _currentFaction.influence_bonus.amount);
            }
            else if(!_isFactionShow)
            {
                ApplyBonusesCharacterictis(_character, _currentTemplate.fixed_bonus);
                foreach (var item in _currentTemplate.skill_upgrades.upgrades)
                {
                    var skillPrefab = _skillCreator.SkillByName(item.Key);
                    _character.Skills.Add(new SkillData()
                    {
                        name = skillPrefab.name,
                        characteristic = skillPrefab.characteristic
                    });
                }
                if (_currentTemplate.talents != null && _currentTemplate.talents.Count > 0)
                    foreach (var name in _currentTemplate.talents[0].talents)
                        if (_finderData.TryGet(name, out TalentData talentData))
                            _character.Talents.Add(talentData);

                foreach (var item in _currentTemplate.gear.items)
                    _character.Equipments.Add(_equipmentParser.TryGetEquipment(item));

                _character.Money.Value += _currentTemplate.gear.money;
            }
            else
            {
                _audioManager.PlayError();
                return;
            }
            
            _character.Faction.Value = _currentFaction.serviceName;
            _view.HideAndDestroyToLeft();
            _nextClicked?.OnNext(_character);
        }

        public void SetCharacter(Character character)
        {
            _character = character;
        }

        public void Dispose()
        {
            _nextClicked.Dispose();
            foreach (var sub in _subscriptions)
                sub.Dispose();
            _subscriptions.Clear();
        }

        private void ApplyBonusesCharacterictis(Character character, Dictionary<string, int> bonuses)
        {
            if (bonuses == null || bonuses.Count == 0) return;

            foreach (var kvp in bonuses)
            {
                var name = kvp.Key;
                var delta = kvp.Value;

                var ch = character.Characteristics
                    .FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));

                if (ch == null)
                {
                    Debug.LogError($"Characteristic {name} == null");
                }

                ch.PlusLevel(delta);
            }
        }
    }
}

