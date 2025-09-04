using CharacterCreation.Background;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static UnityEditor.Progress;

namespace CharacterCreation
{
    public class RolePresenter : ICharacterPresenter
    {
        private readonly Subject<Character> _nextClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        private readonly AudioManager _audioManager;
        private readonly FactionView _view;
        private readonly List<IDisposable> _subscriptions = new();
        [Inject] private SkillCreator _skillCreator;
        [Inject] private FinderData _finderData;
        [Inject] private EquipmentParser _equipmentParser;
        [Inject] private RoleCreator _roleCreator;
        private RoleData _currentRole;
        private int _currentRoleIndex = -1;
        private Character _character;

        public RolePresenter(AudioManager audioManager, FactionView view)
        {
            _view = view;
            _audioManager = audioManager;            
        }

        public void Dispose()
        {
            
        }

        public void Initialize()
        {
            _subscriptions.Add(
                _view.OnNextButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    SetRoleAndGoNext();
                })
            );

            _subscriptions.Add(
                _view.OnNextItemButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    NextRole();
                })
            );

            _subscriptions.Add(
                _view.OnPrevItemButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    PrevRole();
                })
            );

            _subscriptions.Add(
                _view.OnRandomButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    RandomRole();
                })
            );

            NextRole();
        }

        public void SetCharacter(Character character)
        {
            _character = character;
        }

        private void RandomRole()
        {

        }

        private void NextRole()
        {
            var roles = _roleCreator.Roles;
            if (roles == null || roles.Count == 0)
                return;

            _currentRoleIndex = (_currentRoleIndex - 1 + roles.Count) % roles.Count;
            _currentRole = roles[_currentRoleIndex];

            ShowRole();
        }

        private void PrevRole()
        {
            var roles = _roleCreator.Roles;
            if (roles == null || roles.Count == 0)
                return;

            _currentRoleIndex = (_currentRoleIndex + 1) % roles.Count;
            _currentRole = roles[_currentRoleIndex];

            ShowRole();
        }

        private void ShowRole()
        {
            _view.SetSheet(_currentRole.roleName, _currentRole.description);
            _view.SetText("Бонусы:");
            _view.SetList(_currentRole.talents, "Таланты:", _currentRole.amountTalents);
            _view.SetSkills(_currentRole.skills, 2);
            _view.SetSpecializations(_currentRole.specialization, _currentRole.specializationAmount, 1);
        }

        private void SetRoleAndGoNext()
        {

        }
    }
}

