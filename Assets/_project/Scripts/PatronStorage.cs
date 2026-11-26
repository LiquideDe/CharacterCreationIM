using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace CharacterCreation
{
    public sealed class PatronStorage
    {
        private static string Root => Path.Combine(Application.streamingAssetsPath, "Покровители");

        public IReadOnlyList<string> ListNames()
        {
            if (!Directory.Exists(Root)) return Array.Empty<string>();
            return Directory.GetDirectories(Root)
                .Select(Path.GetFileName)
                .Where(name => File.Exists(Path.Combine(Root, name, name + ".json")))
                .OrderBy(name => name, StringComparer.CurrentCultureIgnoreCase).ToArray();
        }

        public bool TryLoad(string name, out Patron patron)
        {
            patron = null;
            if (!ValidName(name, out var error))
            {
                Debug.LogWarning($"[PatronStorage] Некорректное имя '{name}': {error}");
                return false;
            }
            var path = Path.Combine(Root, name, name + ".json");
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[PatronStorage] Файл покровителя не найден: {path}");
                return false;
            }
            try
            {
                patron = JsonConvert.DeserializeObject<Patron>(File.ReadAllText(path));
                if (patron == null || !string.Equals(patron.Name, name, StringComparison.Ordinal))
                    throw new InvalidDataException("Имя в файле не совпадает с именем папки.");
                patron.Influence ??= new Dictionary<string, int>();
                patron.PossibilityVariants ??= new List<string>();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[PatronStorage] Ошибка чтения {path}: {e}");
                patron = null;
                return false;
            }
        }

        public bool TrySave(Patron patron, out string error)
        {
            if (patron == null)
            {
                error = "Данные покровителя отсутствуют.";
                return false;
            }
            if (!ValidName(patron.Name, out error)) return false;
            var path = Path.Combine(Root, patron.Name, patron.Name + ".json");
            if (File.Exists(path))
            {
                error = $"Покровитель «{patron.Name}» уже существует.";
                return false;
            }
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
                using var writer = new StreamWriter(stream);
                writer.Write(JsonConvert.SerializeObject(patron, Formatting.Indented));
                error = null;
                Debug.Log($"[PatronStorage] Сохранён покровитель: {path}");
                return true;
            }
            catch (Exception e)
            {
                error = $"Не удалось сохранить покровителя: {e.Message}";
                Debug.LogError($"[PatronStorage] Ошибка сохранения {path}: {e}");
                return false;
            }
        }

        private static bool ValidName(string name, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(name) || name != name.Trim() || name.EndsWith(".") ||
                name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || name.Contains('/') || name.Contains('\\') ||
                name == "." || name == "..")
            {
                error = "Имя пустое или содержит недопустимые символы.";
                return false;
            }
            return true;
        }
    }
}
