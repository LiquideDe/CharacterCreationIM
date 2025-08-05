using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using R3;

namespace CharacterCreation
{
    public class MainMenuView : ViewBase
    {
        [Header("Кнопки меню")]
        [SerializeField] private Button createPlayerButton;
        [SerializeField] private Button createPatronButton;
        [SerializeField] private Button editCharacterButton;
        [SerializeField] private Button developCharacterButton;
        [SerializeField] private Button printCharacterButton;
        [SerializeField] private Button exitButton;

        public Observable<Unit> OnCreatePlayerClicked => createPlayerButton.OnClickAsObservable();
        public Observable<Unit> OnCreatePatronClicked => createPatronButton.OnClickAsObservable();
        public Observable<Unit> OnEditCharacterClicked => editCharacterButton.OnClickAsObservable();
        public Observable<Unit> OnDevelopCharacterClicked => developCharacterButton.OnClickAsObservable();
        public Observable<Unit> OnPrintCharacterClicked => printCharacterButton.OnClickAsObservable();
        public Observable<Unit> OnExitClicked => exitButton.OnClickAsObservable();

        private void Start()
        {
            Show();
        }

    }
}

