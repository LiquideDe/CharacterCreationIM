using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class CharacterBackgroundInstaller : MonoInstaller
    {
        [SerializeField] private CharacterBackgroundView characterBackgroundView;
        [SerializeField] private UpgradeItemsListView upgradeItemsListView;
        override public void InstallBindings()
        {
            Container.Bind<CharacterBackgroundView>().FromInstance(characterBackgroundView).AsSingle();
            Container.Bind<UpgradeItemsListView>().FromInstance(upgradeItemsListView).AsSingle();
            Container.Bind<UpgradeItemsListPresenter>().AsSingle();
        }
    }
}

