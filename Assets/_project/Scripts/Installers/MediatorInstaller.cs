using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class MediatorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<StartMediator>().AsSingle();
        }
    }
}

