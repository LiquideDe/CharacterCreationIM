using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation
{
    public class EquipmentEntryView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private Button _buttonRemove;

        public Observable<Unit> OnRemoveClick => _buttonRemove.OnClickAsObservable();

        public void SetName(string name)
        {
            _textName.text = name;
        }
    }
}
