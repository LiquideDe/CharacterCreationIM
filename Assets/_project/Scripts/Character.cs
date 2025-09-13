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
        private ObservableList<PsyData> _psyPowers = new ObservableList<PsyData>();
        private ObservableList<SkillData> _skills = new ObservableList<SkillData>();
        private ObservableList<SpecializationData> _specializations = new ObservableList<SpecializationData>();
        private ObservableList<TalentData> _talents = new ObservableList<TalentData>();
        private ObservableList<AugmeticData> _augmetics = new ObservableList<AugmeticData>();
        private Dictionary<string, int> _influence = new Dictionary<string, int>();
        private ObservableList<string> _contacts = new ObservableList<string>();
        public ReactiveProperty<string> Name { get; set; } = new ();
        public ReactiveProperty<Experience> Experience => _experience;
        public IObservable<Experience> ExperienceChanged => (IObservable<Experience>)_experience;
        public ObservableList<Characteristic> Characteristics => _characteristics;
        public ObservableList<EquipmentData> Equipments => _equipments;
        public ObservableList<SkillData> Skills => _skills;
        public ObservableList<SpecializationData> Specializations => _specializations;
        public ObservableList<TalentData> Talents => _talents;
        public ObservableList<AugmeticData> Augmetics => _augmetics;
        public ObservableList<PsyData> PsyPowers => _psyPowers;
        public Dictionary<string, int> Influence => _influence;
        public ObservableList<string> Contacts => _contacts;
        public ReactiveProperty<int> Money { get; } = new();

        public ReactiveProperty<string> Origin { get; set; } = new();
        public ReactiveProperty<string> Faction { get; set; } = new();
        public ReactiveProperty<string> Role { get; set; } = new();
        public ReactiveProperty<int> FreeSmallPsyPower { get; set; } = new();
        public ReactiveProperty<int> FreePsyPower { get; set; } = new();
        public ReactiveProperty<int> Age { get; set; } = new();
        public ReactiveProperty<string> Eyes { get; set; } = new();
        public ReactiveProperty<string> HairColor { get; set; } = new();
        public ReactiveProperty<string> HairStyle { get; set; } = new();
        public ReactiveProperty<string> Omen { get; set; } = new();
        public ReactiveProperty<string> ShortTarget { get; set; } = new();
        public ReactiveProperty<string> LongTarget { get; set; } = new();
        public ReactiveProperty<string> Connections { get; set; } = new();         
        public ReactiveProperty<string> TenQuestions { get; set; } = new();

        public UndoRedoManager CharacteristicHistory = new();
    }

    [Serializable]
    public class  Experience
    {
        public int experiencePoints;
        public int experienceSpent;
    }
}

