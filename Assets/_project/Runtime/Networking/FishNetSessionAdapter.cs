using System;
using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;

namespace CharacterCreation.Vtt.Networking
{
    public sealed class FishNetSessionAdapter : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;

        public event Action<string> ClientConnectionStateChanged;

        private void Awake()
        {
            if (networkManager != null)
                networkManager.ClientManager.OnClientConnectionState += HandleClientConnectionState;
        }

        private void OnDestroy()
        {
            if (networkManager != null)
                networkManager.ClientManager.OnClientConnectionState -= HandleClientConnectionState;
        }

        public bool StartHost(ushort port)
        {
            if (!HasNetworkManager()) return false;
            var serverStarted = networkManager.ServerManager.StartConnection(port);
            var clientStarted = serverStarted && networkManager.ClientManager.StartConnection("127.0.0.1", port);
            Debug.Log($"[FishNetSessionAdapter.StartHost] Server:{serverStarted}; client:{clientStarted}; port:{port}.", this);
            return clientStarted;
        }

        public bool StartClient(string host, ushort port)
        {
            if (!HasNetworkManager()) return false;
            var started = networkManager.ClientManager.StartConnection(host, port);
            Debug.Log($"[FishNetSessionAdapter.StartClient] Client start requested on port:{port}.", this);
            return started;
        }

        private bool HasNetworkManager()
        {
            if (networkManager != null) return true;
            Debug.LogError("[FishNetSessionAdapter] NetworkManager is not assigned.", this);
            return false;
        }

        private void HandleClientConnectionState(ClientConnectionStateArgs args)
        {
            Debug.Log($"[FishNetSessionAdapter.ClientState] {args.ConnectionState}.", this);
            ClientConnectionStateChanged?.Invoke(args.ConnectionState.ToString());
        }
    }
}
