using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;

namespace CharacterCreation
{
    public class BackgroundCreator : IDataCreator
    {
        private readonly List<BackgroundData> _backgrounds = new();
        private Dictionary<string, BackgroundData> _backgroundsByName = new();
        public IReadOnlyList<BackgroundData> Backgrounds => _backgrounds;
        public BackgroundData BackgroundsByName(string name) => _backgroundsByName[name];

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            _backgrounds.Clear();

            string basePath = Path.Combine(Application.streamingAssetsPath, "Служба");
            if (!Directory.Exists(basePath))
            {
                Debug.LogError($"Папка служб не найдена: {basePath}");
                return;
            }

            var backgroundFolders = Directory.GetDirectories(basePath);
            foreach (var folderPath in backgroundFolders)
            {
                string parametersPath = Path.Combine(folderPath, "parameters.json");
                if (File.Exists(parametersPath))
                {
                    string json = await File.ReadAllTextAsync(parametersPath, cancellationToken);
                    _backgrounds.Add(JsonConvert.DeserializeObject<BackgroundData>(json));
                }
                else
                    Debug.LogAssertion($"Не найден файл parametersPath = {parametersPath}");

                string chancePath = Path.Combine(folderPath, "Chance.json");
                if (File.Exists(chancePath))
                {
                    string json = await File.ReadAllTextAsync(chancePath, cancellationToken);
                    var backgroundChance = JsonConvert.DeserializeObject<BackgroundChances>(json);
                    _backgrounds[^1].originChance = backgroundChance;
                }
                else
                    Debug.LogAssertion($"Не найден файл chancePath = {chancePath}");

                string samplesPath = Path.Combine(folderPath, "Samples");
                if (Directory.Exists(samplesPath))
                {
                    var sampleFiles = Directory.GetFiles(samplesPath, "*.json");
                    foreach (var sampleFile in sampleFiles)
                    {
                        string json = await File.ReadAllTextAsync(sampleFile, cancellationToken);
                        var templateData = JsonConvert.DeserializeObject<TemplateBackground>(json);
                        _backgrounds[^1].templates.Add(templateData);
                    }
                }
                else
                    Debug.LogAssertion($"Не найден файл samplesPath = {samplesPath}");
                await UniTask.Yield();
            }

            foreach (var item in _backgrounds)
                _backgroundsByName.Add(item.serviceName, item);
        }
    }
    [System.Serializable]
    public class BackgroundChance
    {
        public string origin;
        public int[] range; // [min, max]
    }

    [System.Serializable]
    public class BackgroundChances
    {
        public string service;
        public List<BackgroundChance> originChances;
    }

    [System.Serializable]
    public class SkillUpgrade
    {
        public int amount;
        public List<string> skills;
    }

    [System.Serializable]
    public class InfluenceBonus
    {
        public string faction;
        public int amount;
        public bool alternative_allowed;
    }

    [System.Serializable]
    public class TalentChoice
    {
        public string type; // "fixed" или "choice_set"
        public List<List<string>> choices; // если type == "choice_set"
        public List<string> talents;       // если type == "fixed"
    }

    [System.Serializable]
    public class GearData
    {
        public List<string> items;
        public int money;
        public List<string> choice; // опционально
    }

    [System.Serializable]
    public class BackgroundData
    {
        public string serviceName;
        public string description;
        public Dictionary<string, int> fixed_bonus;
        public Dictionary<string, int> selectable_bonuses;
        public SkillUpgrade skill_upgrades;
        public InfluenceBonus influence_bonus;
        public int contacts;
        public List<TalentChoice> talents;
        public GearData gear;
        public List<TemplateBackground> templates;
        public BackgroundChances originChance;
    }

    [System.Serializable]
    public class TemplateBackground
    {
        public string templateName;
        public string description;
        public Dictionary<string, int> fixed_bonus;
        public Dictionary<string, int> skill_upgrades;
        public List<TalentChoice> talents;
        public GearData gear;
        public InfluenceBonus influence_bonus;
        public int contacts;
    }
}

