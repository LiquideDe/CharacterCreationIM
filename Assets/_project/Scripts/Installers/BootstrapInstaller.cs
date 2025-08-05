using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private Bootstrap bootstrap;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Bootstrap>().FromInstance(bootstrap).AsSingle();
        }
    }
}

