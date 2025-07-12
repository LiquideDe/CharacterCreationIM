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
    public abstract class DataCreator
    {
        protected async UniTask LoadAndAddAsync<TList, TItem>(
            string fileName,
            List<TItem> targetList,
            CancellationToken cancellationToken,
            Func<TList, List<TItem>> extractList, string basePath)
        {
            string path = Path.Combine(basePath, fileName);
            /*
            
            if (File.Exists(path))
            {
                string json = await File.ReadAllTextAsync(path, cancellationToken);
                TList list = JsonUtility.FromJson<TList>(json);
                var data = extractList(list);
                if (data != null)
                    targetList.AddRange(data);
            }
            else
            {
                Debug.LogError($"Файл с {fileName} не найден: {path}");
            }*/

            if (!File.Exists(path))
            {
                Debug.LogError($"Файл не найден: {path}");
                return;
            }

            try
            {
                string json = await File.ReadAllTextAsync(path, cancellationToken);
                TList listContainer = JsonConvert.DeserializeObject<TList>(json);

                if (listContainer == null)
                {
                    Debug.LogError($"Не удалось десериализовать JSON в {typeof(TList).Name}");
                    return;
                }

                var extracted = extractList(listContainer);
                if (extracted != null)
                {
                    targetList.AddRange(extracted);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при загрузке файла {path}: {ex.Message}");
            }
        }
    }
}

