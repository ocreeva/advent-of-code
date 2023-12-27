using System.Numerics;
using Moyba.AdventOfCode.Utility;

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
        [PartTwo("888708704663413")]
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

            var coefficients = Enumerable.Range(0, _hailstones.Length - 1)
                .SelectMany(index1 => Enumerable.Range(index1 + 1, _hailstones.Length - index1 - 1)
                    .Select<int, BigInteger[]>(index2 => [
                        _hailstones[index2].velocity.y - _hailstones[index1].velocity.y,
                        _hailstones[index1].velocity.x - _hailstones[index2].velocity.x,
                        _hailstones[index1].position.y - _hailstones[index2].position.y,
                        _hailstones[index2].position.x - _hailstones[index1].position.x
                    ]))
                .ToArray();
            var sum = Enumerable.Range(0, _hailstones.Length - 1)
                .SelectMany(index1 => Enumerable.Range(index1 + 1, _hailstones.Length - index1 - 1)
                    .Select(index2 => 
                        _hailstones[index1].velocity.x * _hailstones[index1].position.y +
                        _hailstones[index2].velocity.y * _hailstones[index2].position.x -
                        _hailstones[index1].velocity.y * _hailstones[index1].position.x -
                        _hailstones[index2].velocity.x * _hailstones[index2].position.y))
                .ToArray();

            var xyResults = _Reduce(coefficients, sum);

            coefficients = Enumerable.Range(0, _hailstones.Length - 1)
                .SelectMany(index1 => Enumerable.Range(index1 + 1, _hailstones.Length - index1 - 1)
                    .Select<int, BigInteger[]>(index2 => [
                        _hailstones[index2].velocity.z - _hailstones[index1].velocity.z,
                        _hailstones[index1].velocity.x - _hailstones[index2].velocity.x,
                        _hailstones[index1].position.z - _hailstones[index2].position.z,
                        _hailstones[index2].position.x - _hailstones[index1].position.x
                    ]))
                .ToArray();
            sum = Enumerable.Range(0, _hailstones.Length - 1)
                .SelectMany(index1 => Enumerable.Range(index1 + 1, _hailstones.Length - index1 - 1)
                    .Select(index2 => 
                        _hailstones[index1].velocity.x * _hailstones[index1].position.z +
                        _hailstones[index2].velocity.z * _hailstones[index2].position.x -
                        _hailstones[index1].velocity.z * _hailstones[index1].position.x -
                        _hailstones[index2].velocity.x * _hailstones[index2].position.z))
                .ToArray();

            var xzResults = _Reduce(coefficients, sum);

            yield return $"{xyResults[0] + xyResults[1] + xzResults[1]}";

            await Task.CompletedTask;
        }

        private static BigInteger[] _Reduce(BigInteger[][] coefficients, BigInteger[] sum, int coefficientIndex = 0)
        {
            var height = coefficients.Length;
            var width = coefficients[coefficientIndex].Length;

            if (height < width) throw new Exception("Too few rows.");

            if (coefficientIndex == width)
            {
                return new BigInteger[width];
            }

            // reduce the coefficients by even multiples, if possible
            for (var formulaIndex = coefficientIndex; formulaIndex < height; formulaIndex++)
            {
                var gcd = GCD.Calculate(sum[formulaIndex], GCD.Calculate(coefficients[formulaIndex]));
                if (coefficients[formulaIndex][coefficientIndex] < 0) gcd = -gcd;
                if (gcd != 1)
                {
                    sum[formulaIndex] /= gcd;
                    for (var index = 0; index < width; coefficients[formulaIndex][index++] /= gcd);
                }
            }

            var minValue = coefficients.Select(_ => _[coefficientIndex]).Where(_ => _ > 0).Min();
            for (var formulaIndex = coefficientIndex; formulaIndex < height; formulaIndex++)
            {
                if (coefficients[formulaIndex][coefficientIndex] == minValue)
                {
                    if (formulaIndex != coefficientIndex)
                    {
                        (coefficients[formulaIndex], coefficients[coefficientIndex]) = (coefficients[coefficientIndex], coefficients[formulaIndex]);
                        (sum[formulaIndex], sum[coefficientIndex]) = (sum[coefficientIndex], sum[formulaIndex]);
                    }

                    break;
                }
            }

            var retainIndex = height;
            if (minValue != 1)
            {
                retainIndex = coefficientIndex + 1;
                for (var formulaIndex = coefficientIndex + 1; formulaIndex < height; formulaIndex++)
                {
                    if (coefficients[formulaIndex][coefficientIndex] % minValue == 0)
                    {
                        (coefficients[retainIndex], coefficients[formulaIndex]) = (coefficients[formulaIndex], coefficients[retainIndex]);
                        (sum[retainIndex], sum[formulaIndex]) = (sum[formulaIndex], sum[retainIndex]);
                        retainIndex++;
                    }
                }
            }

            for (var formulaIndex = coefficientIndex + 1; formulaIndex < retainIndex; formulaIndex++)
            {
                var value = coefficients[formulaIndex][coefficientIndex] / minValue;
                for (var index = coefficientIndex; index < width; index++)
                {
                    coefficients[formulaIndex][index] -= value * coefficients[coefficientIndex][index];
                }

                sum[formulaIndex] -= value * sum[coefficientIndex];
            }

            var result = _Reduce(coefficients[0..retainIndex], sum, coefficientIndex + 1);
            var total = sum[coefficientIndex];
            for (var index = width - 1; index > coefficientIndex; index--)
            {
                total -= result[index] * coefficients[coefficientIndex][index];
            }

            result[coefficientIndex] = total / coefficients[coefficientIndex][coefficientIndex];

            return result;
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
