using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreation
{
    [Serializable]
    public class CharacterDto
    {
        public string Name;
        public Experience Experience;

        public List<Characteristic> Characteristics { get; set; }
        public List<EquipmentData> Equipments { get; set; }
        public List<PsyData> PsyPowers { get; set; }
        public List<SkillData> Skills { get; set; }
        public List<SpecializationData> Specializations { get; set; }
        public List<TalentData> Talents { get; set; }
        public List<AugmeticData> Augmetics { get; set; }
        public List<Mutation> Mutations { get; set; }
        public List<string> Contacts { get; set; }
        public Dictionary<string, int> Influence { get; set; }

        public int Money;

        public string Origin;
        public string Faction;
        public string Role;

        public int FreeSmallPsyPower;
        public int FreePsyPower;

        public int Age;
        public string Eyes;
        public string HairColor;
        public string HairStyle;
        public string Omen;
        public string ShortTarget;
        public string LongTarget;
        public string Connections;
        public string TenQuestions;
        public string Prophecy;
        public string Hand;
        public int Height;
        public int Weight;
        public int FatePoints;
        public int CorruptionPoints;

        public CharacterDto()
        {
            Characteristics = new();
            Equipments = new();
            PsyPowers = new();
            Skills = new();
            Specializations = new();
            Talents = new();
            Augmetics = new();
            Mutations = new();
            Contacts = new();
            Influence = new();
        }
    }
}

