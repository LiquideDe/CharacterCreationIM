using System;
using CharacterCreation.Vtt.Shared;

namespace CharacterCreation.Vtt.Encounter.Domain
{
    public sealed class EffectInstance
    {
        public EffectInstance(EntityId id, string definitionKey, EntityId targetActorId, EntityId sourceActorId, int stacks)
        {
            if (string.IsNullOrWhiteSpace(definitionKey))
            {
                throw new ArgumentException("Effect definition key is required.", nameof(definitionKey));
            }

            if (stacks < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(stacks), "Effect stacks must be at least one.");
            }

            Id = id;
            DefinitionKey = definitionKey;
            TargetActorId = targetActorId;
            SourceActorId = sourceActorId;
            Stacks = stacks;
        }

        public EntityId Id { get; }

        public string DefinitionKey { get; }

        public EntityId TargetActorId { get; }

        public EntityId SourceActorId { get; }

        public int Stacks { get; }
    }
}
