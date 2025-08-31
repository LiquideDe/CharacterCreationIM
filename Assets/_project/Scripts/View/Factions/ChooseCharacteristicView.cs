using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CharacterCreation
{
    public class ChooseCharacteristicView : GarantedCharacteristic
    {
        
        [SerializeField] private Toggle toggle;
        [Inject] private AudioManager _audioManager;
        public bool IsSelected => toggle.isOn;

        public void SetToggleGroup(ToggleGroup toggleGroup) => toggle.group = toggleGroup;

        private void Start()
        {
            toggle.onValueChanged.AddListener((bo) => _audioManager.PlayClick());
        }
    }
}

