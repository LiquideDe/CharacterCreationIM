using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CharacterCreation
{
    public class CharacteristicView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI descriptionText = null;
        [SerializeField] private CharacteristicCard cardPrefab = null;
        [SerializeField] private Transform cardParent = null;
        [SerializeField] private CardWithNumber cardWithNumberPrefab = null;
        private List<CharacteristicCard> _characteristicCards = new List<CharacteristicCard>();
        private List<CardWithNumber> _cardsWithNumber = new List<CardWithNumber>();

        public void SetCards(List<Characteristic> characteristics)
        {
            foreach (var item in characteristics)
            {
                var card = Instantiate(cardPrefab, cardParent);
                card.SetCharacteristicName(item.Name);
                card.SetStartCharacteristicValue(item.Level);
                _characteristicCards.Add(card);
            }
        }

        public void SetDescription(string text) => 
            descriptionText.text = text;

        public void SetAmount(List<int> ints)
        {
            foreach (var item in _characteristicCards)
            {
                var card = Instantiate(cardWithNumberPrefab);
                card.SetNumber(ints[_characteristicCards.IndexOf(item)]);
                item.SetCardWithNumber(card);
                _cardsWithNumber.Add(card);
            }
        }

        public void ResetCards()
        {
            foreach(var item in _characteristicCards)
                item.ResetValue();
        }

    }
}

