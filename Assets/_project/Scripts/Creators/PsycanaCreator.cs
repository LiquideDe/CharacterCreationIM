using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace CharacterCreation
{
    public class PsycanaCreator : DataCreator, IDataCreator, INameProvider
    {
        private readonly List<PsyData> _psyPowers = new();
        private Dictionary<string, PsyData> _psyPowerByName = new();
        public IReadOnlyList<PsyData> PsyPowers => _psyPowers;
        public PsyData PsyPowerByName(string name) => _psyPowerByName.GetValueOrDefault(name);
        public Type ItemType => typeof(PsyData);

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            _psyPowers.Clear();

            string basePath = Path.Combine(Application.streamingAssetsPath, "Психосилы");

            await LoadAndAddAsync<PsyPowersList, PsyData>("Психосилы.json",
                _psyPowers, cancellationToken, list => list.data, basePath);

            foreach (var item in _psyPowers)
                _psyPowerByName.Add(item.name,item);
        }

        public bool TryGet(string name, out object value)
        {
            value = PsyPowerByName(name);
            return value != null;
        }
    }

    [System.Serializable]
    public class PsyData
    {
        public string name;
        public string description;
        public bool isObvious;
        public bool isLesser;
        public int warpCharge;
        public int range;
        public int testDifficulty;
        public string target;
        public string duration;
        public string specialization;

    }

    [System.Serializable]
    public class PsyPowersList
    {
        public List<PsyData> data;
    }
}

