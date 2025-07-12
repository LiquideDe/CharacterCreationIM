using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace CharacterCreation
{
    public class EquipmentCreator : DataCreator,IDataCreator
    {
        private readonly List<EquipmentData> _equipments = new();
        private readonly List<AmmunitionData> _ammunitions = new();
        private readonly List<ForceFieldData> _forceFields = new();
        private readonly List<WeaponUpgradeData> _weaponUpgrade = new();
        private readonly List<ArmorData> _armors = new();
        private readonly List<MeleeWeaponData> _meleeWeapons = new();
        private readonly List<RangedWeaponData> _rangeWeapons = new();
        private readonly List<MeleeWeaponData> _grenades = new();
        private Dictionary<string, EquipmentData> _equipmentByName = new();

        public IReadOnlyList<EquipmentData> Equipments => _equipments;
        public IReadOnlyList<AmmunitionData> Ammunitions => _ammunitions;
        public IReadOnlyList<WeaponUpgradeData> WeaponUpgrade => _weaponUpgrade;
        public IReadOnlyList<ArmorData> Armors => _armors;
        public IReadOnlyList<MeleeWeaponData> MeleeWeapon => _meleeWeapons;
        public IReadOnlyList<RangedWeaponData> RangedWeapon => _rangeWeapons;
        public IReadOnlyList<MeleeWeaponData> Grenades => _grenades;
        public IReadOnlyList<ForceFieldData> ForceFields => _forceFields;
        public EquipmentData EquipmentDataByName(string name) => _equipmentByName.GetValueOrDefault(name);

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            _equipments.Clear();
            _ammunitions.Clear();
            _weaponUpgrade.Clear();
            _armors.Clear();
            _rangeWeapons.Clear();
            _meleeWeapons.Clear();

            string basePath = Path.Combine(Application.streamingAssetsPath, "Арсенал");

            var tasks = new List<UniTask>() 
            {
                LoadAndAddAsync<EquipmentDataList, EquipmentData>("Cнаряжение.json",
                _equipments, cancellationToken, list => list.data, basePath),

                LoadAndAddAsync<EquipmentDataList, EquipmentData>("Инструменты.json",
                _equipments, cancellationToken, list => list.data, basePath),

                LoadAndAddAsync<ArmorsDataList, ArmorData>("Броня.json",
                _armors, cancellationToken, list => list.data, basePath),

                LoadAndAddAsync<GrenadesWeaponDataList, MeleeWeaponData>("Гранаты.json",
                _grenades, cancellationToken, list => list.data, basePath),

                LoadAndAddAsync<ForceFieldDataList, ForceFieldData>("Защитные поля.json",
                _forceFields, cancellationToken, list => list.data, basePath),

                LoadAndAddAsync<WeaponUpgradeDataList, WeaponUpgradeData>("Улучшения оружия.json",
                _weaponUpgrade, cancellationToken, list => list.data, basePath),                

                LoadAndAddAsync<AmmunitionDataList, AmmunitionData>("Патроны.json",
                _ammunitions, cancellationToken, list => list.data, basePath),

                LoadAndAddAsync<RangedWeaponDataList, RangedWeaponData>("Стрелковое оружие.json",
                _rangeWeapons, cancellationToken, list => list.data, basePath),

                LoadAndAddAsync<MeleeWeaponDataList, MeleeWeaponData>("Холодное оружие.json",
                _meleeWeapons, cancellationToken, list => list.data, basePath)
            };

            await UniTask.WhenAll(tasks);

            AddListAtDict(_equipments);
            AddListAtDict(_ammunitions);
            AddListAtDict(_forceFields);
            AddListAtDict(_weaponUpgrade);
            AddListAtDict(_armors);
            AddListAtDict(_meleeWeapons);
            AddListAtDict(_rangeWeapons);
            AddListAtDict(_grenades);
        }

        private void AddListAtDict<T>(List<T> equipments) where T : EquipmentData
        {
            foreach (var item in equipments)
                _equipmentByName.Add(item.name, item);
        }
        
    }

    [System.Serializable]
    public class EquipmentData
    {
        public string name;
        public string description;
        public int weight;
        public int maxWeight;
    }

    [System.Serializable]
    public class AmmunitionData : EquipmentData
    {
        public string effect;
    }

    [System.Serializable]
    public class WeaponUpgradeData : EquipmentData
    {
        public string typeWeapon;
    }

    [System.Serializable]
    public class ArmorData : EquipmentData
    {
        public string type;
        public new Dictionary<string, int> weight;
        public List<string> protectionZones;
        public List<string> properties;
        public int armorPoints;
    }

    [System.Serializable]
    public class MeleeWeaponData : EquipmentData
    {
        public string type;
        public Dictionary<string, int> specialization;
        public int damage;
        public List<string> properties;
    }

    [System.Serializable]
    public class RangedWeaponData : MeleeWeaponData
    {
        public int range;
        public int clip;
    }

    [System.Serializable]
    public class ForceFieldData : EquipmentData
    {
        public string type;
        public string defense;
        public int reload;
    }

    [System.Serializable]
    public class EquipmentDataList
    {
        public List<EquipmentData> data;
    }

    [System.Serializable]
    public class ArmorsDataList
    {
        public List<ArmorData> data;
    }

    [System.Serializable]
    public class AmmunitionDataList
    {
        public List<AmmunitionData> data;
    }

    [System.Serializable]
    public class WeaponUpgradeDataList
    {
        public List<WeaponUpgradeData> data;
    }

    [System.Serializable]
    public class MeleeWeaponDataList
    {
        public List<MeleeWeaponData> data;
    }

    [System.Serializable]
    public class RangedWeaponDataList
    {
        public List<RangedWeaponData> data;
    }

    [System.Serializable]
    public class GrenadesWeaponDataList
    {
        public List<MeleeWeaponData> data;
    }

    [System.Serializable]
    public class ForceFieldDataList
    {
        public List<ForceFieldData> data;
    }
}

