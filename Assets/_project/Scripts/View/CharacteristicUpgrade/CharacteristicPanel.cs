using R3;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation
{
    public class CharacteristicPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textName;
        [field: SerializeField] public TextMeshProUGUI TextAmount;
        [field: SerializeField] public TextMeshProUGUI HelpText;
        [SerializeField] private Button _buttonUpgrade;

        public Observable<Unit> OnUpgradeButtonClick => _buttonUpgrade.OnClickAsObservable();

        public void SetName(string name) => _textName.text = name;
    }
}

