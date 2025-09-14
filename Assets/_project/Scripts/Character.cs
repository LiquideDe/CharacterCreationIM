using ObservableCollections;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private ObservableList<Mutation> _mutations = new ObservableList<Mutation>();
        private Dictionary<string, int> _influence = new Dictionary<string, int>();
        private ObservableList<string> _contacts = new ObservableList<string>();
        public ReactiveProperty<string> Name { get; set; } = new ();
        public ReactiveProperty<Experience> Experience => _experience;
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
        public ReactiveProperty<string> Prophecy { get; set; } = new();
        public ReactiveProperty<string> Hand { get; set; } = new();
        public ReactiveProperty<int> Height { get; set; } = new();
        public ReactiveProperty<int> Weight { get; set; } = new();
        public ReactiveProperty<int> FatePoints { get; set; } = new();
        public ReactiveProperty<int> CorruptionPoints { get; set; } = new();
        public ObservableList<Mutation> Mutations => _mutations;

        public UndoRedoManager CharacteristicHistory = new();

        private CompositeDisposable _cd = new CompositeDisposable();

        public bool IsPsyker { get; private set; }
        
        public Character()
        {
            Talents.ObserveAdd()
            .Subscribe(e =>
            {
                RecalcIsPsyker();
            }, ex => Debug.LogException(ex.Exception))
            .AddTo(_cd);

            Talents.ObserveRemove()
                .Subscribe(_ => RecalcIsPsyker(), ex => Debug.LogException(ex.Exception))
                .AddTo(_cd);

            Talents.ObserveReplace()
                .Subscribe(_ => RecalcIsPsyker(), ex => Debug.LogException(ex.Exception))
                .AddTo(_cd);

            Talents.ObserveReset()
                .Subscribe(_ => RecalcIsPsyker(), ex => Debug.LogException(ex.Exception))
                .AddTo(_cd);

        }

        public void Release()
        {
            _cd.Dispose();
        }

        private void RecalcIsPsyker()
        {
            IsPsyker = Talents.Any(t => string.Compare(t.name,"Псайкер",true)== 0);
        }
    }

    [Serializable]
    public class  Experience
    {
        public int experiencePoints;
        public int experienceSpent;
    }

    [Serializable]
    public class Mutation
    {
        public string name;
        public string description;
    }
}

