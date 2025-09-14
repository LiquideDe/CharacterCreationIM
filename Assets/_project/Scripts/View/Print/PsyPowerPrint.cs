using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CharacterCreation
{
    public class PsyPowerPrint : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private TextMeshProUGUI _textWarpCharge;
        [SerializeField] private TextMeshProUGUI _textCheck;
        [SerializeField] private TextMeshProUGUI _textRange;
        [SerializeField] private TextMeshProUGUI _textTarget;
        [SerializeField] private TextMeshProUGUI _textDuration;

        public bool IsEmpty { get; set; } = true;

        public void SetPsyPower(PsyData psyData, int total)
        {
            IsEmpty = false;
            _textName.text = psyData.name;
            _textWarpCharge.text = psyData.warpCharge.ToString();
            _textRange.text = psyData.range.ToString();
            _textTarget.text = psyData.target.ToString();
            _textDuration.text = psyData.duration.ToString();
            _textCheck.text = total.ToString();
        }
    }
}

