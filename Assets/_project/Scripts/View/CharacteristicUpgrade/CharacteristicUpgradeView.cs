using R3;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CharacterCreation
{
    public class CharacteristicUpgradeView : ViewBase
    {
        [SerializeField] private Button _buttonCancel;
        [SerializeField] private TextMeshProUGUI _textExperience;
        [SerializeField] private Button _buttonNext;
        [SerializeField] private Transform _content;
        [Inject] private AudioManager _audioManager;
        [Inject] private IFactory<CharacteristicPanel> _factory;
        private List<CharacteristicPanel> _panelList = new List<CharacteristicPanel>();
        private CompositeDisposable _disposables = new CompositeDisposable();
        private readonly Subject<Characteristic> _characteristicClicked = new();
        private LevelCostTable _levelCostTable = new LevelCostTable();
        public Observable<Characteristic> CharacteristicClicked => _characteristicClicked;
        public Observable<Unit> OnButtonCancelClick => _buttonCancel.OnClickAsObservable();
        public Observable<Unit> OnButtonNextClick => _buttonNext.OnClickAsObservable();

        public void SetCharacteristics(List<Characteristic> characteristics)
        {
            foreach (var item in characteristics)
            {
                var panel = _factory.Create();
                var characteristic = item;
                panel.transform.SetParent(_content, false);
                panel.gameObject.SetActive(true);
                panel.SetName(item.Name);
                panel.TextAmount.text = item.Level.ToString();
                panel.OnUpgradeButtonClick.Subscribe(_ => { _characteristicClicked.OnNext(characteristic); }).AddTo(_disposables);
                characteristic.LevelChanged.Subscribe(level => { 
                    panel.TextAmount.text = level.ToString(); 
                    panel.HelpText.text = $"Стоимость следующего уровня - {_levelCostTable.GetCostForNextLevel(level)} ОО";
                }).AddTo(_disposables);
                _panelList.Add(panel);
            }
        }

        public void SetExperience(int experience)
        {
            _textExperience.text = experience.ToString();
        }
    }    
}

