using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace CharacterCreation
{
    public class SkillCreator : DataCreator, IDataCreator
    {
        private readonly List<SkillData> _skills = new();       
        private readonly List<SpecializationData> _specializations = new();
        private Dictionary<string, SkillData> _skillByName = new Dictionary<string, SkillData>();
        private Dictionary<string, SpecializationData> _specializationByName = new Dictionary<string, SpecializationData>();

        public IReadOnlyList<SkillData> Skills => _skills;
        public IReadOnlyList<SpecializationData> Specializations => _specializations;
        public SkillData SkillByName(string name) => _skillByName.TryGetValue(name, out var data) ? data : null;
        public SpecializationData SpecializationByName(string name) => _specializationByName.TryGetValue(name, out var data) ? data : null;


        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            _skills.Clear();
            _specializations.Clear();
            string basePath = Path.Combine(Application.streamingAssetsPath, "Умения");

            var tasks = new List<UniTask>()
            {
                LoadAndAddAsync<SkillList, SkillData>("Умения.json",
                _skills, cancellationToken, list => list.data,basePath),

                LoadAndAddAsync<SpecializationList, SpecializationData>("Специализации.json",
                _specializations, cancellationToken, list => list.data,basePath),
            };

            await UniTask.WhenAll(tasks);

            foreach (var item in _skills)            
                _skillByName.Add(item.name, item);

            foreach (var item in _specializations)            
                _specializationByName.Add(item.name, item);     
        }

        public List<string> GetSpecializations(string nameSkill)
        {
            var list = new List<string>();
            foreach (var item in _specializations)            
                if (string.Compare(nameSkill, item.skill) == 0)                
                    list.Add(item.name);

            return list;
        }
    }

    public class SkillProvider : INameProvider
    {
        private SkillCreator _skillCreator;
        public Type ItemType => typeof(SkillData);
        public SkillProvider(SkillCreator skillCreator)
        {
            _skillCreator = skillCreator;
        }
        public bool TryGet(string name, out object value)
        {
            value = _skillCreator.SkillByName(name); 
            return value != null;
        }
    }

    public class SpecializationProvider : INameProvider
    {
        private SkillCreator _skillCreator;
        public Type ItemType => typeof(SpecializationData);
        public SpecializationProvider(SkillCreator skillCreator)
        {
            _skillCreator = skillCreator;
        }
        public bool TryGet(string name, out object value)
        {
            value = _skillCreator.SpecializationByName(name); 
            return value != null;
        }
    }

    [System.Serializable]
    public class SkillData
    {
        public string name;
        public string characteristic;
        public string description;
        public int level;

        [NonSerialized] private Subject<int> _levelChanged;
        [Newtonsoft.Json.JsonIgnore]
        public Observable<int> LevelChanged => _levelChanged ??= new Subject<int>();

        public void PlusLevel(int value)
        {
            level += value;
            _levelChanged?.OnNext(level);
        }
    }

    [System.Serializable]
    public class SkillList
    {
        public List<SkillData> data;
    }

    [System.Serializable]
    public class SpecializationList
    {
        public List<SpecializationData> data;
    }

    [System.Serializable]
    public class SpecializationData
    {
        public string name;
        public string skill;
        public string description;
        public string requireTalent;
        public bool specialSpecialization;
        public string requireSkill;
        public int lvlRequireSkill;
        public int level;

        [NonSerialized] private Subject<int> _levelChanged;
        [Newtonsoft.Json.JsonIgnore]
        public Observable<int> LevelChanged => _levelChanged ??= new Subject<int>();

        public void PlusLevel(int value)
        {
            level += value;
            _levelChanged?.OnNext(level);
        }
    }
}

