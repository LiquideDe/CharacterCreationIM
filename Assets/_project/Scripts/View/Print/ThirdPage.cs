using R3;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;
using static CharacterCreation.WeaponPropertyCreator;

namespace CharacterCreation
{
    public class ThirdPage : TakeScreenshot
    {
        [SerializeField] private TextMeshProUGUI _text, _textNameCharacter;
        [Inject] private EquipmentParser _equipmentParser;
        [Inject] private FinderData _finderData;
        private int _page;

        public void SetCharacter(Character character)
        {
            _character = character;
            _textNameCharacter.text = _character.Name.Value;

            _text.text += $"<indent=15%><size=150%>Таланты:</indent> \n<size=100%>";

            foreach (TalentData talent in character.Talents)
            {
                if(talent.description != null && talent.description.Length > 1)
                    _text.text += $"<b>{talent.name}</b> - {talent.description} \n \n";
                else if (_finderData.TryGet(talent.name, out TalentData talentData))
                    _text.text += $"<b>{talent.name}</b> - {talentData.description} \n \n";
            }
                

            if (character.PsyPowers.Count > 0)
            {
                _text.text += $"<indent=15%><size=150%>Психо-силы:</indent> \n<size=100%>";
                foreach (PsyData psyPower in character.PsyPowers)
                {
                    if(psyPower.description != null & psyPower.description.Length > 1)
                    {
                        _text.text += $"<b>{psyPower.name}</b> \n " +
                        $"<b>Варп уровень:</b> {psyPower.warpCharge}.\n" +
                        $"<b>Проверка:</b> {psyPower.testDifficulty}. \n" +
                        $"<b>Дальность:</b> {psyPower.range}. \n" +
                        $"<b>Цель:</b> {psyPower.target}. \n" +
                        $"<b>Срок действия:</b> {psyPower.duration}.\n" +
                        $"<b>Описание:</b> {psyPower.description}\n\n";
                    }
                    else if(_finderData.TryGet(psyPower.name, out PsyData psyData))
                    {
                        _text.text += $"<b>{psyData.name}</b> \n " +
                        $"<b>Варп уровень:</b> {psyData.warpCharge}.\n" +
                        $"<b>Проверка:</b> {psyData.testDifficulty}. \n" +
                        $"<b>Дальность:</b> {psyData.range}. \n" +
                        $"<b>Цель:</b> {psyData.target}. \n" +
                        $"<b>Срок действия:</b> {psyData.duration}.\n" +
                        $"<b>Описание:</b> {psyData.description}\n\n";
                    }
                }       
            }

            if (character.Augmetics.Count > 0)
            {
                _text.text += $"<indent=15%><size=150%>Импланты:</indent> \n<size=100%>";
                foreach (AugmeticData implant in character.Augmetics)
                {
                    if(implant.description != null && implant.description.Length > 1)
                        _text.text += $"<b>{implant.name}</b> - {implant.description}\n \n";
                    else if (_finderData.TryGet(implant.name, out AugmeticData augmetic))
                        _text.text += $"<b>{augmetic.name}</b> - {augmetic.description}\n \n";
                }
            }

            _text.text += $"<indent=15%><size=150%>Экипировка:</indent> \n<size=100%>";
            foreach (EquipmentData equipment in character.Equipments)
            {
                try
                {
                    _text.text += $"<b>{equipment.name}</b>. \nОписание: {equipment.description}. Вес: {equipment.weight} \n \n";
                }
                catch ( Exception ex)
                {
                    Debug.LogAssertion(ex);
                }
                
                if (equipment is MeleeWeaponData weapon)
                {
                    if (weapon.properties.Count > 0)
                    {
                        foreach (var item in weapon.properties)
                        {
                            if (_finderData.TryGet(item, out WeaponPropertyData weaponProperty))
                                _text.text += $"{weaponProperty.name} - {weaponProperty.description}\n";
                        }
                    }
                    _text.text += "\n";
                }
            }

            StartCoroutine(TakePauseForText());
        }

        IEnumerator TakePauseForText()
        {
            yield return new WaitForEndOfFrame();
            if (_text.textInfo.pageCount > 1)
            {
                _page = 1;
                base.PageSaved.Subscribe(_ => PageSavedDown()).AddTo(this);
                StartScreenshot(PageName.Third.ToString(), true);
            }
            else
            {
                StartScreenshot(PageName.Third.ToString());
            }

        }

        private void PageSavedDown()
        {
            _page++;
            _text.pageToDisplay = _page;
            StartCoroutine(TakeAnotherPause());
        }

        IEnumerator TakeAnotherPause()
        {
            _text.pageToDisplay = _page;
            yield return new WaitForSeconds(0.2f);
            StartNextScreenshot();
        }

        private void StartNextScreenshot()
        {
            if (_page == _text.textInfo.pageCount)
                StartScreenshot($"{PageName.Third}+{_page}");
            else
                StartScreenshot($"{PageName.Third}+{_page}", true);
        }     
    }
}

