using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;

namespace CharacterCreation
{
    public class TalentCreator : IDataCreator
    {
        private readonly List<TalentData> _talents = new();
        private Dictionary<string, TalentData> _talentByName = new Dictionary<string, TalentData>();
        public IReadOnlyList<TalentData> Talents => _talents;
        public TalentData TalentByName(string name) => _talentByName[name];

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            _talents.Clear();

            string folderPath = Path.Combine(Application.streamingAssetsPath, "Таланты");
            if (!Directory.Exists(folderPath))
            {
                Debug.LogError($"Папка с талантами не найдена: {folderPath}");
                return;
            }

            string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");
            foreach (string filePath in jsonFiles)
            {
                if (cancellationToken.IsCancellationRequested) return;

                string json = await File.ReadAllTextAsync(filePath, cancellationToken);
                TalentData data = JsonUtility.FromJson<TalentData>(json);
                if (data != null)
                {
                    _talents.Add(data);
                    _talentByName.Add(data.name, _talents[^1]);
                }
                else
                {
                    Debug.LogWarning($"Ошибка чтения таланта из файла: {filePath}");
                }
                await UniTask.Yield();
            }

            Debug.Log($"Загружено талантов: {_talents.Count}");
        }


    }

    [System.Serializable]
    public class TalentRequirement
    {
        public string type;
        public string attribute;
        public int value;
        public string skill;
        public string specialization;
        public int amount;
        public List<string> talents;
    }

    [System.Serializable]
    public class TalentData
    {
        public string name;
        public string description;
        public List<TalentRequirement> requirements;
        public List<string> tags;
        public bool isMultiple;
        public int maxMultiple;
        public bool uniqeText;
    }
}

