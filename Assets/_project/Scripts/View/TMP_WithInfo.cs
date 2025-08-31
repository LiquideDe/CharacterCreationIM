using TMPro;
using UnityEngine;

namespace CharacterCreation
{
    public class TMP_WithInfo : TextMeshProUGUI
    {
        [SerializeField] private InfoButtonView _infoButtonView;
        public void SetText(string textInfo)
        {
            text = textInfo;
            _infoButtonView.gameObject.SetActive(true);
            _infoButtonView.SetInfo(textInfo);
        }
    }
}

