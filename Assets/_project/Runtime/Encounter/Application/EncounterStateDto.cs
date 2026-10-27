using System;
using System.Collections.Generic;

namespace CharacterCreation.Vtt.Encounter.Application
{
    public sealed class EncounterStateDto
    {
        public const int CurrentSchemaVersion = 1;

        public int SchemaVersion { get; set; } = CurrentSchemaVersion;

        public Guid Id { get; set; }

        public Guid MapId { get; set; }

        public long Revision { get; set; }

        public List<ActorDto> Actors { get; set; } = new List<ActorDto>();

        public List<TokenDto> Tokens { get; set; } = new List<TokenDto>();

        public List<EffectInstanceDto> Effects { get; set; } = new List<EffectInstanceDto>();
    }

    public sealed class ActorDto
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public string Kind { get; set; } = string.Empty;
    }

    public sealed class TokenDto
    {
        public Guid Id { get; set; }

        public Guid ActorId { get; set; }

        public Guid ZoneId { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public bool IsVisibleToPlayers { get; set; }
    }

    public sealed class EffectInstanceDto
    {
        public Guid Id { get; set; }

        public string DefinitionKey { get; set; } = string.Empty;

        public Guid TargetActorId { get; set; }

        public Guid SourceActorId { get; set; }

        public int Stacks { get; set; }
    }
}
