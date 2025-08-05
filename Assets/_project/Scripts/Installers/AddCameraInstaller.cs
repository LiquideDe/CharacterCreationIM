using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class AddCameraInstaller : MonoInstaller
    {
        [SerializeField] Camera camera;
        public override void InstallBindings()
        {
            Container.Bind<Camera>()
                .FromInstance(camera)
                .AsSingle()
                .NonLazy();
        }
    }
}

