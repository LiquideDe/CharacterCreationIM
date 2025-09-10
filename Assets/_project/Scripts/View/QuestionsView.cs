using R3;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation
{
    public class QuestionsView : TargetView
    {
        [SerializeField] private Button _buttonSkip;

        public Observable<Unit> OnSkipButtonClick => _buttonSkip.OnClickAsObservable();
    }
}

