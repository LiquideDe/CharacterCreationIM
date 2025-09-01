using ObservableCollections;
using R3;
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
            if (name.Length == 0)
                return;
            
            var background = factoryBackground.Create();
            background.transform.SetParent(_backgroundContent, false);
            background.TextName.text = name;
            background.TextNameBackground.text = nameBackground;
        }

        public void AddCharacteristic(Characteristic characteristic)
        {
            var gChracteristtic = factoryCharacteristic.Create();
            gChracteristtic.transform.SetParent(_CharacteristicContent, false);
            UpdateCharacteristic(gChracteristtic, characteristic);
            characteristic.LevelChanged.Subscribe(_ => UpdateCharacteristic(gChracteristtic, characteristic)).AddTo(_disposables);
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

