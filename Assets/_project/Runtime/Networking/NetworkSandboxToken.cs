using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace CharacterCreation.Vtt.Networking
{
    public sealed class NetworkSandboxToken : NetworkBehaviour
    {
        [SerializeField] private Camera boardCamera;
        private readonly SyncVar<Vector3> _position = new();
        private bool _isServerActive;

        private void Awake()
        {
            _position.OnChange += ApplyPosition;
        }

        public override void OnStartServer()
        {
            _isServerActive = true;
            _position.Value = transform.position;
            Debug.Log($"[FIX:NetworkSandboxToken] Server initialized token '{name}' at {transform.position}.");
        }

        public override void OnStopServer()
        {
            _isServerActive = false;
            Debug.Log($"[FIX:NetworkSandboxToken] Server stopped token '{name}'.");
        }

        public override void OnStartClient()
        {
            Debug.Log($"[FIX:NetworkSandboxToken] Client initialized token '{name}'.");
        }

        private void Update()
        {
            if (!_isServerActive || !Input.GetMouseButton(0) || boardCamera == null)
                return;

            var screenPosition = Input.mousePosition;
            screenPosition.z = -boardCamera.transform.position.z;
            var worldPosition = boardCamera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = transform.position.z;
            _position.Value = worldPosition;
        }

        private void ApplyPosition(Vector3 previous, Vector3 next, bool asServer)
        {
            transform.position = next;
        }
    }
}
