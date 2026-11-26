using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CharacterCreation
{
    public sealed class PatronEffects
    {
        private readonly PatronCreator _creator;
        private readonly PatronStorage _storage;

        public PatronEffects(PatronCreator creator, PatronStorage storage)
        {
            _creator = creator;
            _storage = storage;
        }

        public Dictionary<string, int> CalculateInfluence(PatronTemplate template, string weaknessName, string possibilityName = null)
        {
            var result = new Dictionary<string, int>(template.influence ?? new Dictionary<string, int>());
            var usedPossibilities = new HashSet<string>(StringComparer.Ordinal);
            foreach (var name in new[] { template.opportunity, possibilityName })
            {
                if (string.IsNullOrWhiteSpace(name) || !usedPossibilities.Add(name)) continue;
                var option = _creator.Possibility(name);
                if (option?.influence_bonus == null) continue;
                foreach (var bonus in option.influence_bonus)
                {
                    if (string.IsNullOrWhiteSpace(bonus.Key) || bonus.Value < 0)
                    {
                        Debug.LogWarning($"[PatronEffects] Некорректный бонус влияния в возможности «{name}»: {bonus.Key}={bonus.Value}");
                        continue;
                    }
                    result.TryGetValue(bonus.Key, out var current);
                    result[bonus.Key] = current + bonus.Value;
                }
            }
            if (string.IsNullOrWhiteSpace(weaknessName)) return result;
            var weakness = _creator.Weakness(weaknessName);
            if (weakness == null)
            {
                Debug.LogWarning($"[PatronEffects] Неизвестная слабость: {weaknessName}");
                return result;
            }
            var factions = weakness.penalty_influence;
            var amounts = weakness.penalty_amount;
            if (factions == null || amounts == null || factions.Count != amounts.Count)
            {
                if ((factions?.Count ?? 0) != 0 || (amounts?.Count ?? 0) != 0)
                    Debug.LogWarning($"[PatronEffects] Некорректные штрафы влияния: {weaknessName}");
                return result;
            }
            for (var i = 0; i < factions.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(factions[i])) continue;
                result.TryGetValue(factions[i], out var current);
                result[factions[i]] = current + amounts[i];
            }
            return result;
        }

        public Patron ResolvePatron(Character character)
        {
            if (character == null || string.IsNullOrWhiteSpace(character.Patron.Value)) return null;
            if (!_storage.TryLoad(character.Patron.Value, out var patron))
            {
                Debug.LogWarning($"[PatronEffects] У персонажа «{character.Name.Value}» не найден покровитель «{character.Patron.Value}».");
                return null;
            }
            return patron;
        }

        public int SpecializationDiscount(Patron patron, string specialization)
        {
            if (patron == null) return 0;
            var discount = 0;
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var name in new[] { patron.Opportunity, patron.Possibility })
            {
                if (string.IsNullOrWhiteSpace(name) || !seen.Add(name)) continue;
                var option = _creator.Possibility(name);
                if (option == null)
                {
                    // Обязательная возможность может иметь описание только в шаблоне.
                    if (name != patron.Opportunity ||
                        _creator.Templates.FirstOrDefault(t => t.faction == patron.Faction && t.responsibilities == patron.Responsibility)?.opportunity != name)
                        Debug.LogWarning($"[PatronEffects] Неизвестная возможность: {name}");
                    continue;
                }
                if (option.specialization_sale == null || !option.specialization_sale.Contains(specialization)) continue;
                if (!option.amount_sale.HasValue || option.amount_sale.Value < 0)
                {
                    Debug.LogWarning($"[PatronEffects] Некорректная скидка в возможности: {name}");
                    continue;
                }
                discount += option.amount_sale.Value;
            }
            return discount;
        }

        public int PsyPowerDiscount(Patron patron)
        {
            if (patron == null) return 0;
            var discount = 0;
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var name in new[] { patron.Opportunity, patron.Possibility })
            {
                if (string.IsNullOrWhiteSpace(name) || !seen.Add(name)) continue;
                var option = _creator.Possibility(name);
                if (option == null) continue;
                if (!option.psy_sale.HasValue) continue;
                if (option.psy_sale.Value < 0)
                {
                    Debug.LogWarning($"[PatronEffects] Некорректная скидка на психосилы в возможности: {name}");
                    continue;
                }
                discount += option.psy_sale.Value;
            }
            return discount;
        }
    }
}
