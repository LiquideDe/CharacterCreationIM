using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace CharacterCreation
{
    public sealed class PatronCreator : IDataCreator
    {
        public sealed class DataList<T> { public List<T> data; }

        private readonly List<PatronTemplate> _templates = new();
        private readonly Dictionary<string, PatronOption> _possibilities = new(StringComparer.Ordinal);
        private readonly Dictionary<string, PatronOption> _weaknesses = new(StringComparer.Ordinal);
        private readonly List<PatronOption> _aspirations = new();
        private readonly List<PatronOption> _personalities = new();

        public IReadOnlyList<PatronTemplate> Templates => _templates;
        public IEnumerable<PatronOption> Possibilities => _possibilities.Values;
        public IReadOnlyList<PatronOption> Aspirations => _aspirations;
        public IReadOnlyList<PatronOption> Personalities => _personalities;
        public PatronOption Possibility(string name) => name != null && _possibilities.TryGetValue(name.Trim(), out var item) ? item : null;
        public PatronOption Weakness(string name) => name != null && _weaknesses.TryGetValue(name.Trim(), out var item) ? item : null;

        public UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            _templates.Clear();
            _possibilities.Clear();
            _weaknesses.Clear();
            _aspirations.Clear();
            _personalities.Clear();
            try
            {
                _templates.AddRange(Read<PatronTemplate>("Покровители.json", cancellationToken));
                foreach (var item in Read<PatronOption>("Возможности.json", cancellationToken))
                    Add(_possibilities, item, "Возможности.json");
                foreach (var option in _possibilities.Values)
                {
                    if (option.variants == null) continue;
                    var names = new HashSet<string>(StringComparer.Ordinal);
                    foreach (var variant in option.variants)
                        if (string.IsNullOrWhiteSpace(variant?.name) || !names.Add(variant.name))
                            Debug.LogWarning($"[PatronCreator] Возможности.json: «{option.name}» содержит пустой или повторяющийся вариант «{variant?.name}».");
                    if (option.repeatable && names.Count == 0)
                        Debug.LogWarning($"[PatronCreator] Возможности.json: «{option.name}» помечена повторяемой без вариантов.");
                }
                foreach (var item in Read<PatronOption>("Слабости.json", cancellationToken))
                    Add(_weaknesses, item, "Слабости.json");
                _aspirations.AddRange(Read<PatronOption>("Стремления.json", cancellationToken).Where(x => !string.IsNullOrWhiteSpace(x.name)));
                _personalities.AddRange(Read<PatronOption>("Характер.json", cancellationToken).Where(x => !string.IsNullOrWhiteSpace(x.name)));
                foreach (var template in _templates)
                {
                    foreach (var name in template.possibilities ?? new List<string>())
                        if (Possibility(name) == null) Debug.LogWarning($"[PatronCreator] Покровители.json: {template.faction}/{template.responsibilities}: возможность «{name}» отсутствует в Возможности.json");
                    foreach (var name in template.weakness ?? new List<string>())
                        if (Weakness(name) == null) Debug.LogWarning($"[PatronCreator] Покровители.json: {template.faction}/{template.responsibilities}: слабость «{name}» отсутствует в Слабости.json");
                    if (Possibility(template.opportunity) == null)
                        Debug.Log($"[PatronCreator] Покровители.json: {template.faction}/{template.responsibilities}: обязательная возможность «{template.opportunity}» описана только в шаблоне.");
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"[PatronCreator] Загрузка справочников: {e}");
            }
            return UniTask.CompletedTask;
        }

        private static List<T> Read<T>(string file, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var path = Path.Combine(Application.streamingAssetsPath, "Покровитель", file);
            try
            {
                var result = JsonConvert.DeserializeObject<DataList<T>>(File.ReadAllText(path));
                if (result?.data == null) throw new InvalidDataException("Не найден массив data.");
                Debug.Log($"[PatronCreator] {file}: загружено {result.data.Count} записей.");
                return result.data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[PatronCreator] Ошибка файла {path}: {e}");
                throw;
            }
        }

        private static void Add(Dictionary<string, PatronOption> target, PatronOption item, string file)
        {
            if (string.IsNullOrWhiteSpace(item?.name)) return;
            if (!target.TryAdd(item.name.Trim(), item))
                Debug.LogWarning($"[PatronCreator] {file}: повтор записи «{item.name}».");
        }
    }
}
