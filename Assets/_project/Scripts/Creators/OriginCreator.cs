using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
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
        private OriginData[] _rollToOrigin = new OriginData[101];

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            _origins.Clear();
            Array.Clear(_rollToOrigin, 0, _rollToOrigin.Length);

            string basePath = Path.Combine(Application.streamingAssetsPath, "Происхождения");
            await LoadAndAddAsync<OriginsList, OriginData>("Происхождение.json",
                _origins, cancellationToken, list => list.data, basePath);

            foreach (var item in _origins)
                _originByName.Add(item.name, item);

            string chancePath = Path.Combine(Application.streamingAssetsPath, "Происхождения/Шансы происхождений.json");
            if (!File.Exists(chancePath))
            {
                Debug.LogAssertion($"Не найден файл chancePath = {chancePath}");
                return;
            }

            string json = await File.ReadAllTextAsync(chancePath, cancellationToken);
            var originChance = JsonConvert.DeserializeObject<OriginChances>(json);

            if (originChance?.originChances == null || originChance.originChances.Count == 0)
            {
                Debug.LogError("Файл шансов пуст или невалиден.");
                return;
            }

            // 3) Заполняем таблицу 1..100 и валидируем
            foreach (var ch in originChance.originChances)
            {
                if (!_originByName.TryGetValue(ch.origin, out var data))
                {
                    Debug.LogError($"Не найдено OriginData для '{ch.origin}' — проверь имена.");
                    continue;
                }

                if (ch.range == null || ch.range.Length != 2)
                {
                    Debug.LogError($"Неверный range у '{ch.origin}'. Ожидается [min,max].");
                    continue;
                }

                int min = Math.Min(ch.range[0], ch.range[1]);
                int max = Math.Max(ch.range[0], ch.range[1]);

                if (min < 1 || max > 100)
                {
                    Debug.LogError($"Диапазон '{ch.origin}' выходит за пределы 1..100: [{min}..{max}].");
                    continue;
                }

                for (int r = min; r <= max; r++)
                {
                    if (_rollToOrigin[r] != null)
                    {
                        Debug.LogError(
                            $"Перекрытие диапазонов при значении {r}: уже задано '{_rollToOrigin[r].name}', " +
                            $"пытаемся записать '{data.name}'.");
                    }
                    else
                    {
                        _rollToOrigin[r] = data;
                    }
                }
            }

            // 4) Проверим “дырки”
            for (int r = 1; r <= 100; r++)
            {
                if (_rollToOrigin[r] == null)
                    Debug.LogAssertion($"Нет шанса для броска {r} (дырка в таблице 1..100).");
            }
            Debug.Log($"Загружено {_origins.Count} происхождений с шансами.");
        }

        public OriginData GetByRoll(int roll)
        {
            if (roll < 1 || roll > 100)
                throw new ArgumentOutOfRangeException(nameof(roll), "Ожидается значение 1..100.");

            var data = _rollToOrigin[roll];
            if (data == null)
                throw new KeyNotFoundException($"Бросок {roll} не попал ни в один диапазон.");

            return data;
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

    [System.Serializable]
    public class OriginChances
    {
        public List<OriginChance> originChances;
    }

    [System.Serializable]
    public class OriginChance 
    {
        public string origin;
        public int[] range;
    }
}

