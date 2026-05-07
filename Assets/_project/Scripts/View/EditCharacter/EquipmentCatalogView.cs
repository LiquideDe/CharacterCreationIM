using CharacterCreation.Background;
using R3;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CharacterCreation
{
    public class EquipmentCatalogView : ViewBase
    {
        [SerializeField] private Transform _content;
        [SerializeField] private ToggleGroupForTalents _toggleGroup;
        [SerializeField] private Button _buttonAdd;
        [SerializeField] private Button _buttonClose;

        [Inject] private IFactory<TalentInListView> _factory;
        private readonly List<TalentInListView> _items = new();
        private readonly Subject<string> _equipmentChosen = new();
        public Observable<string> EquipmentChosen => _equipmentChosen;

        private void OnEnable()
        {
            _buttonAdd.OnClickAsObservable().Take(1).Subscribe(_ => AddSelected());
            _buttonClose.OnClickAsObservable().Take(1).Subscribe(_ => Close());
            Show();
        }

        public void ShowCatalog(List<string> names)
        {
            Clear();
            foreach (var name in names)
            {
                var item = _factory.Create();
                item.transform.SetParent(_content, false);
                item.AddTalent(name);
                _toggleGroup.AddToggle(item.Toggle);
                _items.Add(item);
            }
        }

        private void AddSelected()
        {
            if (_toggleGroup.SelectedCount() == 0)
                return;

            foreach (var item in _items)
            {
                if (item.IsSelected)
                {
                    _equipmentChosen.OnNext(item.Talents.FirstOrDefault());
                    break;
                }
            }
            Close();
        }

        private void Close()
        {
            Clear();
            HideToRight();
        }

        private void Clear()
        {
            foreach (var item in _items)
                Destroy(item.gameObject);
            _items.Clear();
        }
    }
}
