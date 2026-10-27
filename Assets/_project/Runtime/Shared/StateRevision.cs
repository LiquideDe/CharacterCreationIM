using System;

namespace CharacterCreation.Vtt.Shared
{
    public readonly struct StateRevision : IEquatable<StateRevision>, IComparable<StateRevision>
    {
        public StateRevision(long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "State revision cannot be negative.");
            }

            Value = value;
        }

        public long Value { get; }

        public static StateRevision Initial => new StateRevision(0);

        public StateRevision Next()
        {
            if (Value == long.MaxValue)
            {
                throw new InvalidOperationException("State revision limit has been reached.");
            }

            return new StateRevision(Value + 1);
        }

        public int CompareTo(StateRevision other)
        {
            return Value.CompareTo(other.Value);
        }

        public bool Equals(StateRevision other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is StateRevision other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
