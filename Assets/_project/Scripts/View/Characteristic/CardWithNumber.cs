using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CharacterCreation
{
    public class CardWithNumber : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private TextMeshProUGUI numberText = null;
        [SerializeField] private CanvasGroup _canvasGroup;
        private Vector3 _startPos;
        
        public bool CantReplace { get; private set; }
        public int Amount { get; private set; } = 0;

        public void SetNumber(int amount)
        {
            Amount = amount;
            numberText.text = amount.ToString();
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPos = transform.position;
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!CantReplace)
            {
                transform.position = _startPos;
                _canvasGroup.blocksRaycasts = true;
            }
        }

        public void CanReplace()
        {
            _canvasGroup.blocksRaycasts = true;
            CantReplace = false;
        }

        public void CantReplaceCard()
        {
            _canvasGroup.blocksRaycasts = false;
            CantReplace = true;
        }
    }
}

