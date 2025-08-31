using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreation
{
    public class Character
    {
        private List<Characteristic> _characteristics = new List<Characteristic>();
        private List<EquipmentData> _equipments = new List<EquipmentData>();
        private List<SkillData> _skills = new List<SkillData>();
        private List<TalentData> _talents = new List<TalentData>();
        private List<AugmeticData> _augmetics = new List<AugmeticData>();
        private Dictionary<string, int> _influence = new Dictionary<string, int>();
        private List<string> _contacts = new List<string>();
        public string Name { get; set; }        
        public Experience Experience { get; set; }
        public List<Characteristic> Characteristics => _characteristics;    
        public List<EquipmentData> Equipments => _equipments;
        public List<SkillData> Skills => _skills;
        public List<TalentData> Talents => _talents;
        public List<AugmeticData> Augmetics => _augmetics;
        public Dictionary<string, int> Influence => _influence;
        public List<string> Contacts => _contacts;
        public int Money { get; set; }

        public string Origin { get; set; }
        public string Faction { get; set; }
    }

    [Serializable]
    public class  Experience
    {
        public int experiencePoints;
        public int experienceSpent;
    }
}

