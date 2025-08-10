using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CharacterCreation
{
    public class CharacteristicCard : MonoBehaviour, IDropHandler
    {
        [SerializeField] private TextMeshProUGUI characteristicNameText = null;
        [SerializeField] private TextMeshProUGUI characteristicValueText = null;
        [SerializeField] private Transform placeForInput = null;
        [SerializeField] private Transform placeForOutput = null;

        private AudioManager _audioManager = null;
        private int _startValue = 0;
        private int _currentValue = 0;
        private CardWithNumber _lastCardWithNumber = null;
        public int CurrentValue => _currentValue;
        public bool isEmpty { get; private set; } = true;

        public void OnDrop(PointerEventData eventData)
        {
            if (isEmpty)
            {
                if (eventData.pointerDrag != null)
                {
                    CardWithNumber cardWith = eventData.pointerDrag.GetComponent<CardWithNumber>();
                    if (_lastCardWithNumber != cardWith)
                    {
                        SetCardWithNumber(cardWith);
                        _audioManager.PlayClick();
                    }
                    else
                        _audioManager.PlayError();
                }
            }
        }

        public void SetCardWithNumber(CardWithNumber cardWithNumber)
        {
            _lastCardWithNumber = cardWithNumber;
            isEmpty = false;
            cardWithNumber.transform.SetPositionAndRotation(placeForInput.position, placeForInput.rotation);
            _currentValue = _startValue + cardWithNumber.Amount;
            cardWithNumber.CantReplaceCard();
            UpdateValueText();
        }

        public void SetAudioManager(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public void SetCharacteristicName(string name)
        {
            characteristicNameText.text = name;
        }

        public void SetStartCharacteristicValue(int value)
        {
            _startValue = value;
            characteristicValueText.text = value.ToString();
        }

        public void ResetValue()
        {
            characteristicValueText.text = _startValue.ToString();
            _currentValue = _startValue;
            isEmpty = true;
            if (_lastCardWithNumber != null)
            {
                _lastCardWithNumber.CanReplace();
                _lastCardWithNumber.transform.position = placeForOutput.position;
            }
            UpdateValueText();
        }

        private void UpdateValueText() => 
            characteristicValueText.text = _currentValue.ToString();

    }
}

