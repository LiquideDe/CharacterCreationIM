using CharacterCreation.Background;
using R3;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace CharacterCreation
{
    public class CreateCharacterSheetView : ViewBase
    {
        [Header("Кнопки")]
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _nextItemButton;
        [SerializeField] private Button _prevItemButton;
        [SerializeField] private Button _randomButton;
        [SerializeField] private Button _closePanelButton;

        [Header("Панели")]
        [SerializeField] private GameObject _startPanel;
        [SerializeField] private GameObject _mainPanel;

        [Header("Тексты")]
        [SerializeField] private TextMeshProUGUI _nameBackgroundText;
        [SerializeField] private TextMeshProUGUI _descriptionBackgroundText;

        [Header("Content")]
        [SerializeField] private Transform _contentList;

        [Inject] private IFactory<SkillInListView> _factorySkillInList = null;
        [Inject] private IFactory<TalentInListView> _factoryTalentInList = null;
        [Inject] private IFactory<SkillCounterInList> _factorySkillCounterInList = null;
        [Inject] private IFactory<ToggleGroupForTalents> _factoryToggleGroup = null;
        [Inject] private IFactory<TMP_WithInfo> _factoryText = null;
        [Inject] private IFactory<GarantedCharacteristic> _factoryGaranted = null;
        [Inject] private IFactory<ChooseCharacteristicView> _factoryChoose = null;

        private List<SkillInListView> _skills = new List<SkillInListView>();
        private List<TalentInListView> _talents = new List<TalentInListView>();
        private List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();
        private List<GarantedCharacteristic> _garantedCharacteristics = new List<GarantedCharacteristic>();
        private SkillCounterInList _skillCounterInList = null;

        public Observable<Unit> OnNextButtonClick => _nextButton.OnClickAsObservable();
        public Observable<Unit> OnNextItemButtonClick => _nextItemButton.OnClickAsObservable();
        public Observable<Unit> OnPrevItemButtonClick => _prevItemButton.OnClickAsObservable();
        public Observable<Unit> OnRandomButtonClick => _randomButton.OnClickAsObservable();

        private void Start()
        {
            Show();
            _closePanelButton.onClick.AddListener(() =>
            {
                _startPanel.SetActive(false);
                _mainPanel.SetActive(true);
            });

            _randomButton.onClick.AddListener(() =>
            {
                _startPanel.SetActive(false);
                _mainPanel.SetActive(true);
            });
        }

        public void SetGaranted(Dictionary<string, int> dictionary, string text)
        {
            if(text.Length > 0)
                CreateText(text);
            foreach (var item in dictionary)
            {
                var garanted = _factoryGaranted.Create();
                garanted.transform.SetParent(_contentList, worldPositionStays: false);
                var characteristic = new Characteristic(item.Key, item.Value);
                garanted.SetCharacteristic(characteristic);
                _garantedCharacteristics.Add(garanted);
            }          
        }

        public void SetChoose(Dictionary<string, int> dictionary)
        {
            var group = CreateToggleGroup("Выберите один из:");
            foreach (var item in dictionary)
            {
                var choose = _factoryChoose.Create();
                choose.transform.SetParent(_contentList, worldPositionStays: false);
                var characteristic = new Characteristic(item.Key, item.Value);
                choose.SetCharacteristic(characteristic);
                _garantedCharacteristics.Add(choose);
                choose.SetToggleGroup(group);
            }
        }

        public void SetSkills(SkillUpgrade skillUpgrade, int amount)
        {
            _skillCounterInList = _factorySkillCounterInList.Create();
            _skillCounterInList.transform.SetParent(_contentList, worldPositionStays: false);
            _skillCounterInList.Counter = skillUpgrade.amount;
            _skillCounterInList.SetText($"Распределите очки между следующими навыками:");

            foreach (var item in skillUpgrade.skills)
            {
                var skill = _factorySkillInList.Create();
                skill.transform.SetParent(_contentList, worldPositionStays: false);
                skill.SetName(item);
                _skillCounterInList.SetSkill(skill);
                _skills.Add(skill);
            }
        }

        public void SetChooseGroup(List<List<string>> chooseList, string text)
        {
            var group = CreateToggleGroup(text);
            foreach (var item in chooseList)
                CreateListChooseInOneGroup(group, item);

        }

        public void SetList(List<string> talent, string text)
        {
            var group = CreateToggleGroup(text);
            CreateTalentInList(group, talent);
        }

        public void SetText(string text)
        {
            CreateText(text);
        }

        public virtual void SetSheet(string name, string decription, bool canChange = true)
        {
            ClearLists();
            _nextItemButton.interactable = canChange;
            _prevItemButton.interactable = canChange;

            _nameBackgroundText.text = name;
            _descriptionBackgroundText.text = decription;
        }

        public bool IsCountEmpty()
        {
            return _skillCounterInList != null && _skillCounterInList.Counter == 0;
        }

        public List<GarantedCharacteristic> GetGarantedCharacteristics() => _garantedCharacteristics;

        public List<SkillInListView> GetSkills() => _skills;

        public List<TalentInListView> GetCanChosen() => _talents;

        private ToggleGroup CreateToggleGroup(string text)
        {
            var textName = _factoryToggleGroup.Create();
            textName.transform.SetParent(_contentList, worldPositionStays: false);
            textName.Text.text = $"{text}";
            _texts.Add(textName.Text);
            return textName.ToggleGroup;
        }

        private void CreateListChooseInOneGroup(ToggleGroup toggleGroup, List<string> strings)
        {
            var equip = _factoryTalentInList.Create();
            equip.transform.SetParent(_contentList, worldPositionStays: false);
            equip.SetToggleGroup(toggleGroup);
            _talents.Add(equip);

            foreach (var item in strings)
                equip.AddTalent(item);
        }

        private void CreateTalentInList(ToggleGroup toggleGroup, List<string> strings)
        {
            foreach (var item in strings)
            {
                var equip = _factoryTalentInList.Create();
                equip.transform.SetParent(_contentList, worldPositionStays: false);
                equip.AddTalent(item);
                equip.SetToggleGroup(toggleGroup);
                _talents.Add(equip);
            }
        }

        private void CreateText(string text)
        {
            var textEquip = _factoryText.Create();
            textEquip.transform.SetParent(_contentList, worldPositionStays: false);
            textEquip.SetText(text);
            _texts.Add(textEquip);
        }

        private void ClearLists()
        {
            if (_skills.Count > 0)
                foreach (var item in _skills)
                    Destroy(item.gameObject);
            _skills.Clear();

            if (_talents.Count > 0)
                foreach (var item in _talents)
                    Destroy(item.gameObject);
            _talents.Clear();

            if (_texts.Count > 0)
                foreach (var item in _texts)
                    Destroy(item.gameObject);
            _texts.Clear();

            if (_garantedCharacteristics.Count > 0)
                foreach (var item in _garantedCharacteristics)
                    Destroy(item.gameObject);
            _garantedCharacteristics.Clear();

            if (_skillCounterInList != null)
                Destroy(_skillCounterInList.gameObject);
        }
    }
}

