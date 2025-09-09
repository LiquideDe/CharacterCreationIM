using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation
{
    public class LookPrefab : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;
        [SerializeField] private TextMeshProUGUI _text;

        private AudioManager _audioManager;
        public string Text => _text.text;
        public bool IsOn => _toggle.isOn;

        public void SetToggleGroup(ToggleGroup toggleGroup) => _toggle.group = toggleGroup;  
        
        public void SetText(string text) => _text.text = text;

        public void SubscribeSound(AudioManager audioManager)
        {
            _audioManager = audioManager;
            _toggle.onValueChanged.AddListener(PlayClick);
        }

        private void PlayClick(bool val) => _audioManager.PlayClick();

        private void OnDestroy()
        {
            _toggle.onValueChanged.RemoveListener(PlayClick);
        }
    }
}

