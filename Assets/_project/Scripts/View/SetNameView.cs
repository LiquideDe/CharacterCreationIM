using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation
{
    public class SetNameView : ViewBase
    {
        [field: SerializeField] public TMP_InputField InputfieldName;
        [field: SerializeField] public TMP_InputField InputfieldHeight;
        [field: SerializeField] public TMP_InputField InputfieldWeight;
        [field: SerializeField] public Toggle ToggleRightHand;
        [SerializeField] private Button _buttonNext;
        public Observable<Unit> OnButtonNextClick => _buttonNext.OnClickAsObservable();

        private void Start()
        {
            Show();
        }
    }
}

