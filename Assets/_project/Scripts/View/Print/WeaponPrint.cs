using TMPro;
using UnityEngine;

namespace CharacterCreation
{
    public class WeaponPrint : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private TextMeshProUGUI _textSpecialization;
        [SerializeField] private TextMeshProUGUI _textCheck;
        [SerializeField] private TextMeshProUGUI _textDamage;
        [SerializeField] private TextMeshProUGUI _textRange;
        [SerializeField] private TextMeshProUGUI _textClip;
        [SerializeField] private TextMeshProUGUI _textWeight;
        [SerializeField] private TextMeshProUGUI _textProperties;

        public bool IsEmpty { get; set; } = true;

        public void SetWeapon(MeleeWeaponData weaponData, int total)
        {
            IsEmpty = false;
            _textName.text = weaponData.name;
            _textSpecialization.text = $"{weaponData.specialization.skill}({weaponData.specialization.specialization})";
            _textCheck.text = total.ToString();
            _textDamage.text = weaponData.damage.ToString();
            _textWeight.text = weaponData.weight.ToString();
            foreach (var item in weaponData.properties)
                _textProperties.text += $"{item}, ";
        }

        public void SetWeapon(RangedWeaponData weaponData, int total)
        {
            _textRange.text = weaponData.range.ToString();
            _textClip.text = weaponData.clip.ToString();
            SetWeapon(weaponData as MeleeWeaponData, total);
        }
    }
}

