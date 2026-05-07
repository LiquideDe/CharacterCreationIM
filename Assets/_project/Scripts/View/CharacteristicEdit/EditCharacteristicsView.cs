using R3;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CharacterCreation
{
    public class EditCharacteristicsView : ViewBase
    {
        [SerializeField] private Button _buttonNext;
        [SerializeField] private Button _buttonPrev;
        [SerializeField] private Transform _content;
        [Inject] private IFactory<CharacteristicEditPanel> _factory;

        private readonly List<CharacteristicEditPanel> _panels = new();

        public Observable<Unit> OnButtonNextClick => _buttonNext.OnClickAsObservable();
        public Observable<Unit> OnButtonPrevClick => _buttonPrev.OnClickAsObservable();

        private void Start()
        {
            Show();
        }

        public CharacteristicEditPanel AddPanel()
        {
            var panel = _factory.Create();
            panel.transform.SetParent(_content, false);
            panel.gameObject.SetActive(true);
            _panels.Add(panel);
            return panel;
        }

        public void Clear()
        {
            foreach (var p in _panels)
                Destroy(p.gameObject);
            _panels.Clear();
        }
    }
}
