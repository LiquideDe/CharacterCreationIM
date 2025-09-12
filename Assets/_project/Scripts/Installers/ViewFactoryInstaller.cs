using CharacterCreation.Background;
using TMPro;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class ViewFactoryInstaller : MonoInstaller
    {
        [SerializeField] private ViewPrefab _prefabSO;
        [SerializeField] private SkillInListView skillInListViewprefab;
        [SerializeField] private TalentInListView talentInListViewprefab;
        [SerializeField] private SkillCounterInList skillCounterInListPrefab;
        [SerializeField] private ToggleGroupForTalents toggleGroupForTalentsPrefab;
        [SerializeField] private TMP_WithInfo textPrefab;
        [SerializeField] private GarantedCharacteristic garantedCharacteristicPrefab;
        [SerializeField] private ChooseCharacteristicView chooseCharacteristicPrefab;
        [SerializeField] private InfoPanelView infoTextViewPrefab;
        [SerializeField] private BackgroundCharacterPrefab backgroundCharacterPrefab;
        [SerializeField] private CharacteristicBackgroundView characteristicBackgroundViewPrefab;
        [SerializeField] private CharacteristicPanel characteristicPanelPrefab;
        [SerializeField] private SkillPanelUpgrade skillPanelUpgradePrefab;
        [SerializeField] private NewSpecializationPanel newSpecializationPanelPrefab;

        public override void InstallBindings()
        {
            _prefabSO.Initialize();
            Container.Bind<ViewPrefab>().FromInstance(_prefabSO).AsSingle();
            Container.Bind<PresenterViewFactory>().AsSingle();
            Container.BindIFactory<SkillInListView>().FromComponentInNewPrefab(skillInListViewprefab);
            Container.BindIFactory<TalentInListView>().FromComponentInNewPrefab(talentInListViewprefab);
            Container.BindIFactory<SkillCounterInList>().FromComponentInNewPrefab(skillCounterInListPrefab);
            Container.BindIFactory<ToggleGroupForTalents>().FromComponentInNewPrefab(toggleGroupForTalentsPrefab);
            Container.BindIFactory<TMP_WithInfo>().FromComponentInNewPrefab(textPrefab);
            Container.BindIFactory<GarantedCharacteristic>().FromComponentInNewPrefab(garantedCharacteristicPrefab);
            Container.BindIFactory<ChooseCharacteristicView>().FromComponentInNewPrefab(chooseCharacteristicPrefab); 
            Container.BindIFactory<InfoPanelView>().FromComponentInNewPrefab(infoTextViewPrefab);
            Container.BindIFactory<BackgroundCharacterPrefab>().FromComponentInNewPrefab(backgroundCharacterPrefab);
            Container.BindIFactory<CharacteristicBackgroundView>().FromComponentInNewPrefab(characteristicBackgroundViewPrefab);
            Container.BindIFactory<CharacteristicPanel>().FromComponentInNewPrefab(characteristicPanelPrefab);
            Container.BindIFactory<SkillPanelUpgrade>().FromComponentInNewPrefab(skillPanelUpgradePrefab);
            Container.BindIFactory<NewSpecializationPanel>().FromComponentInNewPrefab(newSpecializationPanelPrefab);
        }
    }
}

