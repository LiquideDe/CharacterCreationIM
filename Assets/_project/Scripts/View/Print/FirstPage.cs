using CharacterCreation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class FirstPage : TakeScreenshot
    {
        [SerializeField] private List<CharacteristicPrint> _characteristicBase = new List<CharacteristicPrint>();
        [SerializeField] private List<CharacteristicPrint> _characteristicUpgrades = new List<CharacteristicPrint>();
        [SerializeField] private List<CharacteristicPrint> _characteristicTotal = new List<CharacteristicPrint>();
        [SerializeField] private TextMeshProUGUI _origin;
        [SerializeField] private TextMeshProUGUI _faction;
        [SerializeField] private TextMeshProUGUI _role;
        [SerializeField] private TextMeshProUGUI _patron;
        [SerializeField] private TextMeshProUGUI _age;
        [SerializeField] private TextMeshProUGUI _eyes;
        [SerializeField] private TextMeshProUGUI _hair;
        [SerializeField] private TextMeshProUGUI _height;
        [SerializeField] private TextMeshProUGUI _weight;
        [SerializeField] private TextMeshProUGUI _hand;
        [SerializeField] private TextMeshProUGUI _nameCharacter;
        [SerializeField] private TextMeshProUGUI _marks;
        [SerializeField] private TextMeshProUGUI _exp;
        [SerializeField] private TextMeshProUGUI _expSpent;
        [SerializeField] private List<SkillInPrint> _skillUpgrades = new List<SkillInPrint>();
        [SerializeField] private List<SkillInPrint> _skillTotal = new List<SkillInPrint>();
        [SerializeField] private List<SpecializationPrint> _specializationPrints = new List<SpecializationPrint>();
        [SerializeField] private TextMeshProUGUI _goals;
        [SerializeField] private TextMeshProUGUI _connections;
        [SerializeField] private TextMeshProUGUI _prophecy;
        [SerializeField] private TextMeshProUGUI _corruption;
        [SerializeField] private TextMeshProUGUI _fatepoints;
        [SerializeField] private TextMeshProUGUI _mutations;
        [Inject] private SkillCreator _skillCreator;

        private Dictionary<string, CharacteristicPrint> _characteristicBaseDictionary = new Dictionary<string, CharacteristicPrint>();
        private Dictionary<string, CharacteristicPrint> _characteristicUpgradesDictionary = new Dictionary<string, CharacteristicPrint>();
        private Dictionary<string, CharacteristicPrint> _characteristicTotalDictionary = new Dictionary<string, CharacteristicPrint>();
        private Dictionary<string, SkillInPrint> _skillUpgradesDictionary = new Dictionary<string, SkillInPrint>();
        private Dictionary<string, SkillInPrint> _skillTotalDictionary = new Dictionary<string, SkillInPrint>();
        private List<string> _namesCharacteristis = new List<string>()
        {
            "Ближний бой",
            "Дальний бой",
            "Сила",
            "Выносливость",
            "Ловкость",
            "Интеллект",
            "Восприятие",
            "Сила воли",
            "Товарищество",
        };

        private Dictionary<string, Characteristicname> _characteristicNameToEnum = new Dictionary<string, Characteristicname>()
        {
            { "Ближний бой",  Characteristicname.WS },
        { "Дальний бой",  Characteristicname.BS },
        { "Сила",         Characteristicname.Str },
        { "Выносливость", Characteristicname.Tou },
        { "Ловкость",     Characteristicname.Ag },
        { "Интеллект",    Characteristicname.Int },
        { "Восприятие",   Characteristicname.Perc },
        { "Сила воли",    Characteristicname.WP },
        { "Товарищество", Characteristicname.Fel },
        };

        private Dictionary<string, SkillName> _skillNameToEnum = new Dictionary<string, SkillName>()
        {
            {"Атлетика", SkillName.Atletica },
            {"Бдительность", SkillName.Bditelnost },
            {"Ловкость рук", SkillName.LovkostRuk },
            {"Дисциплина", SkillName.Disciplin },
            {"Стойкость", SkillName.Stokost },
            {"Чутье", SkillName.Chut },
            {"Языки", SkillName.Yasiki },
            {"Логика", SkillName.Logic },
            {"Знания", SkillName.Znania },
            {"Медика", SkillName.Medica },
            {"Бой", SkillName.Boy },
            {"Ориентирование", SkillName.Orientirovanie },
            {"Пилотирование", SkillName.Pilotirov },
            {"Командование", SkillName.Command },
            {"Психическое мастерство", SkillName.PsyMaster },
            {"Стрельба", SkillName.Strelba },
            {"Взаимопонимание", SkillName.Vzaimootn },
            {"Рефлексы", SkillName.Refleks },
            {"Скрытность", SkillName.Skritnost },
            {"Техника", SkillName.Tech }
        };

        private Dictionary<string, Characteristic> _nameToCharacterCharacteristicDict = new Dictionary<string, Characteristic>();
        private Dictionary<string, SkillData> _nameToCharacterSkillDict = new Dictionary<string, SkillData>();

        public void SetCharacter(Character character)
        {
            _character = character;
            FillDictionaries();
            StartScreenshot(PageName.First.ToString());
        }

        private void FillDictionaries()
        {
            FillCharacteristics(_characteristicBase, _characteristicBaseDictionary);
            FillCharacteristics(_characteristicUpgrades, _characteristicUpgradesDictionary);
            FillCharacteristics(_characteristicTotal, _characteristicTotalDictionary);
            FillSkills(_skillUpgrades, _skillUpgradesDictionary);
            FillSkills(_skillTotal, _skillTotalDictionary);
            FillCharacterDicts();
            SetBackgrounds();
            SetCharacteristics();
            SetSkills();
            SetSpecializations();
        }

        private void FillCharacteristics(List<CharacteristicPrint> prints,Dictionary<string, CharacteristicPrint> dict)
        {
            foreach (var item in _namesCharacteristis)
            {
                var pan = prints.Where(ch => ch.Characteristicname == _characteristicNameToEnum[item]).First();
                dict.Add(item, pan);
            }
        }

        private void FillSkills(List<SkillInPrint> prints, Dictionary<string, SkillInPrint> dict)
        {
            foreach (var item in _skillCreator.Skills)
            {
                var pan = prints.Where(sk => sk.SkillName == _skillNameToEnum[item.name]).First();
                dict.Add(item.name, pan);
            }
        }

        private void FillCharacterDicts()
        {
            foreach (var item in _character.Characteristics)            
                _nameToCharacterCharacteristicDict.Add(item.Name,item);

            foreach (var item in _character.Skills)            
                _nameToCharacterSkillDict.Add(item.name,item);
            
        }

        private void SetBackgrounds()
        {
            _origin.text = _character.Origin.Value;
            _faction.text = _character.Faction.Value;
            _role.text = _character.Role.Value;
            _age.text = _character.Age.Value.ToString();
            _eyes.text = _character.Eyes.Value;
            _hair.text = $"{_character.HairColor.Value} {_character.HairStyle.Value}";
            _height.text = _character.Height.Value.ToString();
            _weight.text = _character.Weight.Value.ToString();
            _hand.text = _character.Hand.Value;
            _nameCharacter.text = _character.Name.Value;
            _marks.text = _character.Omen.Value;
            _exp.text = _character.Experience.Value.experiencePoints.ToString();
            _expSpent.text = _character.Experience.Value.experienceSpent.ToString();

            _goals.text = $"Краткосрочные: {_character.ShortTarget.Value} \nДолгосрочные: {_character.LongTarget.Value}";
            _prophecy.text = _character.Prophecy.Value;
            _corruption.text = _character.CorruptionPoints.Value.ToString();
            _fatepoints.text = _character.FatePoints.Value.ToString();
            foreach (var item in _character.Mutations)
            {
                _mutations.text += $"{item.name}\n";
            }            
        }

        private void SetCharacteristics()
        {
            foreach (var item in _namesCharacteristis)
            {
                var characteristic = _nameToCharacterCharacteristicDict[item];
                _characteristicBaseDictionary[item].Set(characteristic.BaseLevel.ToString());
                _characteristicUpgradesDictionary[item].Set((characteristic.Level - characteristic.BaseLevel).ToString());
                _characteristicBaseDictionary[item].Set(characteristic.Level.ToString());
            }                 
        }

        private void SetSkills()
        {
            foreach (var item in _skillCreator.Skills)
            {
                int characteristic = _nameToCharacterCharacteristicDict[item.characteristic].Level;
                SkillData skillData = null;
                if(_nameToCharacterSkillDict.TryGetValue(item.name, out skillData))
                    _skillUpgradesDictionary[item.name].SetText(skillData.level);

                if (skillData != null)
                    _skillTotalDictionary[item.name].SetText(skillData.level * 5 + characteristic);
                else
                    _skillTotalDictionary[item.name].SetText(characteristic);
            }
        }

        private void SetSpecializations()
        {
            foreach (var item in _character.Specializations)
            {
                var freePanel = _specializationPrints.Where(panel => panel.IsEmpty).First();
                int count = item.level * 5;
                if(freePanel != null)
                {
                    SkillData skill = null;
                    _nameToCharacterSkillDict.TryGetValue(item.skill, out skill);
                    if (skill != null)                    
                        count += skill.level * 5;  
                    else                    
                        skill = _skillCreator.SkillByName(item.skill);
                    
                    count += _nameToCharacterCharacteristicDict[skill.characteristic].Level;

                    freePanel.SetSpec(item, count);
                }
            }
        }
    }
}

