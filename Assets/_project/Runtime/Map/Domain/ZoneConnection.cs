using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CharacterCreation.Vtt.Shared;

namespace CharacterCreation.Vtt.Map.Domain
{
    /// <summary>
    /// A derived navigable connection between two zones that share one or more boundary segments.
    /// It is computed from zone geometry and is not authored as a separate wall or door.
    /// </summary>
    public sealed class ZoneConnection
    {
        public ZoneConnection(EntityId firstZoneId, EntityId secondZoneId, IEnumerable<MapPoint> sharedBoundary)
        {
            if (firstZoneId == secondZoneId)
            {
                throw new ArgumentException("A zone connection must join two different zones.");
            }

            var points = new List<MapPoint>(sharedBoundary ?? throw new ArgumentNullException(nameof(sharedBoundary)));
            if (points.Count < 2)
            {
                throw new ArgumentException("A shared boundary requires at least two points.", nameof(sharedBoundary));
            }

            FirstZoneId = firstZoneId;
            SecondZoneId = secondZoneId;
            SharedBoundary = new ReadOnlyCollection<MapPoint>(points);
        }

        public EntityId FirstZoneId { get; }

        public EntityId SecondZoneId { get; }

        public IReadOnlyList<MapPoint> SharedBoundary { get; }
    }
}
