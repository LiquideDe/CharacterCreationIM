using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class SkillUpgradePresenter : ICharacterPresenter
    {
        [Inject] private SkillCreator _skillCreator;
        private readonly Subject<Character> _nextClicked = new();
        private readonly Subject<Character> _prevClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        public Observable<Character> PrevClicked => _prevClicked;
        private readonly AudioManager _audioManager;
        private SkillUpgradeView _view;
        private readonly CompositeDisposable _cd = new CompositeDisposable();
        private Character _character;
        private List<SkillData> _skillDatas = new List<SkillData>();
        private List<SpecializationData> _specializationDatas = new List<SpecializationData>();

        public SkillUpgradePresenter(AudioManager audioManager, SkillUpgradeView view)
        {
            _audioManager = audioManager;
            _view = view;
        }

        public void SetCharacter(Character character)
        {
            _character = character;
            SetSkills();
            InitialSpecializations();
        }

        private void SetSkills()
        {
            foreach (var item in _character.Skills)            
                _skillDatas.Add(item);

            var list = _skillCreator.Skills.ToList();
            foreach (var item in list)            
                if(IsSkillUnUsed(item))
                    _skillDatas.Add(new SkillData() 
                    { name = item.name, characteristic = item.characteristic, description = item.description, level = item.level});
            

            _view.SetSkills(_skillDatas);
            _view.SetExperience(_character.Experience.Value.experiencePoints);
        }

        private void InitialSpecializations()
        {
            foreach (var item in _character.Specializations)
                _specializationDatas.Add(item);

            var list = _skillCreator.Specializations.ToList();
            foreach (var item in list)
            if(IsSpecUnUsed(item))
                    _specializationDatas.Add(new SpecializationData() { name = item.name, description = item.description, level = item.level
                    , lvlRequireSkill = item.lvlRequireSkill, requireTalent = item.requireTalent, requireSkill = item.requireSkill,
                     skill = item.skill, specialSpecialization = item.specialSpecialization});

            SetSpecializations(_skillDatas[0]);
        }

        private void SetSpecializations(string nameSkill)
        {
            _audioManager.PlayClick();
            var skill = _skillDatas.Where(skill => skill.name == nameSkill).First();
            if (skill != null)
                SetSpecializations(skill);
            else
                Debug.LogAssertion($"Skill is null");
        }

        private void SetSpecializations(SkillData skillData)
        {
            var list = _specializationDatas.Where(spec => spec.skill == skillData.name).ToList();
            _view.SetSpecializations(skillData, list);
        }

        private bool IsSkillUnUsed(SkillData skillData)
        {
            foreach (var item in _skillDatas)            
                if (string.Compare(item.name, skillData.name, true) == 0)
                    return false;

            return true;            
        }

        private bool IsSpecUnUsed(SpecializationData specData)
        {
            foreach (var item in _specializationDatas)
                if (string.Compare(item.name, specData.name, true) == 0)
                    return false;

            return true;
        }

        public void Initialize()
        {
            _view.NewSpecialization.Subscribe(spec => { _specializationDatas.Add(spec); } ).AddTo(_cd);
            _view.OnButtonCancelClick.Subscribe(_ => { CancelUpgrade(); } ).AddTo(_cd);
            _view.OnButtonNextClick.Subscribe(_ => { GoNext(); } ).AddTo(_cd);
            _view.OnButtonPrevClick.Subscribe(_ => { ReturnToCharacteristics(); } ).AddTo(_cd);
            _view.UpgradeSkill.Subscribe(skill => { UpgradeSkill(skill); } ).AddTo(_cd);
            _view.UpgradeSpecialization.Subscribe(spec => { UpgradeSpecialization(spec); } ).AddTo(_cd);
            _view.ShowSpecialization.Subscribe(skill => { SetSpecializations(skill); }).AddTo(_cd);
        }

        private void UpgradeSkill(SkillData skill)
        {
            if(skill.level < 5)
            {
                var cmd = new UpgradeSkillCommand(_character, skill, 1, (skill.level + 1) * 50);
                var ok = _character.CharacteristicHistory.Do(cmd);
                if (!ok)
                    _audioManager.PlayError();
                else
                {
                    _view.SetExperience(_character.Experience.Value.experiencePoints);
                    _audioManager.PlayClick();
                }
            }
            
        }

        private void GoNext()
        {
            PrepareToGoAnywhere();
            _nextClicked.OnNext(_character);
            _view.HideAndDestroyToLeft();
        }

        private void ReturnToCharacteristics()
        {
            PrepareToGoAnywhere();
            _prevClicked.OnNext(_character);
            _view.HideAndDestroyToRight();
        }

        private void PrepareToGoAnywhere()
        {
            CheckNewSkills();
            CheckNewSpecializations();
        }

        private void CheckNewSkills()
        {
            foreach(var skill in _skillDatas)
            {
                if (_character.Skills.Contains(skill))
                    continue;

                if(skill.level > 0)
                    _character.Skills.Add(skill);
            }
        }

        private void CheckNewSpecializations()
        {
            foreach (var item in _specializationDatas)
            {
                if(_character.Specializations.Contains(item))
                    continue;

                if(item.level > 0)
                    _character.Specializations.Add(item);
            }
        }

        public void Dispose()
        {
            _cd.Dispose();
        }

        private void CancelUpgrade()
        {
            _audioManager.PlayClick();
            _character.CharacteristicHistory.Undo();
            _view.SetExperience(_character.Experience.Value.experiencePoints);
        }

        private void UpgradeSpecialization(SpecializationData spec)
        {
            if(IsRequireTalent(spec.requireTalent) && IsRequireSkill(spec.skill, spec.lvlRequireSkill) && spec.level < 5)
            {
                var cmd = new UpgradeSpecializationCommand(_character, spec, 1, (spec.level + 1) * 50);
                var ok = _character.CharacteristicHistory.Do(cmd);
                if (!ok)
                    _audioManager.PlayError();
                else
                {
                    _view.SetExperience(_character.Experience.Value.experiencePoints);
                    _audioManager.PlayClick();
                }
            }
        }

        private bool IsRequireTalent(string nameTalent)
        {
            if (string.IsNullOrEmpty(nameTalent))
                return true;

            var talent = _character.Talents.Where(talent => talent.name == nameTalent).First();

            return talent != null;
        }

        private bool IsRequireSkill(string nameSkill, int levelSkill)
        {
            if(levelSkill == 0)
                return true;

            var skill = _skillDatas.Where(skill => skill.name == nameSkill).First();
            if(skill.level >= levelSkill)
                return true;
            return false;
        }

    }

    public sealed class UpgradeSkillCommand : IGameCommand
    {
        private readonly Character _character;
        private readonly SkillData _skill;
        private readonly int _delta;         // обычно 1
        private readonly int _xpCost;        // сколько списываем
        private int _prevLevel;
        private int _prevXP;
        private int _prevExpSpent;
        private bool _applied;

        public UpgradeSkillCommand(Character player, SkillData ch, int delta, int xpCost)
        {
            _character = player;
            _skill = ch;
            _delta = delta;
            _xpCost = xpCost;
        }

        public bool Execute()
        {
            if (_character.Experience.Value.experiencePoints < _xpCost) return false;

            _prevLevel = _skill.level;
            _prevXP = _character.Experience.Value.experiencePoints;
            _prevExpSpent = _character.Experience.Value.experienceSpent;
            _character.Experience.Value.experiencePoints -= _xpCost;
            _character.Experience.Value.experienceSpent += _xpCost;
            _skill.PlusLevel(_delta);

            _applied = true;
            return true;
        }

        public void Undo()
        {
            if (!_applied) return;
            _skill.level = _prevLevel;
            _skill.PlusLevel(0); //Чтобы тригернуть подписку
            _character.Experience.Value.experiencePoints = _prevXP;
            _character.Experience.Value.experienceSpent = _prevExpSpent;
            _applied = false;
        }
    }

    public sealed class UpgradeSpecializationCommand : IGameCommand
    {
        private readonly Character _character;
        private readonly SpecializationData _specialization;
        private readonly int _delta;         // обычно 1
        private readonly int _xpCost;        // сколько списываем
        private int _prevLevel;
        private int _prevXP;
        private int _prevExpSpent;
        private bool _applied;

        public UpgradeSpecializationCommand(Character player, SpecializationData ch, int delta, int xpCost)
        {
            _character = player;
            _specialization = ch;
            _delta = delta;
            _xpCost = xpCost;
        }

        public bool Execute()
        {
            if (_character.Experience.Value.experiencePoints < _xpCost) return false;

            _prevLevel = _specialization.level;
            _prevXP = _character.Experience.Value.experiencePoints;
            _prevExpSpent = _character.Experience.Value.experienceSpent;
            _character.Experience.Value.experiencePoints -= _xpCost;
            _character.Experience.Value.experienceSpent += _xpCost;
            _specialization.PlusLevel(_delta);

            _applied = true;
            return true;
        }

        public void Undo()
        {
            if (!_applied) return;
            _specialization.level = _prevLevel;
            _specialization.PlusLevel(0); //Чтобы тригернуть подписку
            _character.Experience.Value.experiencePoints = _prevXP;
            _character.Experience.Value.experienceSpent = _prevExpSpent;
            _applied = false;
        }
    }
}

