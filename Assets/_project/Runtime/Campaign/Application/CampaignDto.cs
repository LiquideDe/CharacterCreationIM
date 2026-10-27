using System;
using System.Collections.Generic;

namespace CharacterCreation.Vtt.Campaign.Application
{
    public sealed class CampaignDto
    {
        public const int CurrentSchemaVersion = 1;

        public int SchemaVersion { get; set; } = CurrentSchemaVersion;

        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<Guid> MapIds { get; set; } = new List<Guid>();
    }
}
