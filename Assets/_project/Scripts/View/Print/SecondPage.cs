using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class SecondPage : TakeScreenshot
    {
        [SerializeField] private TextMeshProUGUI _textInitiative;
        [SerializeField] private TextMeshProUGUI _textWounds;
        [SerializeField] private TextMeshProUGUI _textWs;
        [SerializeField] private TextMeshProUGUI _textBs;
        [SerializeField] private TextMeshProUGUI _textReflex;
        [SerializeField] private TextMeshProUGUI _textWeight;
        [SerializeField] private TextMeshProUGUI _textWeightMax;
        [SerializeField] private TextMeshProUGUI _textWarpCharge;
        [SerializeField] private List<WeaponPrint> _weaponPrints;
        [SerializeField] private List<ArmorPrint> _armorPrints;
        [SerializeField] private ZonesPrint _zonesPrints;
        [SerializeField] private List<TextMeshProUGUI> _equipments;
        [SerializeField] private List<PsyPowerPrint> _psyPowerPrints;
        [Inject] private SkillCreator _skillCreator;

        private Dictionary<string, Characteristic> _nameToCharacterCharacteristicDict = new Dictionary<string, Characteristic>();
        private Dictionary<string, SkillData> _nameToCharacterSkillDict = new Dictionary<string, SkillData>();
        private Dictionary<string, SpecializationData> _nameToCharacterSpecDict = new Dictionary<string, SpecializationData>();
        
        public void SetCharacter(Character character)
        {
            _character = character;
            FillDictionaries();
            FillSimple();
            FillWeapons();
            FillArmors();
            FillEquipments();
            if(_character.IsPsyker)
                FillPsyPowers();
            StartScreenshot(PageName.Second.ToString());
        }

        private void FillDictionaries()
        {
            foreach (var item in _character.Characteristics)
                _nameToCharacterCharacteristicDict.Add(item.Name, item);

            foreach (var item in _character.Skills)
                _nameToCharacterSkillDict.Add(item.name, item);

            foreach (var item in _character.Specializations)
                _nameToCharacterSpecDict.Add(item.name, item);
        }

        private void FillSimple()
        {
            int initiative = _nameToCharacterCharacteristicDict["Ловкость"].Level/10 + _nameToCharacterCharacteristicDict["Выносливость"].Level/10;
            _textInitiative.text = initiative.ToString();

            int wounds = 0;
            wounds += _nameToCharacterCharacteristicDict["Сила"].Level / 10;
            wounds += _nameToCharacterCharacteristicDict["Сила воли"].Level / 10;
            wounds += _nameToCharacterCharacteristicDict["Выносливость"].Level / 10 * 2;
            _textWounds.text = wounds.ToString();

            _textWs.text = _nameToCharacterCharacteristicDict["Ближний бой"].Level.ToString();
            _textBs.text = _nameToCharacterCharacteristicDict["Дальний бой"].Level.ToString();
            if (_nameToCharacterSkillDict.TryGetValue("Рефлексы", out SkillData skillData))
            {
                _textReflex.text = (_nameToCharacterCharacteristicDict["Ловкость"].Level + (skillData.level * 5)).ToString();
            }
            else
            {
                _textReflex.text = _nameToCharacterCharacteristicDict["Ловкость"].Level.ToString();
            }

            int bonusWeight = 0;
            _textWeight.text = CalculateWeight(out bonusWeight).ToString();
            _textWeightMax.text = (_nameToCharacterCharacteristicDict["Сила"].Level/10 + _nameToCharacterCharacteristicDict["Выносливость"].Level/10 + bonusWeight).ToString() ;
            

            if (_character.IsPsyker)
            {
                if(_character.Talents.Any(t => string.Compare(t.name, "Санкционированный псайкер", true) == 0))
                {
                    _textWarpCharge.text = (_nameToCharacterCharacteristicDict["Сила воли"].Level / 10 * 2).ToString();
                }
                else
                    _textWarpCharge.text = (_nameToCharacterCharacteristicDict["Сила воли"].Level / 10).ToString();
            }
        }

        private int CalculateWeight(out int weight)
        {
            int countZero = 0;
            int countTotalWeight = 0;
            weight = 0;
            foreach (var item in _character.Equipments)
            {
                if(item == null)
                    continue;

                if(item.weight == 0)
                    countZero++;

                if(countZero >= 10)
                {
                    countTotalWeight++;
                    countZero = 0;
                }

                countTotalWeight += item.weight;
                if(item.maxWeight > 0)
                    weight = item.maxWeight;
            }

            return countTotalWeight;
        }

        private void FillWeapons()
        {
            var list = _character.Equipments.Where(e => e != null && e.GetType() == typeof(MeleeWeaponData)).Cast<MeleeWeaponData>().ToList();
            int index = 0;
            
            foreach (var weaponPanel in _weaponPrints) 
            {
                if (index >= list.Count)
                    break;
                if (weaponPanel.IsEmpty)
                {
                    string nameCharacteristic = _skillCreator.SkillByName(list[index].specialization.skill).characteristic;
                    weaponPanel.SetWeapon(list[index], CalculateTotalWeapon(list[index], nameCharacteristic));
                    index++;
                }                    
            }

            var rangedList = _character.Equipments.Where(e => e != null && e.GetType() == typeof(RangedWeaponData)).Cast<RangedWeaponData>().ToList();
            index = 0;
            foreach (var weaponPanel in _weaponPrints)
            {
                if (index >= rangedList.Count)
                    break;
                if (weaponPanel.IsEmpty)
                {
                    string nameCharacteristic = _skillCreator.SkillByName(rangedList[index].specialization.skill).characteristic;
                    weaponPanel.SetWeapon(rangedList[index], CalculateTotalWeapon(rangedList[index], nameCharacteristic));
                    index++;
                }                    
            }
        }

        private int CalculateTotalWeapon(MeleeWeaponData meleeWeaponData, string characteristicName)
        {
            _nameToCharacterSpecDict.TryGetValue(meleeWeaponData.specialization.specialization, out SpecializationData specialization);
            _nameToCharacterSkillDict.TryGetValue(meleeWeaponData.specialization.skill, out SkillData skill);
            int totalCount = _nameToCharacterCharacteristicDict[characteristicName].Level;
            if (specialization != null)
            {
                totalCount += specialization.level * 5;
            }
                
            if(skill != null)
            {
                totalCount += skill.level * 5;
            }
            return totalCount;
        }

        private void FillArmors()
        {
            var list = _character.Equipments.Where(e => e != null && e.GetType() == typeof(ArmorData)).Cast<ArmorData>().ToList();
            int index = 0;
            foreach (var item in _armorPrints)
            {
                if (index >= list.Count)
                    break;
                if (item.IsEmpty)
                {
                    item.SetArmor(list[index]);
                    index++;
                }
            }

            _zonesPrints.SetArmorPoints(list, _character.Augmetics.ToList());
        }

        private void FillEquipments()
        {
            var list = _character.Equipments.Where(e => e != null && e.GetType() == typeof(EquipmentData)).Cast<EquipmentData>().ToList();
            int index = 0;
            foreach (var item in _equipments)
            {
                if (index >= list.Count)
                    break;
                item.text = list[index].name;
                index++;
            }
        }

        private void FillPsyPowers()
        {
            int index = 0;
            foreach (var item in _psyPowerPrints)
            {
                if (index >= _character.PsyPowers.Count)
                    break;

                var psyPower = _character.PsyPowers[index];
                item.SetPsyPower(psyPower, CalculateForcePsyPower(psyPower.specialization) + psyPower.testDifficulty);
                index++;
            }
        }

        private int CalculateForcePsyPower(string nameSchool)
        {
            _nameToCharacterSpecDict.TryGetValue(nameSchool, out SpecializationData spec);
            _nameToCharacterSkillDict.TryGetValue("Психическое мастерство", out SkillData skill);
            _nameToCharacterCharacteristicDict.TryGetValue("Сила воли", out Characteristic characteristic);

            int count = characteristic.Level;
            if (skill != null)
                count += skill.level * 5;
            if(spec != null)
                count += spec.level * 5;

            return count;
        }
    }
}

