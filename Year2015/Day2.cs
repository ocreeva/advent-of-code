namespace Moyba.AdventOfCode.Year2015
{
    public class Day2 : SolutionBase
    {
        private (int x, int y, int z)[] _data = Array.Empty<(int, int, int)>();

        [Expect("1598415")]
        protected override string SolvePart1()
        {
            var paper = _data.Sum(d => 3 * d.x * d.y + 2 * (d.x * d.z + d.y * d.z));
            return $"{paper}";
        }

        [Expect("3812909")]
        protected override string SolvePart2()
        {
            var ribbon = _data.Sum(d => 2 * (d.x + d.y) + d.x * d.y * d.z);
            return $"{ribbon}";
        }

        protected override void TransformData(IEnumerable<string> data) => _data = data
            .Select(s => s.Split('x').Select(Int32.Parse).Order().ToArray())
            .Select(x => (x[0], x[1], x[2]))
            .ToArray();
    }
}
