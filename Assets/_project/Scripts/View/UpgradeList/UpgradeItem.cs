using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation
{
    public class UpgradeItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Button _cancelButton;

        public void SetUpgrade(string upgradeName, int cost, Action cancel)
        {
            _nameText.text = upgradeName;
            _costText.text = $"Стоимость: {cost} ОО";
            _cancelButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.AddListener(() => cancel?.Invoke());
        }
    }
}
