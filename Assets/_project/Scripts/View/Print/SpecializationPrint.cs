using TMPro;
using UnityEngine;

namespace CharacterCreation
{
    public class SpecializationPrint : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private TextMeshProUGUI _skillName;
        [SerializeField] private TextMeshProUGUI _upgrades;
        [SerializeField] private TextMeshProUGUI _totalAmount;
        public bool IsEmpty = true;

        public void SetSpec(SpecializationData spec, int total)
        {
            _textName.text = spec.name;
            _skillName.text = spec.skill;
            _upgrades.text = spec.level.ToString();
            _totalAmount.text = total.ToString();
            IsEmpty = false;
        }
    }
}

