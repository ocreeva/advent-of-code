using System.Numerics;

namespace Moyba.AdventOfCode.Utility
{
    public static class GCD
    {
        public static long Calculate(long a, long b)
        {
            return _Calculate(Math.Abs(a), Math.Abs(b));
        }

        public static long Calculate(long[] values)
        {
            if (values.Length == 0) return 1L;

            var gcd = values[0];
            for (var index = 1; index < values.Length; index++) gcd = GCD.Calculate(values[index], gcd);
            return gcd;
        }

        public static BigInteger Calculate(BigInteger a, BigInteger b)
        {
            return _Calculate(BigInteger.Abs(a), BigInteger.Abs(b));
        }

        public static BigInteger Calculate(BigInteger[] values)
        {
            if (values.Length == 0) return 1L;

            var gcd = values[0];
            for (var index = 1; index < values.Length; index++) gcd = GCD.Calculate(values[index], gcd);
            return gcd;
        }

        // from https://en.wikipedia.org/wiki/Extended_Euclidean_algorithm
        public static long CalculateWithBezout(long a, long b, out (long a, long b) bezoutCoefficients)
        {
            var s = 0L;
            var oldS = 1L;
            var r = b;
            var oldR = a;

            while (r != 0)
            {
                var quotient = oldR / r;
                (oldR, r) = (r, oldR - quotient * r);
                (oldS, s) = (s, oldS - quotient * s);
            }

            var bezoutT = b == 0 ? 0 : (oldR - oldS * a) / b;
            bezoutCoefficients = (oldS, bezoutT);
            return oldR;
        }

        private static long _Calculate(long a, long b)
        {
            if (b == 0) return a;
            return GCD.Calculate(b, a % b);
        }

        private static BigInteger _Calculate(BigInteger a, BigInteger b)
        {
            if (b == 0) return a;
            return GCD.Calculate(b, a % b);
        }
    }
}
