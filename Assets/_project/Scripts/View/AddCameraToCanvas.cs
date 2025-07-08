using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class AddCameraToCanvas : MonoBehaviour
    {
        [Inject]  private Camera _camera = null;
        private Canvas _canvas;

        void Start()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.worldCamera = _camera;
        }
    }
}

