using Zenject;

namespace CharacterCreation
{
    public class CreatorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<AugmeticsCreator>().AsSingle();
            Container.Bind<BackgroundCreator>().AsSingle();
            Container.Bind<EquipmentCreator>().AsSingle();
            Container.Bind<OriginCreator>().AsSingle();
            Container.Bind<PsycanaCreator>().AsSingle();
            Container.Bind<SkillCreator>().AsSingle();
            Container.Bind<TalentCreator>().AsSingle();
            Container.Bind<WeaponPropertyCreator>().AsSingle();
            Container.Bind<WeaponQualityCreator>().AsSingle();
        }
    }
}

