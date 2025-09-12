using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation
{
    public class SkillPanelUpgrade : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private TextMeshProUGUI _textHelp;
        [SerializeField] private Button _buttonShowSpecialization;
        [SerializeField] private Button _buttonUpgrade;
        [SerializeField] private List<Image> _circles;
        [SerializeField] private Sprite _activeSprite, _deactiveSprite;
        [SerializeField] private InfoButtonView _infoButton;
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        public Observable<Unit> OnShowSpecButtonClick => _buttonShowSpecialization.OnClickAsObservable();
        public Observable<Unit> OnUpgradeButtonClick => _buttonUpgrade.OnClickAsObservable();

        public void SetSkill(SkillData skillData)
        {
            _textName.text = skillData.name;
            SetLevelImage(skillData.level);
            _infoButton.SetSkill(skillData.name);
            skillData.LevelChanged.Subscribe(level => { SetLevelImage(level); SetHelp(level); }).AddTo(_compositeDisposable);
        }

        public void SetSpecialization(SpecializationData specializationData)
        {
            _buttonShowSpecialization.interactable = false;
            _textName.text = specializationData.name;
            SetLevelImage(specializationData.level);
            _infoButton.SetSkill(specializationData.name);
            specializationData.LevelChanged.Subscribe(level => { SetLevelImage(level); SetHelp(level); }).AddTo(_compositeDisposable);
        }

        public void SetLevelImage(int level)
        {
            foreach (var item in _circles)
                item.sprite = _deactiveSprite;

            for (int i = 0; i < level; i++)
                _circles[i].sprite = _activeSprite;
        }

        public void SetHelp(int level)
        {
            _textHelp.text = $"Стоимость прокачки {(level + 1)*50}";
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
        }

    }
}

