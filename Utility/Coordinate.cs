using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Utility
{
    public struct Coordinate
    {
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

        public long x;
        public long y;
        public long z;

        public static Coordinate operator+(Coordinate left, Coordinate right) => new Coordinate(left.x + right.x, left.y + right.y, left.z + right.z);
        public static Coordinate operator-(Coordinate left, Coordinate right) => new Coordinate(left.x - right.x, left.y - right.y, left.z - right.z);
        public static Coordinate operator*(Coordinate left, long right) => new Coordinate(left.x * right, left.y * right, left.z * right);
        public static Coordinate operator*(long left, Coordinate right) => new Coordinate(left * right.x, left * right.y, left * right.z);

        public override string ToString()
            => $"{this.x}, {this.y}, {this.z}";
    }
}
