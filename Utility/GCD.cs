namespace AdventOfCode.Utility
{
    public static class GCD
    {
        public static long Calculate(long a, long b)
        {
            if (b == 0) return a;
            return GCD.Calculate(b, a % b);
        }
    }
}
