using TMPro;
using UnityEngine;

namespace CharacterCreation
{
    public class InfluenceEntryView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private TMP_InputField _inputValue;

        public void SetName(string name) => _textName.text = name;
        public void SetValue(int value) => _inputValue.text = value.ToString();
        public int GetValue()
        {
            if (int.TryParse(_inputValue.text, out var v)) return v;
            return 0;
        }

        public string Name => _textName.text;
    }
}
