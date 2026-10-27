using System;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CharacterCreation
{
    /// <summary>
    /// Scene navigation for the top-level game mode menu.
    /// Button events are subscribed through R3 in code.
    /// </summary>
    public sealed class GameModeMenu : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button characterEditorButton;
        [SerializeField] private Button onlineButton;
        [SerializeField] private Button exitButton;

        [Header("Scenes")]
        [SerializeField] private string characterEditorSceneName = "GameScene";
        [SerializeField] private string onlineSceneName = "OnlineScene";

        private readonly CompositeDisposable _subscriptions = new();

        private void Awake()
        {
            SubscribeButton(characterEditorButton, OpenCharacterEditor, nameof(characterEditorButton));
            SubscribeButton(onlineButton, OpenOnline, nameof(onlineButton));
            SubscribeButton(exitButton, ExitApplication, nameof(exitButton));
        }

        private void OnDestroy()
        {
            _subscriptions.Dispose();
        }

        private void OpenCharacterEditor()
        {
            LoadScene(characterEditorSceneName);
        }

        private void OpenOnline()
        {
            LoadScene(onlineSceneName);
        }

        private void ExitApplication()
        {
            Debug.Log("[GameModeMenu.ExitApplication] Exit requested.");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void SubscribeButton(Button button, Action onClick, string buttonName)
        {
            if (button == null)
            {
                Debug.LogError($"[GameModeMenu.SubscribeButton] Button '{buttonName}' is not assigned.", this);
                return;
            }

            _subscriptions.Add(button.OnClickAsObservable().Subscribe(_ => onClick()));
        }

        private static void LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("[GameModeMenu.LoadScene] Scene name is empty.");
                return;
            }

            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"[GameModeMenu.LoadScene] Scene '{sceneName}' is not included in Build Settings.");
                return;
            }

            Debug.Log($"[GameModeMenu.LoadScene] Loading scene '{sceneName}'.");
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
