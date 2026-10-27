using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CharacterCreation.Vtt.Shared;

namespace CharacterCreation.Vtt.Campaign.Domain
{
    public sealed class Campaign
    {
        public Campaign(EntityId id, string name, IEnumerable<EntityId> mapIds)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Campaign name is required.", nameof(name));
            }

            Id = id;
            Name = name;
            MapIds = new ReadOnlyCollection<EntityId>(new List<EntityId>(mapIds ?? throw new ArgumentNullException(nameof(mapIds))));
        }

        public EntityId Id { get; }

        public string Name { get; }

        public IReadOnlyList<EntityId> MapIds { get; }
    }
}
