using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation.Vtt.Presentation.NetworkSandbox
{
    public sealed class NetworkSandboxView : MonoBehaviour
    {
        [Header("Actions")]
        [SerializeField] private Button createRoomButton;
        [SerializeField] private Button connectButton;
        [SerializeField] private Button backButton;

        [Header("Connection input")]
        [SerializeField] private TMP_InputField masterAddressInput;
        [SerializeField] private TMP_InputField portInput;
        [SerializeField] private TMP_InputField inviteCodeInput;

        [Header("Output")]
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text participantsText;

        private readonly CompositeDisposable _subscriptions = new();
        private readonly Subject<Unit> _createRoomRequested = new();
        private readonly Subject<NetworkSandboxConnectionInput> _connectionRequested = new();
        private readonly Subject<Unit> _backRequested = new();

        public Observable<Unit> CreateRoomRequested => _createRoomRequested;

        public Observable<NetworkSandboxConnectionInput> ConnectionRequested => _connectionRequested;

        public Observable<Unit> BackRequested => _backRequested;

        private void Awake()
        {
            SubscribeButton(createRoomButton, RequestRoomCreation, nameof(createRoomButton));
            SubscribeButton(connectButton, RequestConnection, nameof(connectButton));
            SubscribeButton(backButton, RequestBackNavigation, nameof(backButton));
        }

        private void OnDestroy()
        {
            _subscriptions.Dispose();
            _createRoomRequested.Dispose();
            _connectionRequested.Dispose();
            _backRequested.Dispose();
        }

        public void SetStatus(string message)
        {
            if (statusText == null)
            {
                Debug.LogError("[NetworkSandboxView.SetStatus] Status text is not assigned.", this);
                return;
            }

            statusText.text = message ?? string.Empty;
        }

        public void SetParticipants(string message)
        {
            if (participantsText == null)
            {
                Debug.LogWarning("[NetworkSandboxView.SetParticipants] Participants text is not assigned.", this);
                return;
            }

            participantsText.text = message ?? string.Empty;
        }

        private void RequestRoomCreation()
        {
            Debug.Log("[NetworkSandboxView.RequestRoomCreation] Room creation requested.", this);
            _createRoomRequested.OnNext(Unit.Default);
        }

        private void RequestConnection()
        {
            var input = new NetworkSandboxConnectionInput(
                ReadInput(masterAddressInput, nameof(masterAddressInput)),
                ReadInput(portInput, nameof(portInput)),
                ReadInput(inviteCodeInput, nameof(inviteCodeInput)));

            Debug.Log("[NetworkSandboxView.RequestConnection] Connection requested.", this);
            _connectionRequested.OnNext(input);
        }

        private void RequestBackNavigation()
        {
            Debug.Log("[NetworkSandboxView.RequestBackNavigation] Back navigation requested.", this);
            _backRequested.OnNext(Unit.Default);
        }

        private void SubscribeButton(Button button, Action onClick, string buttonName)
        {
            if (button == null)
            {
                Debug.LogError($"[NetworkSandboxView.SubscribeButton] Button '{buttonName}' is not assigned.", this);
                return;
            }

            _subscriptions.Add(button.OnClickAsObservable().Subscribe(_ => onClick()));
        }

        private string ReadInput(TMP_InputField input, string inputName)
        {
            if (input != null)
            {
                return input.text.Trim();
            }

            Debug.LogError($"[NetworkSandboxView.ReadInput] Input '{inputName}' is not assigned.", this);
            return string.Empty;
        }
    }
}
