using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CharacterCreation
{
    public class ChooseCharacteristicView : GarantedCharacteristic
    {
        
        [field: SerializeField] public Toggle Toggle;
        [Inject] private AudioManager _audioManager;
        public bool IsSelected => Toggle.isOn;

        public void SetToggleGroup(ToggleGroup toggleGroup) => Toggle.group = toggleGroup;

        private void Start()
        {
            Toggle.onValueChanged.AddListener((bo) => _audioManager.PlayClick());
        }
    }
}

