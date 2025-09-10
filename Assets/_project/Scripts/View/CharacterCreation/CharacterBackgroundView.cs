using ObservableCollections;
using R3;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class CharacterBackgroundView : MonoBehaviour
    {
        [SerializeField] private Transform _backgroundContent;
        [SerializeField] private Transform _CharacteristicContent;
        [Inject] private IFactory<CharacteristicBackgroundView> factoryCharacteristic;
        [Inject] private IFactory<BackgroundCharacterPrefab> factoryBackground;
        private Character _character;
        private CompositeDisposable _disposables = new CompositeDisposable();
        private CompositeDisposable _disposablesCharacterists = new CompositeDisposable();
        private List<GameObject> _gameObjects = new List<GameObject>();
        private List<GameObject> _characteristics = new List<GameObject>();
        public void SetCharacter(Character character)
        {
            _character = character;
            _character.Characteristics.ObserveAdd().Subscribe(c => AddCharacteristic(c.Value)).AddTo(_disposables);
            _character.Origin.Subscribe(origin => AddBackground("Происхождение", origin)).AddTo(_disposables);
            _character.Faction.Subscribe(faction => AddBackground("Служба", faction)).AddTo(_disposables);
            _character.Role.Subscribe(role => { AddBackground("Роль", role); ClearList(); }).AddTo(_disposables);
            _character.Age.Subscribe(age => { if(age > 0)AddBackground("Возраст", age.ToString()); }).AddTo(_disposables);
            _character.Eyes.Subscribe(eyes => AddBackground("Глаза", eyes)).AddTo(_disposables);
            _character.HairColor.Subscribe(hair => AddBackground("Цвет волос", hair)).AddTo(_disposables);
            _character.HairStyle.Subscribe(hair => AddBackground("Стиль прически", hair)).AddTo(_disposables);
            _character.Omen.Subscribe(omen => AddBackground("Особые приметы", omen)).AddTo(_disposables);
            _character.ShortTarget.Subscribe(target => AddBackground("Краткосрочные цели", target)).AddTo(_disposables);
            _character.LongTarget.Subscribe(target => AddBackground("Долгосрочные цели", target )).AddTo(_disposables);

        }

        private void AddBackground(string nameBackground, string name)
        {
            if (name == null || name.Length == 0)
                return;
            
            var background = factoryBackground.Create();
            background.transform.SetParent(_backgroundContent, false);
            background.TextName.text = name;
            background.TextNameBackground.text = nameBackground;
            _gameObjects.Add(background.gameObject);
        }

        public void AddCharacteristic(Characteristic characteristic)
        {
            var gChracteristtic = factoryCharacteristic.Create();
            gChracteristtic.transform.SetParent(_CharacteristicContent, false);
            UpdateCharacteristic(gChracteristtic, characteristic);
            characteristic.LevelChanged.Subscribe(_ => UpdateCharacteristic(gChracteristtic, characteristic)).AddTo(_disposablesCharacterists);
            _characteristics.Add(gChracteristtic.gameObject);
        }

        public void Clear()
        {
            foreach (var go in _gameObjects)            
                Destroy(go);
            
            _gameObjects.Clear();
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
        }

        public void ClearList() 
        {
            foreach (var item in _characteristics)            
                Destroy(item);
            _characteristics.Clear();
            _disposablesCharacterists?.Clear();
        }

        private void UpdateCharacteristic(CharacteristicBackgroundView characteristicView, Characteristic characteristic)
        {
            characteristicView.TextName.text = characteristic.Name;
            characteristicView.TextAmount.text = characteristic.Level.ToString();
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

    }
}

