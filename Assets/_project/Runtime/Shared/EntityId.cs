using System;

namespace CharacterCreation.Vtt.Shared
{
    public readonly struct EntityId : IEquatable<EntityId>
    {
        public EntityId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("Entity ID cannot be empty.", nameof(value));
            }

            Value = value;
        }

        public Guid Value { get; }

        public static EntityId New()
        {
            return new EntityId(Guid.NewGuid());
        }

        public static EntityId Parse(string value)
        {
            return new EntityId(Guid.Parse(value));
        }

        public bool Equals(EntityId other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is EntityId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString("N");
        }

        public static bool operator ==(EntityId left, EntityId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityId left, EntityId right)
        {
            return !left.Equals(right);
        }
    }
}
