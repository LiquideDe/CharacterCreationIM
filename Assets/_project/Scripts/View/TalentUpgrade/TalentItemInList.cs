using R3;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation
{
    public class TalentItemInList : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private Button _button;
        public string TalentName { get; private set; }
        public int Index { get; internal set; }

        // R3: поток кликов ячейки, публикует текущее имя таланта
        public readonly Subject<string> Clicked = new Subject<string>();

        void Awake()
        {
            _button.onClick
                .AsObservable()
                .Subscribe(_ => Clicked.OnNext(TalentName))
                .AddTo(gameObject);
        }

        public void Bind(string talentName)
        {
            TalentName = talentName;
            _textName.text = talentName;
        }
    }
}

