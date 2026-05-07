using R3;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CharacterCreation
{
    public class LookView : ViewBase
    {
        [SerializeField] private Button _buttonNext;
        [SerializeField] private Button _buttonRandom;

        [SerializeField] private TextMeshProUGUI _textNameCategory;
        [SerializeField] private TextMeshProUGUI _textDescription;
        [SerializeField] private TextMeshProUGUI _hideText;
        [SerializeField] private GameObject _list;
        [SerializeField] private Transform _content;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private ToggleGroup _toggleGroup;
        [SerializeField] private LookPrefab _lookPrefab;

        [SerializeField] private GameObject _openPanel;
        [SerializeField] private GameObject _mainPanel;

        [Inject] private AudioManager _audioManager; 

        private List<LookPrefab> _lookPrefabs = new List<LookPrefab>();

        public Observable<Unit> OnNextButtonClick => _buttonNext.OnClickAsObservable();
        public Observable<Unit> OnRandomButtonClick => _buttonRandom.OnClickAsObservable();

        public string InputField => _inputField.text;

        private void Start()
        {
            Show();
        }

        public void SetNameCategory(string name, string description)
        {
            Clear();      
            
            _textNameCategory.text = name;
            _textDescription.text = description;
            _inputField.text = string.Empty;
            HideAndShow();
        }

        public void SetList(List<string> strings)
        {
            _list.SetActive(true);
            foreach (var item in strings)
            {
                LookPrefab look = Instantiate(_lookPrefab, _content);
                look.SetToggleGroup(_toggleGroup);
                look.SetText(item);
                look.SubscribeSound(_audioManager);
                _lookPrefabs.Add(look);
                look.gameObject.SetActive(true);
            }            
        }

        public string GetChosenLook() => _lookPrefabs.Where(t => t.IsOn).First().Text;        

        private void Clear() 
        {
            foreach (var item in _lookPrefabs)            
                Destroy(item.gameObject);
            
            _lookPrefabs.Clear();
            _hideText.gameObject.SetActive(false);
            _list.SetActive(false);
            _openPanel.SetActive(false);
            _mainPanel.SetActive(true);
        }
    }
}

