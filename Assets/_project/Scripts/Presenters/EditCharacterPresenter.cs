using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class EditCharacterPresenter : ICharacterPresenter
    {
        private readonly Subject<Character> _nextClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        private readonly AudioManager _audioManager;
        private readonly EditCharacterView _view;
        private readonly CompositeDisposable _cd = new CompositeDisposable();
        private Character _character;
        [Inject] private EquipmentParser _equipmentParser;
        [Inject] private EquipmentCreator _equipmentCreator;
        [Inject] private AugmeticsCreator _augmeticsCreator;
        [Inject] private FinderData _finderData;
        [Inject] private FactionCreator _factionCreator;

        public EditCharacterPresenter(AudioManager audioManager, EditCharacterView view)
        {
            _audioManager = audioManager;
            _view = view;
        }

        public void Initialize()
        {
            _view.OnButtonNextClick.Subscribe(_ => { ApplyAndGoNext(); }).AddTo(_cd);
            _view.OnAddEquipmentClick.Subscribe(_ => { ShowEquipmentCatalog(); }).AddTo(_cd);
            _view.OnAddAugmeticClick.Subscribe(_ => { ShowAugmeticsCatalog(); }).AddTo(_cd);
            _view.OnAddInfluenceClick.Subscribe(_ => { AddInfluenceEntry(); }).AddTo(_cd);
            _view.EquipmentChosen.Subscribe(name => { AddCatalogItem(name); }).AddTo(_cd);
            _view.EquipmentRemoveRequested.Subscribe(name => { RemoveEquipment(name); }).AddTo(_cd);
        }

        public void SetCharacter(Character character)
        {
            _character = character;
            _view.SetFields(_character);
            _view.SetMutationsText(string.Join("\n", _character.Mutations.Select(m => m.name)));
            //_view.SetCorruptionsText(string.Empty);
            _view.SetInfluenceEntries(BuildInfluenceEntries());
            _view.SetCurrentItems(_character.Equipments.ToList(), _character.Augmetics.ToList());
        }

        public void Dispose()
        {
            _cd.Dispose();
        }

        private void ApplyAndGoNext()
        {
            ApplyFields();
            _audioManager.PlayConfirm();
            _view.HideAndDestroyToLeft();
            _nextClicked.OnNext(_character);
        }

        private void ApplyFields()
        {
            if (int.TryParse(_view.AgeText, out var age)) _character.Age.Value = age;
            _character.Eyes.Value = _view.EyesText;
            _character.HairColor.Value = _view.HairColorText;
            _character.HairStyle.Value = _view.HairStyleText;
            if (int.TryParse(_view.HeightText, out var height)) _character.Height.Value = height;
            if (int.TryParse(_view.WeightText, out var weight)) _character.Weight.Value = weight;
            if (_view.IsRightHand) _character.Hand.Value = "Правая";
            else if (_view.IsLeftHand) _character.Hand.Value = "Левая";
            _character.Omen.Value = _view.MarksText;
            if (int.TryParse(_view.FatePointsText, out var fate)) _character.FatePoints.Value = fate;
            if (int.TryParse(_view.CorruptionPointsText, out var corruption)) _character.CorruptionPoints.Value = corruption;
            if (int.TryParse(_view.MoneyText, out var money)) _character.Money.Value = money;
            _character.ShortTarget.Value = _view.ShortTargetText;
            _character.LongTarget.Value = _view.LongTargetText;
            _character.Connections.Value = _view.ConnectionsText;

            //ApplyMutations(_view.MutationsText, _view.CorruptionsText);
            ApplyInfluence(_view.GetInfluenceEntries());
        }

        private void ApplyMutations(string mutationsText, string corruptionsText)
        {
            _character.Mutations.Clear();
            foreach (var item in ParseLines(mutationsText))
                _character.Mutations.Add(new Mutation { name = item, description = string.Empty });
            foreach (var item in ParseLines(corruptionsText))
                _character.Mutations.Add(new Mutation { name = item, description = string.Empty });
        }

        private void ApplyInfluence(List<(string name, int value)> entries)
        {
            _character.Influence.Clear();
            foreach (var entry in entries)
            {
                if (string.IsNullOrWhiteSpace(entry.name)) continue;
                _character.Influence[entry.name] = entry.value;
            }
        }

        private void ShowEquipmentCatalog()
        {
            _view.ShowEquipmentCatalog(GetEquipmentNames());
        }

        private void ShowAugmeticsCatalog()
        {
            _view.ShowEquipmentCatalog(GetAugmeticsNames());
        }

        private void AddCatalogItem(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _audioManager.PlayError();
                return;
            }

            var data = _equipmentParser.TryGetEquipment(name);
            if (data != null)
            {
                _character.Equipments.Add(data);
                RefreshCurrentItems();
                _audioManager.PlayClick();
                return;
            }

            if (_finderData.TryGet(name, out AugmeticData augmeticData))
            {
                _character.Augmetics.Add(augmeticData);
                RefreshCurrentItems();
                _audioManager.PlayClick();
                return;
            }

            _audioManager.PlayError();
        }

        private void AddInfluenceEntry()
        {
            var existing = _view.GetInfluenceEntries();
            var existingNames = new HashSet<string>(
                existing.Where(e => !string.IsNullOrWhiteSpace(e.name)).Select(e => e.name),
                StringComparer.OrdinalIgnoreCase);

            var name = _factionCreator.Factions
                .Select(f => f.serviceName)
                .FirstOrDefault(n => !string.IsNullOrWhiteSpace(n) && !existingNames.Contains(n));

            if (string.IsNullOrWhiteSpace(name))
                name = "Новая фракция";

            _view.AddInfluenceEntry(name, 0);
            _audioManager.PlayClick();
        }

        private void RemoveEquipment(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _audioManager.PlayError();
                return;
            }
            
            var equipmentIndex = -1;
            for (var i = 0; i < _character.Equipments.Count; i++)
            {
                var equipment = _character.Equipments[i];
                if (equipment != null && string.Compare(equipment.name, name, true) == 0)
                {
                    equipmentIndex = i;
                    break;
                }
            }

            if (equipmentIndex >= 0)
            {
                _character.Equipments.RemoveAt(equipmentIndex);
                RefreshCurrentItems();
                _audioManager.PlayClick();
                return;
            }

            var augmeticIndex = -1;
            for (var i = 0; i < _character.Augmetics.Count; i++)
            {
                var augmetic = _character.Augmetics[i];
                if (augmetic != null && string.Compare(augmetic.name, name, true) == 0)
                {
                    augmeticIndex = i;
                    break;
                }
            }

            if (augmeticIndex >= 0)
            {
                _character.Augmetics.RemoveAt(augmeticIndex);
                RefreshCurrentItems();
                _audioManager.PlayClick();
                return;
            }

            _audioManager.PlayError();
        }

        private List<string> GetEquipmentNames()
        {
            var list = new List<string>();
            list.AddRange(_equipmentCreator.Equipments.Select(e => e.name));
            list.AddRange(_equipmentCreator.Ammunitions.Select(e => e.name));
            list.AddRange(_equipmentCreator.ForceFields.Select(e => e.name));
            list.AddRange(_equipmentCreator.WeaponUpgrade.Select(e => e.name));
            list.AddRange(_equipmentCreator.Armors.Select(e => e.name));
            list.AddRange(_equipmentCreator.MeleeWeapon.Select(e => e.name));
            list.AddRange(_equipmentCreator.RangedWeapon.Select(e => e.name));
            list.AddRange(_equipmentCreator.Grenades.Select(e => e.name));
            return list.Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private List<string> GetAugmeticsNames()
        {
            return _augmeticsCreator.Augmetics
                .Select(a => a.name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private void RefreshCurrentItems()
        {
            _view.SetCurrentItems(_character.Equipments.ToList(), _character.Augmetics.ToList());
        }

        private static IEnumerable<string> ParseLines(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                yield break;

            var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed))
                    yield return trimmed;
            }
        }

        private List<(string name, int value)> BuildInfluenceEntries()
        {
            var dict = _character.Influence ?? new Dictionary<string, int>();
            var names = new List<string>();
            foreach (var f in _factionCreator.Factions)
                names.Add(f.serviceName);
            foreach (var key in dict.Keys)
                if (!names.Contains(key))
                    names.Add(key);

            names.Sort(StringComparer.OrdinalIgnoreCase);

            var list = new List<(string name, int value)>();
            foreach (var name in names)
            {
                dict.TryGetValue(name, out var value);
                list.Add((name, value));
            }
            return list;
        }
    }
}
