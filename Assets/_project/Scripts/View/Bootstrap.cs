using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace CharacterCreation
{
    public class Bootstrap : MonoBehaviour
    {
        [Inject] private StartMediator _startMediator;
        [Inject] private List<IDataCreator> _creators;

        [SerializeField] private GameObject loadingCanvasPrefab;

        private async void Start()
        {
            // Создаём CanvasLoading
            var loadingCanvasObj = Instantiate(loadingCanvasPrefab);
            var loadingCanvas = loadingCanvasObj.GetComponent<LoadingCanvas>();

            // Запускаем загрузку всех креаторов
            var cts = new CancellationTokenSource();
            var loadTasks = _creators.Select(c => c.LoadAsync(cts.Token)).ToArray();

            // Ожидаем завершения всех загрузок
            await UniTask.WhenAll(loadTasks);

            // Удаляем CanvasLoading
            Destroy(loadingCanvasObj);

            // Показываем главное меню
            _startMediator.ShowMainMenu();
        }
    }
}

