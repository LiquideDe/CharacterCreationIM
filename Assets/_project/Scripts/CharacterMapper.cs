using ObservableCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CharacterCreation
{
    public static class CharacterMapper
    {
        static List<T> CopyOrEmpty<T>(IEnumerable<T> src) => src != null ? new List<T>(src) : new List<T>();
        static Dictionary<TKey, TVal> CopyOrEmpty<TKey, TVal>(IDictionary<TKey, TVal> src)
            => src != null ? new Dictionary<TKey, TVal>(src) : new Dictionary<TKey, TVal>();
        public static CharacterDto ToDto(Character c)
        {
            return new CharacterDto
            {
                Name = c.Name.Value,
                Experience = c.Experience.Value,

                Characteristics = CopyOrEmpty(c.Characteristics),
                Equipments = CopyOrEmpty(c.Equipments),
                PsyPowers = CopyOrEmpty(c.PsyPowers),
                Skills = CopyOrEmpty(c.Skills),
                Specializations = CopyOrEmpty(c.Specializations),
                Talents = CopyOrEmpty(c.Talents),
                Augmetics = CopyOrEmpty(c.Augmetics),
                Mutations = CopyOrEmpty(c.Mutations),
                Contacts = CopyOrEmpty(c.Contacts),
                Influence = CopyOrEmpty(c.Influence),

                Money = c.Money.Value,

                Origin = c.Origin.Value,
                Faction = c.Faction.Value,
                Role = c.Role.Value,
                FreeSmallPsyPower = c.FreeSmallPsyPower.Value,
                FreePsyPower = c.FreePsyPower.Value,
                Age = c.Age.Value,
                Eyes = c.Eyes.Value,
                HairColor = c.HairColor.Value,
                HairStyle = c.HairStyle.Value,
                Omen = c.Omen.Value,
                ShortTarget = c.ShortTarget.Value,
                LongTarget = c.LongTarget.Value,
                Connections = c.Connections.Value,
                TenQuestions = c.TenQuestions.Value,
                Prophecy = c.Prophecy.Value,
                Hand = c.Hand.Value,
                Height = c.Height.Value,
                Weight = c.Weight.Value,
                FatePoints = c.FatePoints.Value,
                CorruptionPoints = c.CorruptionPoints.Value
            };
        }

        public static void ApplyDto(Character c, CharacterDto d)
        {
            Debug.LogAssertion($"CharacterDto d ==null = {d == null}");
            Debug.LogAssertion($"ApplyDto d = {d.Name}");
            c.Name.Value = d.Name;
            c.Experience.Value = d.Experience;

            Replace(c.Characteristics, d.Characteristics);
            Replace(c.Equipments, d.Equipments);
            Replace(c.PsyPowers, d.PsyPowers);
            Replace(c.Skills, d.Skills);
            Replace(c.Specializations, d.Specializations);
            Replace(c.Talents, d.Talents);
            Replace(c.Augmetics, d.Augmetics);
            Replace(c.Mutations, d.Mutations);
            Replace(c.Contacts, d.Contacts);

            c.Influence.Clear();
            if (d.Influence != null)
                foreach (var kv in d.Influence) c.Influence[kv.Key] = kv.Value;

            c.Money.Value = d.Money;

            c.Origin.Value = d.Origin;
            c.Faction.Value = d.Faction;
            c.Role.Value = d.Role;
            c.FreeSmallPsyPower.Value = d.FreeSmallPsyPower;
            c.FreePsyPower.Value = d.FreePsyPower;
            c.Age.Value = d.Age;
            c.Eyes.Value = d.Eyes;
            c.HairColor.Value = d.HairColor;
            c.HairStyle.Value = d.HairStyle;
            c.Omen.Value = d.Omen;
            c.ShortTarget.Value = d.ShortTarget;
            c.LongTarget.Value = d.LongTarget;
            c.Connections.Value = d.Connections;
            c.TenQuestions.Value = d.TenQuestions;
            c.Prophecy.Value = d.Prophecy;
            c.Hand.Value = d.Hand;
            c.Height.Value = d.Height;
            c.Weight.Value = d.Weight;
            c.FatePoints.Value = d.FatePoints;
            c.CorruptionPoints.Value = d.CorruptionPoints;
        }

        private static void Replace<T>(ObservableList<T> target, List<T> src)
        {
            target.Clear();
            if (src != null && src.Count > 0)
                target.AddRange(src);
        }
    }
}

