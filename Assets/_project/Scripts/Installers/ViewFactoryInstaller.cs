using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class ViewFactoryInstaller : MonoInstaller
    {
        [SerializeField] private ViewPrefab _prefabSO;

        public override void InstallBindings()
        {
            _prefabSO.Initialize();
            Container.Bind<ViewPrefab>().FromInstance(_prefabSO).AsSingle();
            Container.Bind<PresenterViewFactory>().AsSingle();
        }
    }
}

