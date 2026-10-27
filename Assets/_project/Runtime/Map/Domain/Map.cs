using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CharacterCreation.Vtt.Shared;

namespace CharacterCreation.Vtt.Map.Domain
{
    public sealed class Map
    {
        public Map(
            EntityId id,
            string name,
            string imageContentHash,
            StateRevision revision,
            IEnumerable<Zone> zones)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Map name is required.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(imageContentHash))
            {
                throw new ArgumentException("Map image content hash is required.", nameof(imageContentHash));
            }

            Id = id;
            Name = name;
            ImageContentHash = imageContentHash;
            Revision = revision;
            Zones = new ReadOnlyCollection<Zone>(new List<Zone>(zones ?? throw new ArgumentNullException(nameof(zones))));
        }

        public EntityId Id { get; }

        public string Name { get; }

        public string ImageContentHash { get; }

        public StateRevision Revision { get; }

        public IReadOnlyList<Zone> Zones { get; }
    }
}
