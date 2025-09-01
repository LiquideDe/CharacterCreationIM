using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class CharacterBackgroundInstaller : MonoInstaller
    {
        [SerializeField] private CharacterBackgroundView characterBackgroundView;
        override public void InstallBindings()
        {
            Container.Bind<CharacterBackgroundView>().FromInstance(characterBackgroundView).AsSingle();
        }
    }
}

