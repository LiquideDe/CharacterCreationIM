using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreation
{
    public class Character
    {
        private List<Characteristic> _characteristics = new List<Characteristic>();
        public string Name { get; set; }
        public Experience Experience { get; set; }
        public List<Characteristic> Characteristics => _characteristics;
    }

    [Serializable]
    public class  Experience
    {
        public int experiencePoints;
        public int experienceSpent;
    }
}

