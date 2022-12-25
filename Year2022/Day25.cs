using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2022
{
    public class Day25 : SolutionBase
    {
        private long[] _data = Array.Empty<long>();

        [Expect("2-0-01==0-1=2212=100")]
        protected override string SolvePart1()
        {
            var sum = _data.Sum();

            return _ToSnafu(sum);
        }

        [Expect()]
        protected override string SolvePart2()
        {
            return $"Merry Christmas!";
        }

        protected override void TransformData(IEnumerable<string> data) => _data = data.Select(_FromSnafu).ToArray();

        private static long _FromSnafu(string snafu)
        {
            var value = 0L;
            for (var index = 0; index < snafu.Length; index++)
            {
                value *= 5;
                var current = snafu[index];
                switch (current)
                {
                    case '-':
                        value -= 1;
                        break;

                    case '=':
                        value -= 2;
                        break;

                    case '0':
                    case '1':
                    case '2':
                        value += current - '0';
                        break;

                    default:
                        throw new Exception("Unexpected input!");
                }
            }

            return value;
        }

        private static string _ToSnafu(long value)
        {
            if (value == 0) return String.Empty;

            var mod = value % 5;
            if (mod > 2) mod -= 5;
            var next = (value - mod) / 5;
            switch (mod)
            {
                case -1:
                    return _ToSnafu(next) + "-";

                case -2:
                    return _ToSnafu(next) + "=";

                case 0:
                case 1:
                case 2:
                    return _ToSnafu(next) + $"{mod}";

                default:
                    throw new Exception("Unexpected value!");
            }
        }
    }
}
