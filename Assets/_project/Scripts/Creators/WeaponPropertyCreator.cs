using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace CharacterCreation
{
    public class WeaponPropertyCreator : DataCreator, IDataCreator
    {
        private readonly List<WeaponPropertyData> _weaponProperties = new();
        private Dictionary<string, WeaponPropertyData> _weaponPropertyByName = new();
        public IReadOnlyList<WeaponPropertyData> WeaponProperties => _weaponProperties;
        public WeaponPropertyData WeaponPropertyByName(string name) => _weaponPropertyByName.GetValueOrDefault(name);

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            string basePath = Path.Combine(Application.streamingAssetsPath, "Арсенал");

            await LoadAndAddAsync<WeaponPropertyDataList, WeaponPropertyData>("Свойства.json",
                _weaponProperties, cancellationToken, list => list.data, basePath);

            foreach (var item in _weaponProperties)            
                _weaponPropertyByName.Add(item.name, item);
            
        }

        [System.Serializable]
        public class WeaponPropertyData
        {
            public string name;
            public string description;
            public bool lvl; 
        }

        [System.Serializable]
        public class WeaponPropertyDataList
        {
            public List<WeaponPropertyData> data;
        }
    }
}

