using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CharacterCreation
{
    public class SkillUpgradeView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI _textExperience;
        [SerializeField] private Transform _contentSkills;
        [SerializeField] private Transform _contentSpecializations;
        [SerializeField] private Button _buttonNext;
        [SerializeField] private Button _buttonPrev;
        [SerializeField] private Button _buttonCancel;
        [SerializeField] private Button _buttonNewSpecialization;
        [Inject] private IFactory<SkillPanelUpgrade> _factorySkillPanel;
        [Inject] private IFactory<NewSpecializationPanel> _factoryNewSpecialization;
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private CompositeDisposable _specDisposable = new CompositeDisposable();
        private SkillData _currentSkill;
        private List<SkillPanelUpgrade> _specPanels = new List<SkillPanelUpgrade>();
        
        private readonly Subject<SpecializationData> _newSpec = new();
        private readonly Subject<string> _showSpecs = new();
        private readonly Subject<SkillData> _upgradeSkill = new();
        private readonly Subject<SpecializationData> _upgradeSpecialization = new();
        public Observable<Unit> OnButtonNextClick => _buttonNext.OnClickAsObservable();
        public Observable<Unit> OnButtonPrevClick => _buttonPrev.OnClickAsObservable();
        public Observable<Unit> OnButtonCancelClick => _buttonCancel.OnClickAsObservable();
        public Observable<SpecializationData> NewSpecialization => _newSpec;
        public Observable<string> ShowSpecialization => _showSpecs;
        public Observable<SkillData> UpgradeSkill => _upgradeSkill;
        public Observable<SpecializationData> UpgradeSpecialization => _upgradeSpecialization;

        private void Start()
        {
            Show();
            _buttonNewSpecialization.OnClickAsObservable().Subscribe(_ => { CreateSpecialization(); }).AddTo(_compositeDisposable);
        }

        public void SetSkills(List<SkillData> skills)
        {
            foreach (var item in skills)
            {
                var skillData = item;
                var skill = _factorySkillPanel.Create();
                skill.transform.SetParent(_contentSkills, false);
                skill.SetSkill(item);
                skill.SetHelp(item.level);
                skill.OnShowSpecButtonClick.Subscribe(_ => { _showSpecs.OnNext(skillData.name); }).AddTo(_compositeDisposable);
                skill.OnUpgradeButtonClick.Subscribe(_ => { _upgradeSkill.OnNext(skillData); }).AddTo(_compositeDisposable);
                skill.gameObject.SetActive(true);
            }
        }

        public void SetSpecializations(SkillData skill, List<SpecializationData> specializationDatas)
        {
            ClearList();
            _currentSkill = skill;
            foreach (var item in specializationDatas)
                AddSpecialization(item);   
        }

        private void CreateSpecialization()
        {
            var panelNewSpec = _factoryNewSpecialization.Create();
            panelNewSpec.SetSkill(_currentSkill.name);
            panelNewSpec.NextClicked.Take(1).Subscribe( spec  => { AddSpecialization(spec); _newSpec.OnNext(spec); });
        }

        private void AddSpecialization(SpecializationData specialization)
        {
            var specializationData = specialization;
            var spec = _factorySkillPanel.Create();
            spec.transform.SetParent(_contentSpecializations, false);
            spec.SetSpecialization(specialization);
            spec.SetHelp(specialization.level);
            spec.OnUpgradeButtonClick.Subscribe(_ => { _upgradeSpecialization.OnNext(specializationData); }).AddTo(_specDisposable);
            spec.gameObject.SetActive(true);
            _specPanels.Add(spec);
        }

        private void ClearList()
        {
            _specDisposable.Clear();
            foreach (var item in _specPanels)
                Destroy(item.gameObject);
            _specPanels.Clear();               
            
        }

        internal void SetExperience(int experiencePoints)
        {
            _textExperience.text = $"Опыт: {experiencePoints}";
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _specDisposable.Dispose();
        }
    }
}

