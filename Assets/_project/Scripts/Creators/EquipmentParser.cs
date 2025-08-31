using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Windows;
using Zenject;
using static CharacterCreation.WeaponPropertyCreator;
using static CharacterCreation.WeaponQualityCreator;

namespace CharacterCreation
{
    public class EquipmentParser
    {
        [Inject] private EquipmentCreator _equipmentCreator = null;
        [Inject] private WeaponPropertyCreator _weaponPropertyCreator = null;
        [Inject] private WeaponQualityCreator _weaponQualityCreator = null;

        private readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
        {
        "и", "и,", ",", "и.", "и-", "и–"
        };

        public EquipmentData TryGetEquipment(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Пустое название предмета.", nameof(input));

            string working = NormalizeSpaces(ReplaceYo(input)).Trim();
            string normWorking = Norm(working);
            List<string> newpropertiesities = new();

            var matchedQual = MatchAnyTokenOnce(normWorking, _weaponQualityCreator.WeaponQualitiesDictionaty.Keys);
            if (matchedQual != null)
            {
                newpropertiesities.Add(_weaponQualityCreator.WeaponQualitiesByName(matchedQual).name);
                normWorking = RemoveWholeWord(normWorking, matchedQual);
            }

            foreach (var propName in _weaponPropertyCreator.WeaponPropertiesDictionaty.Keys)
            {
                if (ContainsWholeWord(normWorking, propName))
                {
                    newpropertiesities.Add(_weaponPropertyCreator.WeaponPropertyByName(propName).name);
                    normWorking = RemoveWholeWord(normWorking, propName);
                }
            }

            normWorking = Regex.Replace(normWorking, @"\bи\b", " ", RegexOptions.IgnoreCase);
            normWorking = NormalizeSpaces(normWorking).Trim();

            EquipmentData result = _equipmentCreator.EquipmentDataByName(normWorking);

            if (result == null)
                return null;

            var list = new List<string>();
            if (result == null)
                Debug.LogAssertion($"Не найден предмет: '{input}' (норм.: '{normWorking}')");
            if (result.properties != null)
                list.AddRange(result.properties);
            list.AddRange(newpropertiesities);
            switch (result)
            {
                case AmmunitionData ammunitionData:                    
                    
                    return new AmmunitionData()
                    {
                        name = ammunitionData.name,
                        description = ammunitionData.description,
                        weight = ammunitionData.weight,
                        maxWeight = ammunitionData.maxWeight,
                        properties = new List<string>(list),
                        effect = ammunitionData.effect
                    };

                case WeaponUpgradeData weaponUpgradeData:
                    return new WeaponUpgradeData()
                    {
                        name = weaponUpgradeData.name,
                        description = weaponUpgradeData.description,
                        weight = weaponUpgradeData.weight,
                        maxWeight = weaponUpgradeData.maxWeight,
                        properties = new List<string>(list),
                        typeWeapon = new List<string>(weaponUpgradeData.typeWeapon)
                    };

                case ArmorData armorData:
                    return new ArmorData()
                    {
                        name = armorData.name,
                        description = armorData.description,
                        weight = armorData.weight,
                        maxWeight = armorData.maxWeight,
                        properties = new List<string>(list),
                        type = armorData.type,
                        protectionZones = new List<string>(armorData.protectionZones),
                        armorPoints = armorData.armorPoints
                    };                    

                case RangedWeaponData rangedWeaponData:
                    return new RangedWeaponData()
                    {
                        name = rangedWeaponData.name,
                        description = rangedWeaponData.description,
                        weight = rangedWeaponData.weight,
                        maxWeight = rangedWeaponData.maxWeight,
                        properties = new List<string>(list),
                        type = rangedWeaponData.type,
                        specialization = rangedWeaponData.specialization,
                        damage = rangedWeaponData.damage,
                        range = rangedWeaponData.range,
                        clip = rangedWeaponData.clip
                    };

                case MeleeWeaponData meleeWeaponData:
                        return new MeleeWeaponData()
                    {
                        name = meleeWeaponData.name,
                        description = meleeWeaponData.description,
                        weight = meleeWeaponData.weight,
                        maxWeight = meleeWeaponData.maxWeight,
                        properties = new List<string>(list),
                        type = meleeWeaponData.type,
                        specialization = meleeWeaponData.specialization,
                        damage = meleeWeaponData.damage
                    };  
                    
                    case ForceFieldData forceFieldData:
                        return new ForceFieldData()
                    {
                        name = forceFieldData.name,
                        description = forceFieldData.description,
                        weight = forceFieldData.weight,
                        maxWeight = forceFieldData.maxWeight,
                        properties = new List<string>(list),
                        defense = forceFieldData.defense,
                        reload = forceFieldData.reload
                    };


                case EquipmentData equipmentData:
                    return new EquipmentData()
                    {
                        name = equipmentData.name,
                        description = equipmentData.description,
                        weight = equipmentData.weight,
                        maxWeight = equipmentData.maxWeight,
                        properties = new List<string>(list)
                    };

                default:
                    {
                        Debug.LogError($"Неизвестный тип экипировки: {result.GetType().Name}, входящее имя {input}");
                        return null;
                    }
            }
        }

        private string ReplaceYo(string s) => s?.Replace('ё', 'е').Replace('Ё', 'Е');

        private string NormalizeSpaces(string s) => Regex.Replace(s ?? "", @"\s+", " ");

        private string Norm(string s)
        {
            s = ReplaceYo(s ?? "");
            s = s.ToLowerInvariant();
            s = Regex.Replace(s, @"\s+", " ");
            s = s.Trim();
            return s;
        }

        private bool ContainsWholeWord(string textNorm, string tokenNorm)
        {
            if (string.IsNullOrEmpty(tokenNorm)) return false;
            var pattern = $@"(?<!\w){Regex.Escape(tokenNorm)}(?!\w)";
            return Regex.IsMatch(textNorm, pattern, RegexOptions.IgnoreCase);
        }

        private string RemoveWholeWord(string textNorm, string tokenNorm)
        {
            var pattern = $@"(?<!\w){Regex.Escape(tokenNorm)}(?!\w)";
            var replaced = Regex.Replace(textNorm, pattern, " ", RegexOptions.IgnoreCase);
            replaced = Regex.Replace(replaced, @"\s*,\s*", " ");     // запятые → пробел
            replaced = Regex.Replace(replaced, @"\s+", " ");         // схлопнуть пробелы
            return replaced.Trim();
        }

        private string MatchAnyTokenOnce(string textNorm, IEnumerable<string> tokensNorm)
        {
            foreach (var t in tokensNorm)
                if (ContainsWholeWord(textNorm, t))
                    return t;
            return null;
        }
    }
}

