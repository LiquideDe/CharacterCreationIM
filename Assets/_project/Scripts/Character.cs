using ObservableCollections;
using R3;
using R3.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreation
{
    public class Character
    {
        private readonly ReactiveProperty<Experience> _experience = new();
        private readonly ObservableList<Characteristic> _characteristics = new();
        private ObservableList<EquipmentData> _equipments = new ObservableList<EquipmentData>();
        private ObservableList<SkillData> _skills = new ObservableList<SkillData>();
        private ObservableList<TalentData> _talents = new ObservableList<TalentData>();
        private ObservableList<AugmeticData> _augmetics = new ObservableList<AugmeticData>();
        private Dictionary<string, int> _influence = new Dictionary<string, int>();
        private ObservableList<string> _contacts = new ObservableList<string>();
        public ReactiveProperty<string> Name { get; set; } = new ();
        public Experience Experience
        {
            get => _experience.Value;
            set => _experience.Value = value;
        }
        public IObservable<Experience> ExperienceChanged => (IObservable<Experience>)_experience;
        public ObservableList<Characteristic> Characteristics => _characteristics;
        public ObservableList<EquipmentData> Equipments => _equipments;
        public ObservableList<SkillData> Skills => _skills;
        public ObservableList<TalentData> Talents => _talents;
        public ObservableList<AugmeticData> Augmetics => _augmetics;
        public Dictionary<string, int> Influence => _influence;
        public ObservableList<string> Contacts => _contacts;
        public ReactiveProperty<int> Money { get; } = new();

        public ReactiveProperty<string> Origin { get; set; } = new();
        public ReactiveProperty<string> Faction { get; set; } = new();
        public ReactiveProperty<string> Role { get; set; } = new();
    }

    [Serializable]
    public class  Experience
    {
        public int experiencePoints;
        public int experienceSpent;
    }
}

