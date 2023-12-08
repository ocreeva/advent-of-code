namespace AdventOfCode.Utility
{
    public static class LCM
    {
        public static long Calculate(long a, long b)
        {
            return a * b / GCD.Calculate(a, b);
        }
    }
}
