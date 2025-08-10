using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Cysharp.Threading.Tasks;

namespace CharacterCreation
{
    public class CharacteristicView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI descriptionText = null;
        [SerializeField] private CharacteristicCard cardPrefab = null;
        [SerializeField] private Transform cardParent = null;
        [SerializeField] private Transform amountParent = null;
        [SerializeField] private CardWithNumber cardWithNumberPrefab = null;
        [SerializeField] private Button resetButton = null;
        [SerializeField] private Button nextButton = null;
        [SerializeField] private Button rollDice = null;
        [Inject] private AudioManager _audioManager = null;
        private List<CharacteristicCard> _characteristicCards = new List<CharacteristicCard>();
        private List<CardWithNumber> _cardsWithNumber = new List<CardWithNumber>();

        public Observable<Unit> OnResetClicked => resetButton.OnClickAsObservable();
        public Observable<Unit> OnNextClicked => nextButton.OnClickAsObservable();
        public Observable<Unit> OnRollClicked => rollDice.OnClickAsObservable();

        private void Start()
        {
            Show();
        }

        public void SetCards(List<Characteristic> characteristics)
        {
            foreach (var item in characteristics)
            {
                var card = Instantiate(cardPrefab, cardParent);
                card.SetCharacteristicName(item.Name);
                card.SetStartCharacteristicValue(item.Level);
                card.SetAudioManager(_audioManager);
                card.gameObject.SetActive(true);
                _characteristicCards.Add(card);
            }
        }

        public void SetDescription(string text) => 
            descriptionText.text = text;

        public void SetAmounts(List<int> ints)
        {
            if (_cardsWithNumber.Count > 0)
            {
                foreach (var card in _cardsWithNumber)
                {
                    if (card != null)
                        Destroy(card.gameObject);
                }
                _cardsWithNumber.Clear();
            }

            foreach (var item in _characteristicCards)
            {
                var card = Instantiate(cardWithNumberPrefab, amountParent);
                card.SetNumber(ints[_characteristicCards.IndexOf(item)]);
                item.SetCardWithNumber(card);
                _cardsWithNumber.Add(card);
                card.gameObject.SetActive(true);
            }
        }

        public async void SetAmountsWithDelay(List<int> ints, float delaySeconds = 0.3f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds));
            SetAmounts(ints);
        }

        public void ResetCards()
        {
            foreach(var item in _characteristicCards)
                item.ResetValue();
        }

        public void ShowRollDiceButton() => 
            rollDice.gameObject.SetActive(true);

        public List<int> GetCurrentValues()
        {
            var values = new List<int>();
            foreach (var card in _characteristicCards)
            {
                values.Add(card.CurrentValue);
            }
            return values;
        }
    }
}

