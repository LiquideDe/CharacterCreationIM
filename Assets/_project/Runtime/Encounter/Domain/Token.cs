using CharacterCreation.Vtt.Shared;

namespace CharacterCreation.Vtt.Encounter.Domain
{
    public sealed class Token
    {
        public Token(EntityId id, EntityId actorId, EntityId zoneId, MapPoint position, bool isVisibleToPlayers)
        {
            Id = id;
            ActorId = actorId;
            ZoneId = zoneId;
            Position = position;
            IsVisibleToPlayers = isVisibleToPlayers;
        }

        public EntityId Id { get; }

        public EntityId ActorId { get; }

        public EntityId ZoneId { get; }

        public MapPoint Position { get; }

        public bool IsVisibleToPlayers { get; }
    }
}
