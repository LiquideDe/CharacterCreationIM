using System;

namespace CharacterCreation.Vtt.Shared
{
    public readonly struct MapPoint : IEquatable<MapPoint>
    {
        public MapPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }

        public double Y { get; }

        public bool Equals(MapPoint other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            return obj is MapPoint other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(MapPoint left, MapPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MapPoint left, MapPoint right)
        {
            return !left.Equals(right);
        }
    }
}
