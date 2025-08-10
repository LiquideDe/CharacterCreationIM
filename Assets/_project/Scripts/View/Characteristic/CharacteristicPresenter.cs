using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using R3;

namespace CharacterCreation
{
    public class CharacteristicPresenter : ICharacterPresenter
    {
        private readonly Subject<Character> _nextClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        private readonly AudioManager _audioManager;
        private readonly CharacteristicView _characteristicView;
        private readonly List<IDisposable> _subscriptions = new();
        private int _countReset = 0;
        private List<Characteristic> _characteristics = new List<Characteristic>
        {
            new Characteristic("Ближний бой", 20),
            new Characteristic("Дальний бой", 20),
            new Characteristic("Сила", 20),
            new Characteristic("Выносливость", 20),
            new Characteristic("Ловкость", 20),
            new Characteristic("Интеллект", 20),
            new Characteristic("Восприятие", 20),
            new Characteristic("Сила воли", 20),
            new Characteristic("Товарищество", 20),
        };
        private List<int> _amounts = new List<int>();
        private Dictionary<int, string> _descriptionText = new Dictionary<int, string>()
        {
            {0, "Здесь мы определяем начальную величину ваших характеристик: каждую характеристику было брошено 2к10 и прибавлено 20 и расставлено по порядку. Если вам по душе такие результаты, нажмите далее и получите 50 очков опыта. Если вы думаете, что вам лучше поменять некоторые показатели характеристик местами, нажмите кнопку Сброс. Ваш дополнительный опыт будет уменьшен до 25" },
            {1, "Шаг 2: поменяйте местами все определённые только что числа, присвоив каждый показатель иной характеристике. Если результаты вас устраивает, нажмите далее и получите 25 очков опыта. Если нет, нажмите кнопку сброса. Дополнительного опыта далее не будет. " },
            {2, "Если вы всё ещё недовольны цифрами, то можете нажать кнопку \"Бросить кости\", чтобы получить новые значения, и меняйте цифры местами, пока не получите результат, что будет вас устраивать. Дополнительных очков опыта вы не получаете." }
        };
        private Dictionary<int, int> _experienceCountReset = new Dictionary<int, int>()
        {
            {0, 50},
            {1, 25},
            {2, 0}
        };

        public CharacteristicPresenter(AudioManager audioManager, CharacteristicView characteristicView)
        {
            _audioManager = audioManager;
            _characteristicView = characteristicView;
        }

        public void Initialize()
        {            
            _subscriptions.Add(_characteristicView.OnResetClicked.Subscribe(_ =>
            {
                _audioManager.PlayClick();
                _characteristicView.ResetCards();
                _countReset++;
                SetDescription();
                if(_countReset > 1)
                    _characteristicView.ShowRollDiceButton();
            }));

            _subscriptions.Add(_characteristicView.OnNextClicked.Subscribe(_ =>
            {
                _audioManager.PlayClick();
                SetCharacterAndGoNext();
            }));

            _subscriptions.Add(_characteristicView.OnRollClicked.Subscribe(_ =>
            {
                _audioManager.PlayClick();
                GenerateRandomAmounts(_characteristics.Count);
                _characteristicView.SetAmounts(_amounts);
            }));

            SetDescription();
            GenerateRandomAmounts(_characteristics.Count);
            _characteristicView.SetCards(_characteristics);
            _characteristicView.SetAmountsWithDelay(_amounts, 1f);
        }

        private void GenerateRandomAmounts(int count)
        {
            _amounts.Clear();
            var random = new System.Random();
            for (int i = 0; i < count; i++)
            {
                _amounts.Add(random.Next(1, 21)); 
            }
        }

        private void SetDescription()
        {
            if (_countReset >= _descriptionText.Count)
                _characteristicView.SetDescription(_descriptionText[_descriptionText.Count - 1]);
            else
                _characteristicView.SetDescription(_descriptionText[_countReset]);
        }

        private void SetCharacterAndGoNext()
        {
            var character = new Character();
            var newAmounts = _characteristicView.GetCurrentValues();
            for (int i = 0; i < _characteristics.Count; i++)
            {
                _characteristics[i].Level = newAmounts[i];
                character.Characteristics.Add(_characteristics[i]);
            }
            if(_countReset >= _experienceCountReset.Count)
                character.Experience = new Experience { experiencePoints = _experienceCountReset[_experienceCountReset.Count - 1], experienceSpent = 0 };
            else
                character.Experience = new Experience { experiencePoints = _experienceCountReset[_countReset], experienceSpent = 0 };
            _characteristicView.HideAndDestroyToLeft();
            _nextClicked.OnNext(character);
        }

        public void Dispose()
        {
            _nextClicked.Dispose();
            foreach (var sub in _subscriptions)
                sub.Dispose();
            _subscriptions.Clear();
        }
    }
}

