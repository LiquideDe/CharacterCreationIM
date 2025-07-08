using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreation
{
    public class Specialization : ISimpleData
    {
        public Specialization(SpecializationData specializationData)
        {
            Name = specializationData.name;
            Description = specializationData.description;
            SkillName = specializationData.skill;
            RequireTalentName = specializationData.requireTalent;
            SpecialSpecialization = specializationData.specialSpecialization;
            RequireSkill = specializationData.requireSkill;
            LvlRequireSkill = specializationData.lvlRequireSkill;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string SkillName { get; private set; }
        public string RequireTalentName { get; private set; }
        public bool SpecialSpecialization {  get; private set; }
        public string RequireSkill {  get; private set; }
        public int LvlRequireSkill { get; private set; }
    }
}

