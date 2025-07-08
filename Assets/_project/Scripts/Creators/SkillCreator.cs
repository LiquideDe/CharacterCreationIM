using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace CharacterCreation
{
    public class SkillCreator : IDataCreator
    {
        private readonly List<SkillData> _skills = new();
        public IReadOnlyList<SkillData> Skills => _skills;

        private readonly List<SpecializationData> _specializations = new();

        public IReadOnlyList<SpecializationData> Specializations => _specializations;

        private Dictionary<string, SkillData> _skillByName = new Dictionary<string, SkillData>();
        private Dictionary<string, SpecializationData> _specializationByName = new Dictionary<string, SpecializationData>();

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            _skills.Clear();
            _specializations.Clear();
            string basePath = Path.Combine(Application.streamingAssetsPath, "Умения");

            string skillsPath = Path.Combine(basePath, "Умения.json");
            if (File.Exists(skillsPath))
            {
                string json = await File.ReadAllTextAsync(skillsPath, cancellationToken);
                SkillList skillList = JsonUtility.FromJson<SkillList>(json);
                if (skillList?.skills != null)                
                    _skills.AddRange(skillList.skills);
                
            }
            else
            {
                Debug.LogWarning($"Файл с умениями не найден: {skillsPath}");
            }
            
            string specializationsPath = Path.Combine(basePath, "Специализации.json");
            if (File.Exists(specializationsPath))
            {
                string json = await File.ReadAllTextAsync(specializationsPath, cancellationToken);
                SpecializationList specializationList = JsonUtility.FromJson<SpecializationList>(json);
                if (specializationList?.specializations != null)                
                    _specializations.AddRange(specializationList.specializations);
                
            }
            else
            {
                Debug.LogWarning($"Файл со специализациями не найден: {specializationsPath}");
            }

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
        public List<SkillData> skills;
    }

    [System.Serializable]
    public class SpecializationList
    {
        public List<SpecializationData> specializations;
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

