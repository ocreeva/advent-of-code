namespace AdventOfCode.Utility
{
    public static class LCM
    {
        public static long Calculate(long a, long b)
        {
            return a * b / GCD.Calculate(a, b);
        }

        public static long CalculateWithBezout(long a, long b, out (long a, long b) bezoutCoefficients)
        {
            return a * b / GCD.CalculateWithBezout(a, b, out bezoutCoefficients);
        }
    }
}
