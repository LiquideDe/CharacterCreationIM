using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation
{
    public class CharacteristicEditPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private TextMeshProUGUI _textValue;
        [SerializeField] private Button _buttonPlus;
        [SerializeField] private Button _buttonMinus;

        public Observable<Unit> OnPlusClick => _buttonPlus.OnClickAsObservable();
        public Observable<Unit> OnMinusClick => _buttonMinus.OnClickAsObservable();

        public void SetName(string name) => _textName.text = name;
        public void SetValue(int value) => _textValue.text = value.ToString();
    }
}
