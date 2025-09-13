using R3;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation
{
    public class UpgradePsyView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI _textExperience;
        [SerializeField] private TextMeshProUGUI _textNamePsy;
        [SerializeField] private TextMeshProUGUI _textNameSchool;
        [SerializeField] private TextMeshProUGUI _textDescriptionPsy;
        [SerializeField] private VirtualListView _virtualListView;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _nextSchoolButton;
        [SerializeField] private Button _prevButton;
        [SerializeField] private Button _prevSchoolButton;
        [SerializeField] private Button _buyButton;

        private CompositeDisposable _cd = new CompositeDisposable();
        private Subject<string> _showPsyClick = new Subject<string>();
        public Observable<string> ShowTalentClicked => _showPsyClick;
        public Observable<Unit> OnButtonBuyClick => _buyButton.OnClickAsObservable();
        public Observable<Unit> OnButtonNextClick => _nextButton.OnClickAsObservable();
        public Observable<Unit> OnButtonPrevClick => _prevButton.OnClickAsObservable();
        public Observable<Unit> OnButtonCancelClick => _cancelButton.OnClickAsObservable();

        private void Start()
        {
            Show();

            _buyButton.OnClickAsObservable().Subscribe(_ => ClearTexts()).AddTo(_cd);
        }

        public void SetPsyPowers(List<string> names)
        {
            _virtualListView.SetNames(names);
            _virtualListView.ItemClicked
            .Subscribe(t => _showPsyClick.OnNext(t.name))
            .AddTo(gameObject);
        }

        public void ShowPsy(PsyData psy)
        {
            _textNamePsy.text = psy.name;            
            if(psy.isLesser)
                _textDescriptionPsy.text = $"Малая психосила \n";
            _textDescriptionPsy.text += $"Цель {psy.target}. \n";
            _textDescriptionPsy.text += $"Дальность {psy.range}.\n";
            _textDescriptionPsy.text += $"Длительность {psy.duration}.\n";
            _textDescriptionPsy.text += $"Сложность {psy.testDifficulty}. \n";
            _textDescriptionPsy.text += $"Стоимость в варп зарядах {psy.warpCharge}\n\n";
            _textDescriptionPsy.text = psy.description;

        }

        private void ClearTexts()
        {
            _textNamePsy.text = string.Empty;
            _textDescriptionPsy.text = string.Empty;
            _buyButton.gameObject.SetActive(false);
        }

    }
}

