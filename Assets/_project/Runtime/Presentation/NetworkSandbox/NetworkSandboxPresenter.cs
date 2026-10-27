using System;
using R3;
using CharacterCreation.Vtt.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CharacterCreation.Vtt.Presentation.NetworkSandbox
{
    public sealed class NetworkSandboxPresenter : IDisposable
    {
        private readonly NetworkSandboxView _view;
        private readonly string _mainMenuSceneName;
        private readonly FishNetSessionAdapter _network;
        private readonly CompositeDisposable _subscriptions = new();

        public NetworkSandboxPresenter(NetworkSandboxView view, FishNetSessionAdapter network, string mainMenuSceneName)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _network = network ?? throw new ArgumentNullException(nameof(network));
            _mainMenuSceneName = mainMenuSceneName;
        }

        public void Initialize()
        {
            _network.ClientConnectionStateChanged += HandleClientConnectionState;
            _subscriptions.Add(_view.CreateRoomRequested.Subscribe(_ => ShowLocalHostState()));
            _subscriptions.Add(_view.ConnectionRequested.Subscribe(ValidateConnectionInput));
            _subscriptions.Add(_view.BackRequested.Subscribe(_ => ReturnToMainMenu()));

            _view.SetStatus("Sandbox ready. Network transport is not configured yet.");
            _view.SetParticipants("Players: none");
        }

        public void Dispose()
        {
            _network.ClientConnectionStateChanged -= HandleClientConnectionState;
            _subscriptions.Dispose();
        }

        private void HandleClientConnectionState(string state)
        {
            _view.SetStatus($"Client connection state: {state}.");
        }

        private void ShowLocalHostState()
        {
            var started = _network.StartHost(7777);
            _view.SetStatus(started ? "Host started on port 7777." : "Failed to start host. Check Console.");
            _view.SetParticipants(started ? "Players:\n- Master (local)" : "Players: none");
        }

        private void ValidateConnectionInput(NetworkSandboxConnectionInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Host))
            {
                _view.SetStatus("Enter the master address before connecting.");
                return;
            }

            if (!ushort.TryParse(input.Port, out var port) || port == 0)
            {
                _view.SetStatus("Enter a port from 1 to 65535.");
                return;
            }

            var started = _network.StartClient(input.Host, port);
            _view.SetStatus(started ? "Connection requested." : "Failed to start connection. Check Console.");
        }

        private void ReturnToMainMenu()
        {
            if (string.IsNullOrWhiteSpace(_mainMenuSceneName))
            {
                Debug.LogError("[NetworkSandboxPresenter.ReturnToMainMenu] Main menu scene name is empty.");
                return;
            }

            if (!Application.CanStreamedLevelBeLoaded(_mainMenuSceneName))
            {
                Debug.LogError($"[NetworkSandboxPresenter.ReturnToMainMenu] Scene '{_mainMenuSceneName}' is not included in Build Settings.");
                return;
            }

            Debug.Log($"[NetworkSandboxPresenter.ReturnToMainMenu] Loading scene '{_mainMenuSceneName}'.");
            SceneManager.LoadScene(_mainMenuSceneName, LoadSceneMode.Single);
        }
    }
}
