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
        private List<GameObject> gameObjects = new List<GameObject>();
        public void SetCharacter(Character character)
        {
            _character = character;
            _character.Characteristics.ObserveAdd().Subscribe(c => AddCharacteristic(c.Value)).AddTo(_disposables);
            _character.Origin.Subscribe(origin => AddBackground("Происхождение", origin)).AddTo(_disposables);
            _character.Faction.Subscribe(faction => AddBackground("Служба", faction)).AddTo(_disposables);
            _character.Role.Subscribe(role => AddBackground("Роль", role)).AddTo(_disposables);
        }

        private void AddBackground(string nameBackground, string name)
        {
            if (name == null || name.Length == 0)
                return;
            
            var background = factoryBackground.Create();
            background.transform.SetParent(_backgroundContent, false);
            background.TextName.text = name;
            background.TextNameBackground.text = nameBackground;
            gameObjects.Add(background.gameObject);
        }

        public void AddCharacteristic(Characteristic characteristic)
        {
            var gChracteristtic = factoryCharacteristic.Create();
            gChracteristtic.transform.SetParent(_CharacteristicContent, false);
            UpdateCharacteristic(gChracteristtic, characteristic);
            characteristic.LevelChanged.Subscribe(_ => UpdateCharacteristic(gChracteristtic, characteristic)).AddTo(_disposables);
            gameObjects.Add(gChracteristtic.gameObject);
        }

        public void Clear()
        {
            foreach (var go in gameObjects)
            {
                Destroy(go);
            }
            gameObjects.Clear();
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
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

