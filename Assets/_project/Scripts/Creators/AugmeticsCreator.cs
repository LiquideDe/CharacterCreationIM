using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;


namespace CharacterCreation
{
    public class AugmeticsCreator : DataCreator, IDataCreator, INameProvider
    {
        private readonly List<AugmeticData> _augmetics = new();
        private Dictionary<string, AugmeticData> _augmeticByName = new(new NormalizingStringComparer());

        public IReadOnlyList<AugmeticData> Augmetics => _augmetics;
        public AugmeticData AugmeticByName(string name) => _augmeticByName.TryGetValue(name, out var t) ? t : null;
        public Type ItemType => typeof(AugmeticData);
        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            string basePath = Path.Combine(Application.streamingAssetsPath, "Арсенал");

            await LoadAndAddAsync<AugmeticDataList, AugmeticData>("Аугметика.json",
                _augmetics, cancellationToken, list => list.data, basePath);

            foreach (var item in _augmetics)
                _augmeticByName.Add(item.name, item);
        }

        public bool TryGet(string name, out object value)
        {
            value = AugmeticByName(name);
            return value != null;
        }
    }

    [System.Serializable]
    public class AugmeticData
    {
        public string name;
        public string description;
        public int armor;
        public string place;
        public bool mechanicus;
    }

    [System.Serializable]
    public class AugmeticDataList
    {
        public List<AugmeticData> data;
    }
}

