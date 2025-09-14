using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class PrintCharacterInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PrintCharacterPresenter>().AsSingle();
        }
    }
}

