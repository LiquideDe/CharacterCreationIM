using TMPro;
using UnityEngine;
using Zenject;
using R3;
using System;

namespace CharacterCreation.Background
{
    public class SkillCounterInList : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text = null;
        public int Counter { get; set; }
        public int MaxChoose { get; set; }
        
        private string defaultText;
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

        [Inject] private AudioManager _audioManager;

        public void SetText(string text)
        {
            defaultText = text;
            UpdateText();
        }

        private void UpdateText()
        {
            _text.text = $"Осталось очков: {Counter}. {defaultText}";
        }

        public void SetSkill(SkillInListView skill)
        {            
            skill.OnPlusButtonClick.Subscribe( _ => 
            {
                PlusToSkill(skill);
            }).AddTo(_compositeDisposable);

            skill.OnMinusButtonClick.Subscribe(_ =>
            {
                MinusToSkill(skill);
            }).AddTo(_compositeDisposable);
        }

        private void PlusToSkill(SkillInListView skill)
        {
            if(Counter > 0)
            {
                if(skill.Level < MaxChoose)
                {
                    _audioManager.PlayClick();
                    skill.PlusLevel();
                    Counter--;
                    UpdateText();
                }
            }
            else
                _audioManager.PlayError();
        }

        private void MinusToSkill(SkillInListView skill)
        {
            if(skill.Level > 0)
            {
                _audioManager.PlayClick();
                skill.MinusLevel();
                Counter++;
                UpdateText();
            }
            else _audioManager.PlayError();
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
        }
    }
}

