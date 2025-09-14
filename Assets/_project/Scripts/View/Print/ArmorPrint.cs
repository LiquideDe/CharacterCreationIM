using TMPro;
using UnityEngine;

namespace CharacterCreation
{
    public class ArmorPrint : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private TextMeshProUGUI _zones;
        [SerializeField] private TextMeshProUGUI _armorPoints;
        [SerializeField] private TextMeshProUGUI _weight;
        [SerializeField] private TextMeshProUGUI _properties;

        public bool IsEmpty { get; set; } = true;

        public void SetArmor(ArmorData armorData)
        {
            _textName.text = armorData.name;
            foreach (var item in armorData.protectionZones)            
                _zones.text += $"{item}, ";

            _armorPoints.text = armorData.armorPoints.ToString();
            _weight.text = armorData.weight.ToString();
            foreach (var item in armorData.properties)
                _properties.text += $"{item}, ";

            IsEmpty = false;
        }
    }
}

