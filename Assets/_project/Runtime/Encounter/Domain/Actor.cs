using System;
using CharacterCreation.Vtt.Shared;

namespace CharacterCreation.Vtt.Encounter.Domain
{
    public enum ActorKind
    {
        PlayerCharacter = 0,
        NonPlayerCharacter = 1
    }

    public sealed class Actor
    {
        public Actor(EntityId id, string displayName, ActorKind kind)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Actor display name is required.", nameof(displayName));
            }

            Id = id;
            DisplayName = displayName;
            Kind = kind;
        }

        public EntityId Id { get; }

        public string DisplayName { get; }

        public ActorKind Kind { get; }
    }
}
