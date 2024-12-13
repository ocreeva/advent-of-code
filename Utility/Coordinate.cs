namespace Moyba.AdventOfCode.Utility
{
    public struct Coordinate
    {
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
    }
}
