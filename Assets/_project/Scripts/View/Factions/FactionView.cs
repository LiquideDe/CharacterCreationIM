using R3;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CharacterCreation.Background
{
    public class FactionView : CreateCharacterSheetView
    {        
        [SerializeField] private Button _chooseTemplateButton;       

        public Observable<Unit> OnChooseTemplateButtonClick => _chooseTemplateButton.OnClickAsObservable();   
        
    }
}

