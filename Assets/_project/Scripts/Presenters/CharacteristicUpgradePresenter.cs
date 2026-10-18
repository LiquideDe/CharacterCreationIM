using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CharacterCreation
{
    public class CharacteristicUpgradePresenter : ICharacterPresenter
    {
        private readonly Subject<Character> _nextClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        private readonly AudioManager _audioManager;
        private CharacteristicUpgradeView _view;
        private readonly List<IDisposable> _subscriptions = new();
        private Character _character;
        private readonly LevelCostTable _costs = new();
        private readonly UpgradeItemsListPresenter _upgradeItemsListPresenter;

        public CharacteristicUpgradePresenter(AudioManager audioManager, CharacteristicUpgradeView view, UpgradeItemsListPresenter upgradeItemsListPresenter)
        {
            _audioManager = audioManager;
            _view = view;
            _upgradeItemsListPresenter = upgradeItemsListPresenter;
        }

        public void SetCharacter(Character character) 
        {
            _character = character;
            _view.SetExperience(_character.Experience.Value.experiencePoints);
            SetCharacteristics();
        }
        

        public void Initialize()
        {
            _subscriptions.Add(
                _view.OnButtonNextClick.Subscribe(_ =>
                {
                    GoToNext();
                })
            );

            _subscriptions.Add(
                _view.CharacteristicClicked.Subscribe(ch => { UpgradeCharacteristic(ch); }));
        }

        private void SetCharacteristics()
        {
            _view.SetCharacteristics(_character.Characteristics.ToList());
        }

        private void UpgradeCharacteristic(Characteristic ch)
        {
            var cmd = new UpgradeCharacteristicCommand(_character, ch, delta: 1, xpCost: _costs.GetCostForNextLevel(ch.Level));
            var ok = cmd.Execute();
            if (!ok)            
                _audioManager.PlayError();
            else
            {
                _view.SetExperience(_character.Experience.Value.experiencePoints);
                _audioManager.PlayClick();
                _upgradeItemsListPresenter.AddUpgrade($"Характеристика: {ch.Name}", _costs.GetCostForNextLevel(ch.Level - 1), cmd,
                    RefreshExperience);
            }   
        }

        private void GoToNext()
        {
            _audioManager.PlayClick();
            _view.HideAndDestroyToLeft();
            _nextClicked.OnNext(_character);
        }

        private void RefreshExperience()
        {
            if (_view != null)
                _view.SetExperience(_character.Experience.Value.experiencePoints);
        }

        public void Dispose()
        {
            _nextClicked.Dispose();
            foreach (var sub in _subscriptions)
                sub.Dispose();
            _subscriptions.Clear();
        }
    }

    public interface IGameCommand
    {
        bool Execute();
        void Undo();
    }

    public sealed class UpgradeCharacteristicCommand : IGameCommand
    {
        private readonly Character _character;
        private readonly Characteristic _ch;
        private readonly int _delta;         // обычно 1
        private readonly int _xpCost;        // сколько списываем
        private int _prevLevel;
        private bool _applied;

        public UpgradeCharacteristicCommand(Character player, Characteristic ch, int delta, int xpCost)
        {
            _character = player;
            _ch = ch;
            _delta = delta;
            _xpCost = xpCost;
        }

        public bool Execute()
        {
            if (_character.Experience.Value.experiencePoints < _xpCost) return false;

            _prevLevel = _ch.Level;
            _character.Experience.Value.experiencePoints -= _xpCost;
            _character.Experience.Value.experienceSpent += _xpCost;
            _ch.PlusLevel(_delta);

            _applied = true;
            return true;
        }

        public void Undo()
        {
            if (!_applied) return;
            _ch.Level = _prevLevel;
            _ch.PlusLevel(0);
            _character.Experience.Value.experiencePoints += _xpCost;
            _character.Experience.Value.experienceSpent -= _xpCost;
            _applied = false;
        }
    }

    public struct LevelCostBand
    {
        [Min(1)] public int FromLevel;     
        [Min(1)] public int ToLevel;       
        [Min(0)] public int CostPerLevel; 
    }

    public sealed class LevelCostTable
    {
        private List<LevelCostBand> _bands = new()
        {
            new LevelCostBand(){ FromLevel = 20, ToLevel = 25, CostPerLevel = 20},
            new LevelCostBand(){ FromLevel = 26, ToLevel = 30, CostPerLevel = 25},
            new LevelCostBand(){ FromLevel = 31, ToLevel = 35, CostPerLevel = 30},
            new LevelCostBand(){ FromLevel = 36, ToLevel = 40, CostPerLevel = 40},
            new LevelCostBand(){ FromLevel = 41, ToLevel = 45, CostPerLevel = 60},
            new LevelCostBand(){ FromLevel = 46, ToLevel = 50, CostPerLevel = 80},
            new LevelCostBand(){ FromLevel = 51, ToLevel = 55, CostPerLevel = 110},
            new LevelCostBand(){ FromLevel = 56, ToLevel = 60, CostPerLevel = 140},
            new LevelCostBand(){ FromLevel = 61, ToLevel = 65, CostPerLevel = 180},
            new LevelCostBand(){ FromLevel = 66, ToLevel = 70, CostPerLevel = 220},
            new LevelCostBand(){ FromLevel = 71, ToLevel = 75, CostPerLevel = 270},
            new LevelCostBand(){ FromLevel = 76, ToLevel = 80, CostPerLevel = 320}
        };
        private int[] _costByLevel;
        private int _maxLevel;

        public LevelCostTable()
        {
            RebuildCache();
        }

        public int GetCostForNextLevel(int currentLevel)
        {
            // Стоимость апгрейда currentLevel -> currentLevel+1
            int target = currentLevel + 1;
            if (target <= 0) return 0;
            if (_costByLevel != null && target <= _maxLevel) return _costByLevel[target];

            // fallback без кэша
            foreach (var b in _bands)
                if (target >= b.FromLevel && target <= b.ToLevel)
                    return b.CostPerLevel;

            // если не нашли — считаем 0 или бросаем? Лучше 0 и лог.
            Debug.LogWarning($"No cost band found for target level {target}");
            return 0;
        }

        public int GetTotalCost(int currentLevel, int levelsToGain)
        {
            if (levelsToGain <= 0) return 0;
            int total = 0;
            for (int i = 0; i < levelsToGain; i++)
            {
                total += GetCostForNextLevel(currentLevel + i);
            }
            return total;
        }

        private void RebuildCache()
        {
            if (_bands.Count == 0)
            {
                _costByLevel = null;
                _maxLevel = 0;
                return;
            }

            _maxLevel = 0;
            foreach (var b in _bands) _maxLevel = Mathf.Max(_maxLevel, b.ToLevel);

            _costByLevel = new int[_maxLevel + 1]; // индекс = уровень

            foreach (var b in _bands)
            {
                for (int lvl = b.FromLevel; lvl <= b.ToLevel; lvl++)
                    _costByLevel[lvl] = b.CostPerLevel;
            }
        }
    }
}

