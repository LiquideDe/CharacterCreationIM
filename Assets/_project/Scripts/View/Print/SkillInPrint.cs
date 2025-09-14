using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CharacterCreation
{
    public enum SkillName
    {
        Atletica, Bditelnost, LovkostRuk, Disciplin, Stokost, Chut, Yasiki, Logic, Znania, Medica, Boy,
        Orientirovanie, Pilotirov, Command, PsyMaster, Strelba, Vzaimootn, Refleks,
        Skritnost, Tech
    }
    public class SkillInPrint : MonoBehaviour
    {
        [field: SerializeField] public SkillName SkillName;
        [SerializeField] private TextMeshProUGUI text;

        public void SetText(int amount) => text.text = amount.ToString();
    }
}

