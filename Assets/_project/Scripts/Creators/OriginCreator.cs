using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace CharacterCreation
{
    public class OriginCreator : DataCreator,IDataCreator
    {
        private readonly List<OriginData> _origins = new();
        private Dictionary<string, OriginData> _originByName = new();
        public IReadOnlyList<OriginData> Backgrounds => _origins;
        public OriginData OriginByName(string name) => _originByName[name];

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            _origins.Clear();

            string basePath = Path.Combine(Application.streamingAssetsPath, "Происхождения");
            await LoadAndAddAsync<OriginsList, OriginData>("Происхождение.json",
                _origins, cancellationToken, list => list.data, basePath);

            foreach (var item in _origins)
                _originByName.Add(item.name, item);
        }
    }

    [System.Serializable]
    public class OriginData
    {
        public string name;
        public string description;
        public Dictionary<string, int> fixed_bonus;
        public Dictionary<string, int> selectable_bonuses;
        public List<string> items;
    }

    [System.Serializable]
    public class OriginsList
    {
        public List<OriginData> data;
    }
}

