using System;

namespace CharacterCreation
{
    [Serializable]
    public class Characteristic
    {
        public string Name;
        public int Level;

        public Characteristic(string name, int level)
        {
            Name = name;
            Level = level;
        }
    }
}

