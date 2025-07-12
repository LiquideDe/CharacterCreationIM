using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;


namespace CharacterCreation
{
    public class AugmeticsCreator : DataCreator,IDataCreator
    {
        private readonly List<AugmeticData> _augmetics = new();
        private Dictionary<string, AugmeticData> _augmeticByName = new();

        public IReadOnlyList<AugmeticData> Augmetics => _augmetics;
        public AugmeticData AugmeticByName(string name) => _augmeticByName[name];
        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            string basePath = Path.Combine(Application.streamingAssetsPath, "Арсенал");

            await LoadAndAddAsync<AugmeticDataList, AugmeticData>("Аугметика.json",
                _augmetics, cancellationToken, list => list.data, basePath);

            foreach (var item in _augmetics)
                _augmeticByName.Add(item.name, item);
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

