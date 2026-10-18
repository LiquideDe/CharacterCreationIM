using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class UpgradeItemsListView : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [Inject] private IFactory<UpgradeItem> _itemFactory;
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public UpgradeItem CreateUpgradeItem(string name, int cost, System.Action onCancel)
        {
            var item = _itemFactory.Create();
            item.transform.SetParent(_content, false);
            item.SetUpgrade(name, cost, onCancel);
            item.gameObject.SetActive(true);
            return item;
        }

        public void RemoveUpgradeItem(UpgradeItem item)
        {
            Destroy(item.gameObject);
        }

        public void Clear()
        {
            for (var index = _content.childCount - 1; index >= 0; index--)
                Destroy(_content.GetChild(index).gameObject);
        }
    }
}
