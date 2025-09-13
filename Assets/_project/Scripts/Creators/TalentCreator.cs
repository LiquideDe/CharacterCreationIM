using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace CharacterCreation
{
    public class TalentCreator : IDataCreator, INameProvider
    {
        private readonly List<TalentData> _talents = new();
        private Dictionary<string, TalentData> _talentByName = new Dictionary<string, TalentData>();
        public IReadOnlyList<TalentData> Talents => _talents;
        public TalentData TalentByName(string name) => _talentByName.TryGetValue(name, out var t) ? t : null;
        public Type ItemType => typeof(TalentData);

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            _talents.Clear();
            _talentByName.Clear();

            string folderPath = Path.Combine(Application.streamingAssetsPath, "Таланты");
            if (!Directory.Exists(folderPath))
            {
                Debug.LogError($"Папка с талантами не найдена: {folderPath}");
                return;
            }

            string[] jsonFiles = Directory.GetFiles(folderPath, "*.json", SearchOption.TopDirectoryOnly);
            foreach (string filePath in jsonFiles)
            {
                if (cancellationToken.IsCancellationRequested) return;

                try
                {
                    string json = await File.ReadAllTextAsync(filePath, cancellationToken);

                    try
                    {
                        JToken.Parse(json);
                    }
                    catch (JsonReaderException jex)
                    {
                        LogJsonSyntaxError(filePath, json, jex);
                        continue; // к следующему файлу
                    }

                    TalentData data = null;
                    try
                    {
                        data = JsonUtility.FromJson<TalentData>(json);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Не удалось распарсить TalentData (JsonUtility) из файла:\n{filePath}\n{ex}");
                        try
                        {
                            data = JsonConvert.DeserializeObject<TalentData>(json);
                        }
                        catch (Exception nEx)
                        {
                            Debug.LogError($"Newtonsoft.Json тоже не смог прочитать файл:\n{filePath}\n{nEx.Message}");
                            continue;
                        }
                    }

                    if (data == null)
                    {
                        Debug.LogWarning($"Файл прочитан, но объект пуст: {filePath}");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(data.name))
                    {
                        Debug.LogWarning($"Талант без имени пропущен. Файл: {filePath}");
                        continue;
                    }

                    if (_talentByName.ContainsKey(data.name))
                    {
                        Debug.LogWarning($"Дубликат таланта '{data.name}' — файл пропущен: {filePath}");
                        continue;
                    }

                    _talents.Add(data);
                    _talentByName.Add(data.name, data);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Неожиданная ошибка при обработке файла:\n{filePath}\n{ex}");
                }

                await UniTask.Yield();
            }

            Debug.Log($"Загружено талантов: {_talents.Count}");

        }

        private static void LogJsonSyntaxError(string filePath, string json, JsonReaderException jex)
        {
            var lines = json.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            int idx = Math.Max(0, jex.LineNumber - 1);
            string context = lines.Length > idx ? lines[idx] : "";

            string pointer = new string(' ', Math.Max(0, jex.LinePosition - 1)) + "^";

            Debug.LogError(
                $"Ошибка синтаксиса JSON в файле:\n{filePath}\n" +
                $"{jex.Message}\n" +
                $"Строка: {jex.LineNumber}, Позиция: {jex.LinePosition}\n" +
                $"--------------------------------\n{context}\n{pointer}\n--------------------------------"
            );
        }

        public bool TryGet(string name, out object value)
        {
            value = TalentByName(name);
            return value != null;
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
        public bool isMultiple;
        public int maxMultiple;
        public int currentMultiple;
        public bool uniqeText;
        public bool character_creation_only;
    }
}

