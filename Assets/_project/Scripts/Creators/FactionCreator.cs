using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace CharacterCreation
{
    public class FactionCreator : IDataCreator
    {
        private readonly List<FactionData> _factions = new();
        private Dictionary<string, FactionData> _factionByName = new();
        private readonly Dictionary<string, FactionData[]> _byOrigin =
        new(StringComparer.Ordinal);
        public IReadOnlyList<FactionData> Factions => _factions;
        public FactionData BackgroundsByName(string name) => _factionByName[name];

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            _factions.Clear();

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
                    _factions.Add(JsonConvert.DeserializeObject<FactionData>(json));
                }
                else
                    Debug.LogAssertion($"Не найден файл parametersPath = {parametersPath}");

                string chancePath = Path.Combine(folderPath, "Chance.json");
                if (File.Exists(chancePath))
                {
                    string json = await File.ReadAllTextAsync(chancePath, cancellationToken);
                    var backgroundChance = JsonConvert.DeserializeObject<FactionChances>(json);
                    _factions[^1].originChance = backgroundChance;
                }
                else
                    Debug.LogAssertion($"Не найден файл chancePath = {chancePath}");
                _factions[^1].templates = new List<TemplateFaction>();
                string samplesPath = Path.Combine(folderPath, "Samples");
                if (Directory.Exists(samplesPath))
                {
                    var sampleFiles = Directory.GetFiles(samplesPath, "*.json");
                    foreach (var sampleFile in sampleFiles)
                    {
                        string json = await File.ReadAllTextAsync(sampleFile, cancellationToken);
                        TemplateFaction templateData = null;
                        try
                        {
                            templateData = JsonConvert.DeserializeObject<TemplateFaction>(json);
                        }
                        catch
                        {
                            Debug.LogAssertion($"Failed to parse {sampleFile}");
                        }
                        
                        _factions[^1].templates.Add(templateData);
                    }
                }
                else
                    Debug.LogAssertion($"Не найден файл samplesPath = {samplesPath}");
                await UniTask.Yield();
            }

            foreach (var item in _factions)
                try
                {
                    _factionByName.Add(item.serviceName, item);
                }
                catch 
                {
                    Debug.LogAssertion($"Не смогли добавить службу {item.serviceName} в словарь _backgroundsByName");
                }
            
            Build(_factions);

        }

        public FactionData GetFactionForRoll(string origin, int roll)
        {
            if (string.IsNullOrWhiteSpace(origin) || roll < 1 || roll > 100)
                return null;

            var key = Norm(origin);
            if (!_byOrigin.TryGetValue(key, out var table) || table == null)
                return null;

            return table[roll];
        }

        private void Build(IEnumerable<FactionData> allServices)
        {
            if (allServices == null) return;

            foreach (var svc in allServices)
            {
                var oc = svc?.originChance;
                if (oc?.originChances == null || oc.originChances.Count == 0)
                    continue;

                foreach (var chance in oc.originChances)
                {
                    if (chance?.origin == null || chance.range == null || chance.range.Length != 2)
                    {
                        Debug.LogWarning($"[{oc.service}] пропущена запись originChance (некорректные данные).");
                        continue;
                    }

                    var originKey = Norm(chance.origin);

                    if (!_byOrigin.TryGetValue(originKey, out var table))
                    {
                        table = new FactionData[101]; // индекс 1..100
                        _byOrigin[originKey] = table;
                    }

                    int min = Math.Min(chance.range[0], chance.range[1]);
                    int max = Math.Max(chance.range[0], chance.range[1]);

                    if (min < 1 || max > 100)
                    {
                        Debug.LogError($"[{oc.service}] Диапазон для '{chance.origin}' выходит за 1..100: [{min}..{max}]");
                        min = Math.Clamp(min, 1, 100);
                        max = Math.Clamp(max, 1, 100);
                    }

                    for (int r = min; r <= max; r++)
                    {
                        var prev = table[r];
                        if (prev != null && !ReferenceEquals(prev, svc))
                        {
                            Debug.LogError(
                                $"Перекрытие для происхождения '{chance.origin}' в точке {r}: " +
                                $"уже '{prev.originChance?.service}', теперь '{oc.service}'.");
                        }
                        table[r] = svc; // если в одном сервисе будет несколько интервалов для одного origin — это ок
                    }
                }
            }

            // Проверка «дыр» (опционально)
            foreach (var kv in _byOrigin)
            {
                var origin = kv.Key;
                var table = kv.Value;
                for (int r = 1; r <= 100; r++)
                {
                    if (table[r] == null)
                    {
                        Debug.LogWarning($"Для происхождения '{origin}' нет службы при броске {r} (дырка в 1..100).");
                        break; // чтобы не заспамить — достаточно 1 предупреждения на origin
                    }
                }
            }
        }

        private string Norm(string s)
        => (s ?? "").Trim().ToLowerInvariant().Replace('ё', 'е');

    }
    [System.Serializable]
    public class FactionChance
    {
        public string origin;
        public int[] range; // [min, max]
    }

    [System.Serializable]
    public class FactionChances
    {
        public string service;
        public List<FactionChance> originChances;
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
        public int amount_choice;
        public List<string> choice; // опционально
    }

    [System.Serializable]
    public class FactionData
    {
        public string serviceName;
        public string description;
        public Dictionary<string, int> fixed_bonus;
        public Dictionary<string, int> selectable_bonuses;
        public SkillUpgrade skill_upgrades;
        public InfluenceBonus influence_bonus;
        public int contacts;
        public ImplantsData implants_data;
        public List<TalentChoice> talents;
        public GearData gear;
        public List<TemplateFaction> templates;
        public FactionChances originChance;
    }

    [Serializable]
    public class TemplateFaction
    {
        public string templateName;
        public string description;
        public Dictionary<string, int> fixed_bonus;
        public SkillUpgradesBlock skill_upgrades;
        public List<TalentChoice> talents;
        public List<string> implants;
        public GearData gear;
        public InfluenceBonus influence_bonus;
        public int contacts;
    }

    [Serializable]
    public class SkillUpgradesBlock
    {
        public Dictionary<string, int> upgrades;
    }

    [Serializable]
    public class ImplantsData
    {
        public List<string> first_implants;
        public List<string> second_implants;
    }
}

