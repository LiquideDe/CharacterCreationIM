using System;
using System.Collections.Generic;

namespace CharacterCreation
{
    [Serializable]
    public sealed class Patron
    {
        public string Name;
        public string Faction;
        public string Responsibility;
        public string Opportunity;
        public string Possibility;
        public List<string> PossibilityVariants = new();
        public string Weakness;
        public string Aspiration;
        public string Personality;
        public Dictionary<string, int> Influence = new();
    }

    [Serializable]
    public sealed class PatronTemplate
    {
        public string faction;
        public string responsibilities;
        public string description;
        public string opportunity;
        public string description_oppotrunity;
        public List<string> possibilities;
        public List<string> weakness;
        public Dictionary<string, int> influence;
    }

    [Serializable]
    public sealed class PatronOption
    {
        public string name;
        public string description;
        public string additional_influence;
        public Dictionary<string, int> influence_bonus;
        public List<string> specialization_sale;
        public int? amount_sale;
        public int? psy_sale;
        public List<string> penalty_influence;
        public List<int> penalty_amount;
        public bool repeatable;
        public List<PatronVariant> variants;
    }

    [Serializable]
    public sealed class PatronVariant
    {
        public string name;
        public string description;
    }
}
