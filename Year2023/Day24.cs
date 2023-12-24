using System.Numerics;

namespace Moyba.AdventOfCode.Year2023
{
    using Hailstone = ((BigInteger x, BigInteger y, BigInteger z) position, (BigInteger x, BigInteger y, BigInteger z) velocity);

    public class Day24(string[] _data) : IPuzzle
    {
        private const bool _IsExample = false;
        private const long _LowerBound = _IsExample ? 7 : 200_000_000_000_000;
        private const long _UpperBound = _IsExample ? 27 : 400_000_000_000_000;

        private static readonly char[] _DataSplitCharacters = [ ',', '@' ];

        private readonly Hailstone[] _hailstones = _data
            .Select(_ => _.Split(_DataSplitCharacters, StringSplitOptions.TrimEntries).Select(BigInteger.Parse).ToArray())
            .Select(_ => ((_[0], _[1], _[2]), (_[3], _[4], _[5])))
            .ToArray();

        [PartOne("23760")]
        [PartTwo()]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            // x = p1.x + v1.x * t
            // y = p1.y + v1.y * t

            // y = p1.y + v1.y * (x - p1.x) / v1.x
            // y = p2.y + v2.y * (x - p2.x) / v2.x

            // x * (v1.y * v2.x - v2.y * v1.x) = v1.x * v2.x * (p2.y - p1.y) + p1.x * v1.y * v2.x - p2.x * v2.y * v1.x

            foreach (var hailstone in _hailstones)
            {
                if (hailstone.velocity.x == 0) Console.WriteLine($"0 x velocity");
                if (hailstone.velocity.y == 0) Console.WriteLine($"0 y velocity");
                if (hailstone.velocity.z == 0) Console.WriteLine($"0 z velocity");
            }

            var intersectionXYCount = 0;
            for (var index1 = 0; index1 < _hailstones.Length - 1; index1++)
            {
                var h1 = _hailstones[index1];
                for (var index2 = index1 + 1; index2 < _hailstones.Length; index2++)
                {
                    var h2 = _hailstones[index2];
                    if (_TryFindIntersection(h1, h2, out var x, out var y, out var t1, out var t2))
                    {
                        if (t1 < 0 || t2 < 0) continue;
                        if (x < _LowerBound || x > _UpperBound || y < _LowerBound || y > _UpperBound) continue;

                        intersectionXYCount++; 
                    }
                }
            }

            yield return $"{intersectionXYCount}";

            await Task.CompletedTask;
        }

        private static bool _TryFindIntersection(Hailstone h1, Hailstone h2, out double x, out double y, out double t1, out double t2)
        {
            x = 0.0;
            y = 0.0;
            t1 = 0.0;
            t2 = 0.0;

            (var p1, var v1) = h1;
            (var p2, var v2) = h2;

            var velocityCrossProduct = v1.y * v2.x - v2.y * v1.x;
            var xComputation = v1.x * v2.x * (p2.y - p1.y) + p1.x * v1.y * v2.x - p2.x * v2.y * v1.x;
            var yComputation = v1.y * v2.y * (p1.x - p2.x) - p1.y * v1.x * v2.y + p2.y * v2.x * v1.y;

            if (velocityCrossProduct == 0)
            {
                if (xComputation == 0) throw new Exception("Parallel but maybe intersecting, need to implement.");
                if (yComputation == 0) throw new Exception("Parallel but maybe intersecting, need to implement.");
                return false;
            }

            x = 1.0 * (double)xComputation / (double)velocityCrossProduct;
            y = 1.0 * (double)yComputation / (double)velocityCrossProduct;

            // x = p.x + t * v.x
            // t = (x - p.x) / v.x

            t1 = (x - (double)p1.x) / (double)v1.x;
            t2 = (x - (double)p2.x) / (double)v2.x;

            return true;
        }
    }
}
