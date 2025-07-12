using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace CharacterCreation
{
    public class SkillCreator : DataCreator,IDataCreator
    {
        private readonly List<SkillData> _skills = new();       
        private readonly List<SpecializationData> _specializations = new();
        private Dictionary<string, SkillData> _skillByName = new Dictionary<string, SkillData>();
        private Dictionary<string, SpecializationData> _specializationByName = new Dictionary<string, SpecializationData>();

        public IReadOnlyList<SkillData> Skills => _skills;
        public IReadOnlyList<SpecializationData> Specializations => _specializations;
        public SkillData SkillByName(string name) => _skillByName[name];
        public SpecializationData SpecializationByName(string name) => _specializationByName[name];
        

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


    }

    [System.Serializable]
    public class SkillData
    {
        public string name;
        public string characteristic;
        public string description;
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
    }
}

