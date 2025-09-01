using R3;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation.Background
{
    public class FactionView : CreateCharacterSheetView
    {        
        [SerializeField] private Button _chooseTemplateButton;       

        public Observable<Unit> OnChooseTemplateButtonClick => _chooseTemplateButton.OnClickAsObservable();

    }
}

