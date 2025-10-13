using R3;
using System;

namespace CharacterCreation
{
    [Serializable]
    public class Characteristic
    {
        public string Name;
        public int Level;
        public int BaseLevel;
        [NonSerialized] private Subject<int> _levelChanged;
        [Newtonsoft.Json.JsonIgnore]
        public Observable<int> LevelChanged => _levelChanged ??= new Subject<int>();

        public Characteristic(string name, int level)
        {
            Name = name;
            BaseLevel = level;
            Level = level;
        }

        public void PlusLevel(int value)
        {
            Level += value;
            _levelChanged?.OnNext(Level);
        }

        public void EmitCurrentLevel() => _levelChanged?.OnNext(Level);

        public void Dispose() => _levelChanged?.Dispose();
    }
}

