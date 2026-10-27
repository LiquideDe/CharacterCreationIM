using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CharacterCreation.Vtt.Shared;

namespace CharacterCreation.Vtt.Encounter.Domain
{
    public sealed class EncounterState
    {
        public EncounterState(
            EntityId id,
            EntityId mapId,
            StateRevision revision,
            IEnumerable<Actor> actors,
            IEnumerable<Token> tokens,
            IEnumerable<EffectInstance> effects)
        {
            Id = id;
            MapId = mapId;
            Revision = revision;
            Actors = new ReadOnlyCollection<Actor>(new List<Actor>(actors ?? throw new ArgumentNullException(nameof(actors))));
            Tokens = new ReadOnlyCollection<Token>(new List<Token>(tokens ?? throw new ArgumentNullException(nameof(tokens))));
            Effects = new ReadOnlyCollection<EffectInstance>(new List<EffectInstance>(effects ?? throw new ArgumentNullException(nameof(effects))));
        }

        public EntityId Id { get; }

        public EntityId MapId { get; }

        public StateRevision Revision { get; }

        public IReadOnlyList<Actor> Actors { get; }

        public IReadOnlyList<Token> Tokens { get; }

        public IReadOnlyList<EffectInstance> Effects { get; }
    }
}
