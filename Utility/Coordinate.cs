using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Utility
{
    public struct Coordinate
    {
        private static readonly Coordinate
            _North = new Coordinate(0, -1),
            _East  = new Coordinate(1, 0),
            _South = new Coordinate(0, 1),
            _West = new Coordinate(-1, 0),
            _Zero = new Coordinate(0, 0);
        private static readonly Coordinate[] _Orthogonal = [ _North, _East, _South, _West ];
        private static readonly Coordinate[] _Adjacent = [
            _North,
            new Coordinate(1, -1),
            _East,
            new Coordinate(1, 1),
            _South,
            new Coordinate(-1, 1),
            _West,
            new Coordinate(-1, -1)
        ];

        public Coordinate(Group x, Group y) : this(x.Value, y.Value) { }
        public Coordinate(Group x, Group y, Group z) : this(x.Value, y.Value, z.Value) { }
        public Coordinate(string x, string y) : this(Int64.Parse(x), Int64.Parse(y)) { }
        public Coordinate(string x, string y, string z) : this(Int64.Parse(x), Int64.Parse(y), Int64.Parse(z)) { }
        public Coordinate(long x, long y) : this(x, y, 0) {}
        public Coordinate(long x, long y, long z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Coordinate North => _North;
        public static Coordinate East => _East;
        public static Coordinate South => _South;
        public static Coordinate West => _West;
        public static Coordinate Zero => _Zero;

        public IEnumerable<Coordinate> Adjacent => _Adjacent.Select(_Sum);
        public IEnumerable<Coordinate> Orthogonal => _Orthogonal.Select(_Sum);

        public long x;
        public long y;
        public long z;

        public static Coordinate operator+(Coordinate left, Coordinate right) => new Coordinate(left.x + right.x, left.y + right.y, left.z + right.z);
        public static Coordinate operator-(Coordinate left, Coordinate right) => new Coordinate(left.x - right.x, left.y - right.y, left.z - right.z);
        public static Coordinate operator*(Coordinate left, long right) => new Coordinate(left.x * right, left.y * right, left.z * right);
        public static Coordinate operator*(long left, Coordinate right) => new Coordinate(left * right.x, left * right.y, left * right.z);
        public static bool operator==(Coordinate left, Coordinate right) => left.x == right.x && left.y == right.y && left.z == right.z;
        public static bool operator!=(Coordinate left, Coordinate right) => left.x != right.x || left.y != right.y || left.z != right.z;
        public static Coordinate operator-(Coordinate coordinate) => new Coordinate(-coordinate.x, -coordinate.y, -coordinate.z);

        public override bool Equals([NotNullWhen(true)] object? obj)
            => obj is Coordinate c && this == c;

        public override int GetHashCode()
            => HashCode.Combine(this.x, this.y, this.z);

        public override string ToString()
            => $"{this.x}, {this.y}, {this.z}";

        private Coordinate _Sum(Coordinate c) => this + c;
    }
}
