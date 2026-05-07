using R3;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CharacterCreation
{
    public class EditCharacterView : ViewBase
    {
        [Header("Buttons")]
        [SerializeField] private Button _buttonNext;
        [SerializeField] private Button _buttonAddEquipment;
        [SerializeField] private Button _buttonAddAugmetic;
        [SerializeField] private Button _buttonAddInfluence;
        [SerializeField] private EquipmentCatalogView _equipmentCatalogView;

        [Header("Inputs")]
        [SerializeField] private TMP_InputField _inputAge;
        [SerializeField] private TMP_InputField _inputEyes;
        [SerializeField] private TMP_InputField _inputHairColor;
        [SerializeField] private TMP_InputField _inputHairStyle;
        [SerializeField] private TMP_InputField _inputHeight;
        [SerializeField] private TMP_InputField _inputWeight;
        [SerializeField] private Toggle _toggleRightHand;
        [SerializeField] private Toggle _toggleLeftHand;
        [SerializeField] private TMP_InputField _inputMarks;
        [SerializeField] private TMP_InputField _inputFatePoints;
        [SerializeField] private TMP_InputField _inputCorruptionPoints;
        [SerializeField] private TMP_InputField _inputMoney;
        [SerializeField] private TMP_InputField _inputMutations;
        [SerializeField] private TMP_InputField _inputShortTarget;
        [SerializeField] private TMP_InputField _inputLongTarget;
        [SerializeField] private TMP_InputField _inputConnections;
        
        [Header("Equipment")]
        [SerializeField] private Transform _equipmentContent;
        [Inject] private IFactory<EquipmentEntryView> _equipmentFactory;
        
        [Header("Augmetics")]
        [SerializeField] private Transform _augmeticsContent;
        [Inject] private IFactory<EquipmentEntryView> _augmeticsFactory;

        [Header("Influence")]
        [SerializeField] private Transform _influenceContent;
        [Inject] private IFactory<InfluenceEntryView> _influenceFactory;

        private readonly List<EquipmentEntryView> _equipmentViews = new();
        private readonly List<EquipmentEntryView> _augmeticsViews = new();
        private readonly Subject<string> _equipmentRemoveRequested = new();
        public Observable<string> EquipmentRemoveRequested => _equipmentRemoveRequested;
        public Observable<string> EquipmentChosen => _equipmentCatalogView.EquipmentChosen;
        private readonly List<InfluenceEntryView> _influenceViews = new();

        public Observable<Unit> OnButtonNextClick => _buttonNext.OnClickAsObservable();
        public Observable<Unit> OnAddEquipmentClick => _buttonAddEquipment.OnClickAsObservable();
        public Observable<Unit> OnAddAugmeticClick => _buttonAddAugmetic.OnClickAsObservable();
        public Observable<Unit> OnAddInfluenceClick => _buttonAddInfluence.OnClickAsObservable();

        public string AgeText => _inputAge.text;
        public string EyesText => _inputEyes.text;
        public string HairColorText => _inputHairColor.text;
        public string HairStyleText => _inputHairStyle.text;
        public string HeightText => _inputHeight.text;
        public string WeightText => _inputWeight.text;
        public bool IsRightHand => _toggleRightHand != null && _toggleRightHand.isOn;
        public bool IsLeftHand => _toggleLeftHand != null && _toggleLeftHand.isOn;
        public string MarksText => _inputMarks.text;
        public string FatePointsText => _inputFatePoints.text;
        public string CorruptionPointsText => _inputCorruptionPoints.text;
        public string MoneyText => _inputMoney.text;
        public string MutationsText => _inputMutations.text;
        public string ShortTargetText => _inputShortTarget.text;
        public string LongTargetText => _inputLongTarget.text;
        public string ConnectionsText => _inputConnections.text;

        private void Start()
        {
            Show();
        }

        public void SetFields(Character character)
        {
            _inputAge.text = character.Age.Value.ToString();
            _inputEyes.text = character.Eyes.Value;
            _inputHairColor.text = character.HairColor.Value;
            _inputHairStyle.text = character.HairStyle.Value;
            _inputHeight.text = character.Height.Value.ToString();
            _inputWeight.text = character.Weight.Value.ToString();
            _inputMarks.text = character.Omen.Value;
            _inputFatePoints.text = character.FatePoints.Value.ToString();
            _inputMoney.text = character.Money.Value.ToString();
            _inputShortTarget.text = character.ShortTarget.Value;
            _inputLongTarget.text = character.LongTarget.Value;
            _inputConnections.text = character.Connections.Value;

            if (_toggleRightHand != null) _toggleRightHand.isOn = string.Compare(character.Hand.Value, "Правая", true) == 0;
            if (_toggleLeftHand != null) _toggleLeftHand.isOn = string.Compare(character.Hand.Value, "Левая", true) == 0;
        }

        public void SetMutationsText(string text) => _inputMutations.text = text;
        public void SetInfluenceEntries(List<(string name, int value)> entries)
        {
            ClearInfluence();
            foreach (var entry in entries)
            {
                var view = _influenceFactory.Create();
                view.transform.SetParent(_influenceContent, false);
                view.SetName(entry.name);
                view.SetValue(entry.value);
                _influenceViews.Add(view);
            }
        }

        public List<(string name, int value)> GetInfluenceEntries()
        {
            var list = new List<(string name, int value)>();
            foreach (var view in _influenceViews)
                list.Add((view.Name, view.GetValue()));
            return list;
        }

        public void AddInfluenceEntry(string name, int value)
        {
            var view = _influenceFactory.Create();
            view.transform.SetParent(_influenceContent, false);
            view.SetName(name);
            view.SetValue(value);
            _influenceViews.Add(view);
        }

        public void ShowEquipmentCatalog(List<string> names)
        {
            if (_equipmentCatalogView == null) return;
            _equipmentCatalogView.gameObject.SetActive(true);
            _equipmentCatalogView.ShowCatalog(names);
        }

        public void SetCurrentEquipment(List<EquipmentData> equipments)
        {
            ClearEquipmentList();
            foreach (var item in equipments)
            {
                var view = _equipmentFactory.Create();
                view.transform.SetParent(_equipmentContent, false);
                view.SetName(item.name);
                _equipmentViews.Add(view);
                view.OnRemoveClick.Subscribe(_ => _equipmentRemoveRequested.OnNext(item.name)).AddTo(view);
            }
        }

        public void SetCurrentItems(List<EquipmentData> equipments, List<AugmeticData> augmetics)
        {
            ClearEquipmentList();
            ClearAugmeticsList();

            foreach (var item in equipments)
            {
                if (item == null) continue;
                var view = _equipmentFactory.Create();
                view.transform.SetParent(_equipmentContent, false);
                view.SetName(item.name);
                _equipmentViews.Add(view);
                view.OnRemoveClick.Subscribe(_ => _equipmentRemoveRequested.OnNext(item.name)).AddTo(view);
            }

            foreach (var item in augmetics)
            {
                if (item == null) continue;
                var view = _augmeticsFactory.Create();
                view.transform.SetParent(_augmeticsContent, false);
                view.SetName(item.name);
                _augmeticsViews.Add(view);
                view.OnRemoveClick.Subscribe(_ => _equipmentRemoveRequested.OnNext(item.name)).AddTo(view);
            }
        }

        public void ClearEquipmentList()
        {
            foreach (var view in _equipmentViews)
                Destroy(view.gameObject);
            _equipmentViews.Clear();
        }

        public void ClearAugmeticsList()
        {
            foreach (var view in _augmeticsViews)
                Destroy(view.gameObject);
            _augmeticsViews.Clear();
        }

        private void ClearInfluence()
        {
            foreach (var view in _influenceViews)
                Destroy(view.gameObject);
            _influenceViews.Clear();
        }
    }
}
