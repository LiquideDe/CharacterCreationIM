using Newtonsoft.Json;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation
{
    public class TalentUpgradeView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI _textExperience;
        [SerializeField] private TextMeshProUGUI _textNameTalent;
        [SerializeField] private TextMeshProUGUI _textDescriptionTalent;
        [SerializeField] private VirtualListView _virtualListView;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _prevButton;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Toggle _toggleAvailable;
        private CompositeDisposable _cd = new CompositeDisposable();
        private CompositeDisposable _listDisposables = new CompositeDisposable();
        private Subject<string> _showTalentClick = new Subject<string> ();
        public Observable<string> ShowTalentClicked => _showTalentClick;
        public Observable<bool> ToggleShowAvailableClicked => _toggleAvailable.OnValueChangedAsObservable();
        public Observable<Unit> OnButtonBuyClick => _buyButton.OnClickAsObservable();
        public Observable<Unit> OnButtonNextClick => _nextButton.OnClickAsObservable();
        public Observable<Unit> OnButtonPrevClick => _prevButton.OnClickAsObservable();
        public Observable<Unit> OnButtonCancelClick => _cancelButton.OnClickAsObservable();

        private void Start()
        {
            Show();

            _buyButton.OnClickAsObservable().Subscribe(_ => ClearTexts()).AddTo(_cd);
        }

        public void SetTalents(List<string> names)
        {
            _listDisposables?.Clear();
            _virtualListView.SetNames(names);
            _virtualListView.ItemClicked
            .Subscribe(t => _showTalentClick.OnNext(t.name))
            .AddTo(_listDisposables);
        }

        public void ShowTalent(TalentData talent, bool isAvailable)
        {
            _textNameTalent.text = talent.name;
            _textDescriptionTalent.text = talent.description;
            if(talent.requirements != null)
            {
                foreach (var require in talent.requirements)
                {
                    _textDescriptionTalent.text += ParseRequire(require);
                    _textDescriptionTalent.text += $"\n";
                }
            }
            if (talent.isMultiple)
                _textDescriptionTalent.text += $"Можно брать несколько раз. Максимум {talent.maxMultiple} раз. \n";
            if (talent.character_creation_only)
                _textDescriptionTalent.text += $"Можно взять только при создании персонажа";
            _buyButton.gameObject.SetActive(isAvailable);
        }

        private string ParseRequire(TalentRequirement requirement)
        {
            string text = string.Empty;
            switch (requirement.type)
            {
                case "specialization_improvement":
                     return text = $"Требуется прокачка специализации {requirement.specialization} на {requirement.amount} пункта.";

                case "skill_improvement":
                    return text = $"Требуется прокачка навыка {requirement.skill} на {requirement.amount} пункта.";

                case "attribute_min":
                    return text = $"Требуется развитие характеристики {requirement.attribute} минимум на {requirement.value}.";

                case "no_improvement":
                    return text = $"Не должен быть прокачен навык {requirement.skill}";

                case "requirement_talent":
                    foreach (var item in requirement.talents)
                        text += $"Требуется талант {item}. ";
                    return text;

                case "exclusive_with":
                    return text = $"Не совместим с навыком {requirement.talents[0]}";

                default:
                    return text = $"Не нашли такого ключа {requirement.type}";
            }
        }

        private void ClearTexts()
        {
            _textNameTalent.text = string.Empty;
            _textDescriptionTalent.text = string.Empty;
            _buyButton.gameObject.SetActive(false);
        }

        internal void SetExperience(int experiencePoints)
        {
            _textExperience.text = $"Опыт: {experiencePoints}";
        }

        private void OnDestroy()
        {
            _cd.Dispose();
        }
    }
}

