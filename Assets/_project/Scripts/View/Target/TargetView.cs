using R3;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation
{
    public class TargetView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [field: SerializeField] public TMP_InputField InputField;
        [SerializeField] private Button _buttonNext;

        public Observable<Unit> OnNextButtonClick => _buttonNext.OnClickAsObservable();

        private void Start()
        {
            Show();
        }

        public void SetText(string title, string description)
        {
            _titleText.text = title;
            _descriptionText.text = description;
            HideAndShow();
            InputField.gameObject.SetActive(true);
            InputField.text = string.Empty;
        }
    }
}

