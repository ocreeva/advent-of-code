using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2016
{
    public class Day3(string[] _data) : IPuzzle
    {
        private static readonly Regex _Parser = new Regex(@"^\s+?(\d+)\s+(\d+)\s+(\d+)$", RegexOptions.Compiled);

        private readonly (int a, int b, int c)[] _values = _data.Transform<(int, int, int)>(_Parser).ToArray();

        [PartOne("862")]
        [PartTwo("1577")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var validTriangles = _values
                .Where(_IsValidTriangle)
                .Count();

            yield return $"{validTriangles}";

            validTriangles = 0;
            for (var index = 0; index < _values.Length; index += 3)
            {
                if (_IsValidTriangle(_values[index].a, _values[index + 1].a, _values[index + 2].a)) validTriangles++;
                if (_IsValidTriangle(_values[index].b, _values[index + 1].b, _values[index + 2].b)) validTriangles++;
                if (_IsValidTriangle(_values[index].c, _values[index + 1].c, _values[index + 2].c)) validTriangles++;
            }

            yield return $"{validTriangles}";

            await Task.CompletedTask;
        }

        private static bool _IsValidTriangle((int a, int b, int c) _) => _IsValidTriangle(_.a, _.b, _.c);

        private static bool _IsValidTriangle(int a, int b, int c) =>
            a + b > c && a + c > b && b + c > a;
    }
}
