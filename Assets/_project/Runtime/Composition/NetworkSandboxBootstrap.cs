using CharacterCreation.Vtt.Presentation.NetworkSandbox;
using CharacterCreation.Vtt.Networking;
using UnityEngine;

namespace CharacterCreation.Vtt.Composition
{
    public sealed class NetworkSandboxBootstrap : MonoBehaviour
    {
        [SerializeField] private NetworkSandboxView view;
        [SerializeField] private FishNetSessionAdapter network;
        [SerializeField] private string mainMenuSceneName = "GameModeMenuScene";

        private NetworkSandboxPresenter _presenter;

        private void Awake()
        {
            if (view == null || network == null)
            {
                Debug.LogError("[NetworkSandboxBootstrap.Awake] View or network adapter is not assigned.", this);
                enabled = false;
                return;
            }

            _presenter = new NetworkSandboxPresenter(view, network, mainMenuSceneName);
            _presenter.Initialize();
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
        }
    }
}
