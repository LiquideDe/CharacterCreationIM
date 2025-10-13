using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Zenject;
using static UnityEngine.UI.Image;

namespace CharacterCreation
{
    public class OriginPresenter : ICharacterPresenter
    {
        private readonly Subject<Character> _nextClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        private readonly AudioManager _audioManager;
        private readonly OriginView _view;
        private readonly List<IDisposable> _subscriptions = new();
        [Inject] private OriginCreator _originCreator = null;
        [Inject] private EquipmentParser _equipmentParser = null;
        private OriginData _currentOrigin;
        private int _currentOriginIndex = -1;
        private Character _character;

        public OriginPresenter(AudioManager audioManager, OriginView characteristicView)
        {
            _audioManager = audioManager;
            _view = characteristicView;
        }

        public void Initialize()
        {
            _subscriptions.Add(
                _view.OnNextButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    SetOriginAndGoNext();
                })
            );

            _subscriptions.Add(
                _view.OnNextItemButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    NextOrigin();
                })
            );

            _subscriptions.Add(
                _view.OnPrevItemButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    PrevOrigin();
                })
            );

            _subscriptions.Add(
                _view.OnRandomButtonClick.Subscribe(_ =>
                {
                    _audioManager.PlayClick();
                    RandomOrigin();
                })
            );

            NextOrigin();
        }

        public void SetCharacter(Character character) => _character = character;

        public void Dispose()
        {
            _nextClicked.Dispose();
            foreach (var sub in _subscriptions)
                sub.Dispose();
            _subscriptions.Clear();
        }      

        private void NextOrigin()
        {
            var backgrounds = _originCreator.Backgrounds;
            if (backgrounds == null || backgrounds.Count == 0)
                return;

            // Переход к следующему индексу по кругу
            _currentOriginIndex = (_currentOriginIndex + 1) % backgrounds.Count;
            _currentOrigin = backgrounds[_currentOriginIndex];
            SetOrigin();
        }

        private void PrevOrigin()
        {
            var backgrounds = _originCreator.Backgrounds;
            if (backgrounds == null || backgrounds.Count == 0)
                return;

            // Переход к предыдущему индексу по кругу
            _currentOriginIndex = (_currentOriginIndex - 1 + backgrounds.Count) % backgrounds.Count;
            _currentOrigin = backgrounds[_currentOriginIndex];
            SetOrigin();
        }

        private void SetOrigin(bool canChange = true)
        {            
            _view.SetSheet(_currentOrigin.name, _currentOrigin.description, canChange);
            _view.SetText("Бонусы:", false);
            _view.SetGaranted(_currentOrigin.fixed_bonus, "Характеристики:");
            _view.SetChoose(_currentOrigin.selectable_bonuses, 1);
            _view.SetText("Вы получаете следующее снаряжение:", false);
            foreach (var item in _currentOrigin.items)            
                _view.SetText(item, true);
            
        }

        private void RandomOrigin()
        {
            _audioManager.PlayClick();
            var random = new System.Random();
            var chislo = random.Next(1, 101);
            _currentOrigin = _originCreator.GetByRoll(chislo);
            _character.Experience.Value.experiencePoints += 25;
            SetOrigin(canChange: false);
        }

        private void SetOriginAndGoNext()
        {            
            ApplyBonusesCharacterictis(_character, _currentOrigin.fixed_bonus);
            var selected = _view.GetGarantedCharacteristics();
            foreach(var item in selected)                
                if(item is ChooseCharacteristicView choose)
                    if (choose.IsSelected)
                    {
                        ApplyBonusesCharacterictis(_character, new Dictionary<string, int> { { choose.Characteristic.Name, choose.Characteristic.Level } });
                        break;
                    }            

            foreach (var item in _currentOrigin.items)
                _character.Equipments.Add(_equipmentParser.TryGetEquipment(item));
            _character.Origin.Value = _currentOrigin.name;
            _view.HideAndDestroyToLeft();
            _nextClicked?.OnNext(_character);
        }

        private void ApplyBonusesCharacterictis(Character character, Dictionary<string, int> bonuses)
        {
            if (bonuses == null || bonuses.Count == 0) return;

            foreach (var kvp in bonuses)
            {
                var name = kvp.Key;
                var delta = kvp.Value;

                var ch = character.Characteristics
                    .FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));

                if (ch == null)
                {
                    Debug.LogError($"Characteristic {name} == null");
                }

                ch.PlusLevel(delta);
            }
        }
    }
}

