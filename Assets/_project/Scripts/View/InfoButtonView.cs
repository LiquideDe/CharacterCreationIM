using CharacterCreation.Background;
using R3;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static CharacterCreation.TalentCreator;

namespace CharacterCreation
{
    public class InfoButtonView : MonoBehaviour
    {
        [SerializeField] private Button _infoButton;
        [Inject] private AudioManager _audioManager = null;
        [Inject] private IFactory<InfoPanelView> _infoPanelFactory = null;
        [Inject] private EquipmentParser _equipmentParser = null;
        [Inject] private FinderData _finderData = null;
        [Inject] private SkillCreator _skillCreator = null;
        private string _infoText;
        private CompositeDisposable _disposable = new CompositeDisposable();
        private Canvas _canvas;

        private void Awake()
        {
            _infoButton.onClick
            .AsObservable()                  
            .Subscribe(_ =>
            {
                _audioManager.PlayClick();
                var infoPanel = _infoPanelFactory.Create();
                infoPanel.transform.SetParent(GetCanvas().transform, false);
                infoPanel.SetText(_infoText);
            })
            .AddTo(_disposable);
        }

        public void SetInfo(string nameSheet)
        {
            if (_finderData.TryGet(nameSheet, out TalentData talentData)) 
                _infoText += $"{nameSheet} - {talentData.description}\n\n";

            else if (_finderData.TryGet(nameSheet, out EquipmentData equipmentData))
                ParseEquipment(equipmentData);

            else if (_finderData.TryGet(nameSheet, out AugmeticData augmeticData))
                _infoText += $"{nameSheet} - {augmeticData.description}\n\n";
            else
            {
                //Debug.LogAssertion($"Не нашли {nameSheet}");
                gameObject.SetActive(false);
            }
                
        }

        public void SetInfos(List<string> strings)
        {
            foreach (var item in strings)
                SetInfo(item);
        }

        public void SetSkill(string nameSkill)
        {
            SkillData skillData = _skillCreator.SkillByName(nameSkill);
            _infoText = $"{nameSkill} - {skillData.description}, основная характеристика - {skillData.characteristic}";
        }

        private void ParseEquipment(EquipmentData equipmentData)
        {
            _infoText += $"{equipmentData.name} - {equipmentData.description}. Вес в руках - {equipmentData.weight}. " +
                $"Вес на теле - {equipmentData.maxWeight}. ";            
                
            switch (equipmentData)
            {
                case AmmunitionData ammunitionData:
                    _infoText += $"Эффект - {ammunitionData.effect}";break;

                case WeaponUpgradeData weaponUpgradeData:
                    _infoText += $"Тип оружия: {weaponUpgradeData.typeWeapon}";break;

                case ArmorData armorData:
                    _infoText += $"Тип брони: {armorData.type}. Защищаемые зоны: {string.Join(", ", armorData.protectionZones)}. " +
                        $"Очки брони: {armorData.armorPoints}."; break;

                case RangedWeaponData rangedWeaponData:
                    _infoText += $"Тип оружия: {rangedWeaponData.type}. Специализация: {rangedWeaponData.specialization.specialization} " +
                        $"({rangedWeaponData.specialization.skill}). Урон: {rangedWeaponData.damage}. Дальность: {rangedWeaponData.range}. " +
                        $"Емкость магазина: {rangedWeaponData.clip}."; break;

                case MeleeWeaponData meleeWeaponData:
                    _infoText += $"Тип оружия: {meleeWeaponData.type}. Специализация: {meleeWeaponData.specialization.specialization} " +
                        $"({meleeWeaponData.specialization.skill}). Урон: {meleeWeaponData.damage}."; break;

                case ForceFieldData forceFieldData:
                    _infoText += $"Защита: {forceFieldData.defense}. Перезарядка: {forceFieldData.reload}."; break;
            }

            if (equipmentData.properties != null && equipmentData.properties.Count > 0)
            {
                _infoText += "Свойства:\n";
                foreach (var item in equipmentData.properties)
                    _infoText += $" {item}, ";
            }
        }

        private Canvas GetCanvas()
        {
            if (_canvas == null)
                _canvas = GetComponentInParent<Canvas>(true)
                       ?? transform.root.GetComponentInChildren<Canvas>(true);
            return _canvas;
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}
