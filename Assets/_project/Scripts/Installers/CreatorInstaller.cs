using Zenject;

namespace CharacterCreation
{
    public class CreatorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AugmeticsCreator>().AsSingle();
            Container.BindInterfacesAndSelfTo<FactionCreator>().AsSingle();
            Container.BindInterfacesAndSelfTo<EquipmentCreator>().AsSingle();
            Container.BindInterfacesAndSelfTo<OriginCreator>().AsSingle();
            Container.BindInterfacesAndSelfTo<PsycanaCreator>().AsSingle();
            Container.BindInterfacesAndSelfTo<SkillCreator>().AsSingle();
            Container.BindInterfacesAndSelfTo<TalentCreator>().AsSingle();
            Container.BindInterfacesAndSelfTo<WeaponPropertyCreator>().AsSingle();
            Container.BindInterfacesAndSelfTo<WeaponQualityCreator>().AsSingle();
            Container.BindInterfacesAndSelfTo<EquipmentParser>().AsSingle();
            Container.BindInterfacesAndSelfTo<FinderData>().AsSingle();
        }
    }
}

