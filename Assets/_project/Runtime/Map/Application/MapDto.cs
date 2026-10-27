using System;
using System.Collections.Generic;

namespace CharacterCreation.Vtt.Map.Application
{
    public sealed class MapDto
    {
        public const int CurrentSchemaVersion = 1;

        public int SchemaVersion { get; set; } = CurrentSchemaVersion;

        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string ImageContentHash { get; set; } = string.Empty;

        public long Revision { get; set; }

        public List<ZoneDto> Zones { get; set; } = new List<ZoneDto>();
    }

    public sealed class ZoneDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Visibility { get; set; } = string.Empty;

        public List<MapPointDto> Boundary { get; set; } = new List<MapPointDto>();

        public List<string> EffectKeys { get; set; } = new List<string>();
    }

    public sealed class MapPointDto
    {
        public double X { get; set; }

        public double Y { get; set; }
    }
}
