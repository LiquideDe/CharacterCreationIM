using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace CharacterCreation
{
    public class WeaponQualityCreator : DataCreator, IDataCreator
    {
        private readonly List<WeaponQualityData> _weaponQualities = new();
        private Dictionary<string, WeaponQualityData> _weaponQualitiesByName = new();
        public IReadOnlyList<WeaponQualityData> WeaponQualityDatas => _weaponQualities;

        public WeaponQualityData WeaponQualitiesByName(string name) => _weaponQualitiesByName.GetValueOrDefault(name); 

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            string basePath = Path.Combine(Application.streamingAssetsPath, "Арсенал");

            await LoadAndAddAsync<WeaponQualityDataList, WeaponQualityData>("Качество.json",
                _weaponQualities, cancellationToken, list => list.data, basePath);

            foreach (var item in _weaponQualities)            
                _weaponQualitiesByName.Add(item.name, item);
            
        }

        [System.Serializable]
        public class WeaponQualityData
        {
            public string name;
            public string description;
        }

        [System.Serializable]
        public class  WeaponQualityDataList
        {
            public List<WeaponQualityData> data;
        }
    }
}

