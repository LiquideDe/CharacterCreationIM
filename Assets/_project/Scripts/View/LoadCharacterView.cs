using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace CharacterCreation
{
    public class LoadCharacterView : ViewBase
    {
        [SerializeField] private Transform _content;
        [SerializeField] private Button _buttonClose;
        [Inject] private AudioManager _audioManager;
        [Inject] private IFactory<ButtonInList> _factory;
        private CompositeDisposable _cd = new CompositeDisposable();
        private Subject<Entry> _onSaveClick = new Subject<Entry>();
        public Observable<Entry> OnSaveClicked => _onSaveClick;
        public Observable<Unit> OnButtonCloseClicked => _buttonClose.OnClickAsObservable();

        private void Start()
        {
            Show();
        }

        public void SetEntries(List<Entry> entries)
        {
            Debug.LogAssertion($"entries = {entries.Count}");
            foreach (var item in entries)
            {
                var button = _factory.Create();
                var entry = item;
                button.transform.SetParent(_content, false);
                button.SetName(item.FileName);
                button.Button.OnClickAsObservable().Subscribe(_ => { _audioManager.PlayClick(); _onSaveClick.OnNext(entry); }).AddTo(_cd);
            }
        }

        private void OnDestroy()
        {
            _cd.Dispose();
        }
    }
}

