using R3;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation
{
    public class PanelWithText : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI Text;
        [SerializeField] private Button _button;

        private readonly Subject<string> _onSend = new();
        public Observable<string> OnSend => _onSend; 

        private CompositeDisposable _cd;

        private void OnEnable()
        {
            _cd = new CompositeDisposable();

            _button.onClick.AsObservable()
                .Subscribe(_ =>
                {
                    var text = Text.text;
                    _onSend.OnNext(text);
                })
                .AddTo(_cd);
        }

        void OnDisable()
        {
            _cd?.Dispose();
        }

        void OnDestroy()
        {
            _onSend?.Dispose();
        }
    }
}

