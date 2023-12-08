namespace AdventOfCode.Utility
{
    public static class ChineseRemainderTheorem
    {
        public static (long mod, long lcm) Calculate(long aMod, long a, long bMod, long b)
        {
            var lcm = LCM.CalculateWithBezout(a, b, out var coeff);

            var mod = aMod * b * coeff.b + bMod * a * coeff.a;
            if (mod < 0) mod += lcm;

            return (mod, lcm);
        }
    }
}
