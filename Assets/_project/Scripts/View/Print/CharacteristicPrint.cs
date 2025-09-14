using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CharacterCreation
{
    public enum Characteristicname
    {
        WS, BS, Str, Tou, Ag, Int, Perc, WP, Fel
    }
    public class CharacteristicPrint : MonoBehaviour
    {
        [field: SerializeField] public Characteristicname Characteristicname;
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;

        public void Set(string text) => textMeshProUGUI.text = text;
    }
}

