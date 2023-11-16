namespace Moyba.AdventOfCode.Year2015
{
    public class Day8 : SolutionBase
    {
        private string[] _data = Array.Empty<string>();

        [Expect("1333")]
        protected override string SolvePart1()
        {
            int result = 0;
            foreach (var value in _data)
            {
                // leading and trailing quotes, already stripped
                result += 2;

                for (var index = 0; index < value.Length; index++)
                {
                    if (value[index] != '\\') continue;

                    result++;

                    if (value[++index] != 'x') continue;

                    result += 2;
                    index += 2;
                }
            }

            return $"{result}";
        }

        [Expect("2046")]
        protected override string SolvePart2()
        {
            int result = 0;
            foreach (var value in _data)
            {
                // leading and trailing quotes, and escaping the original leading and trailing quotes
                result += 4;

                // escaping \ and "
                result += value.Count(c => c == '\\' || c == '"');
            }

            return $"{result}";
        }

        protected override void TransformData(IEnumerable<string> data) => _data = data.Select(x => x.Substring(1, x.Length - 2)).ToArray();
    }
}
