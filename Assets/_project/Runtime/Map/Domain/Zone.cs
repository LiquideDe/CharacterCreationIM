using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CharacterCreation.Vtt.Shared;

namespace CharacterCreation.Vtt.Map.Domain
{
    public enum ZoneVisibility
    {
        Hidden = 0,
        Revealed = 1
    }

    public sealed class Zone
    {
        public Zone(
            EntityId id,
            string name,
            IEnumerable<MapPoint> boundary,
            ZoneVisibility visibility,
            IEnumerable<string> effectKeys)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Zone name is required.", nameof(name));
            }

            var boundaryPoints = new List<MapPoint>(boundary ?? throw new ArgumentNullException(nameof(boundary)));
            if (boundaryPoints.Count < 3)
            {
                throw new ArgumentException("A zone boundary requires at least three points.", nameof(boundary));
            }

            Id = id;
            Name = name;
            Boundary = new ReadOnlyCollection<MapPoint>(boundaryPoints);
            Visibility = visibility;
            EffectKeys = new ReadOnlyCollection<string>(new List<string>(effectKeys ?? throw new ArgumentNullException(nameof(effectKeys))));
        }

        public EntityId Id { get; }

        public string Name { get; }

        public IReadOnlyList<MapPoint> Boundary { get; }

        public ZoneVisibility Visibility { get; }

        public IReadOnlyList<string> EffectKeys { get; }
    }
}
