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

        public List<Characteristic> Characteristics;
        public List<EquipmentData> Equipments;
        public List<PsyData> PsyPowers;
        public List<SkillData> Skills;
        public List<SpecializationData> Specializations;
        public List<TalentData> Talents;
        public List<AugmeticData> Augmetics;
        public List<Mutation> Mutations;
        public List<string> Contacts;
        public Dictionary<string, int> Influence;

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
    }
}

