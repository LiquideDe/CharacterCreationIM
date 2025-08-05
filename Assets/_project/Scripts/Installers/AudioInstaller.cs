using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class AudioInstaller : MonoInstaller
    {
        [SerializeField] private AudioManager _audioManager;

        public override void InstallBindings()
        {
            Container.Bind<AudioManager>().FromInstance(_audioManager).AsSingle();
        }
    }
}
