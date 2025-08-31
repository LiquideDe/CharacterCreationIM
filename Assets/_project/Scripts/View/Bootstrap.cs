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
            var loadingCanvasObj = Instantiate(loadingCanvasPrefab);
            var loadingCanvas = loadingCanvasObj.GetComponent<LoadingCanvas>();

            var cts = new CancellationTokenSource();
            var loadTasks = _creators.Select(c => c.LoadAsync(cts.Token)).ToArray();
            await UniTask.WhenAll(loadTasks);

            Destroy(loadingCanvasObj);

            _startMediator.ShowMainMenu();
        }
    }
}

