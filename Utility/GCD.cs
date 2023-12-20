namespace Moyba.AdventOfCode.Utility
{
    public static class GCD
    {
        public static long Calculate(long a, long b)
        {
            if (b == 0) return a;
            return GCD.Calculate(b, a % b);
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
    }
}
