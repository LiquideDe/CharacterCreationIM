using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CharacterCreation
{
    public class GarantedCharacteristic : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textname;
        [SerializeField] private TextMeshProUGUI textAmount;
        public Characteristic Characteristic { get; private set; }
        public virtual void SetCharacteristic(Characteristic characteristic)
        {
            Characteristic = characteristic;
            textname.text = characteristic.Name;
            textAmount.text = characteristic.Level.ToString();
        }
    }
}

