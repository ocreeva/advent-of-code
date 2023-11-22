namespace Moyba.AdventOfCode.Year2015
{
    public class Day17 : SolutionBase
    {
        private int[] _data = Array.Empty<int>();

        private int _part1 = 0;
        private int _part2Containers = Int32.MaxValue;
        private int _part2Count = 0;

        [Expect("4372")]
        protected override string SolvePart1()
        {
            for (var index = 0; index < _data.Length; index++) FindCombinations(index, 0, 0);
            return $"{_part1}";
        }

        [Expect("4")]
        protected override string SolvePart2()
        {
            return $"{_part2Count}";
        }

        private void FindCombinations(int index, int capacity, int numContainers)
        {
            capacity += _data[index];
            numContainers++;

            if (capacity == 150)
            {
                _part1++;

                if (numContainers < _part2Containers)
                {
                    _part2Containers = numContainers;
                    _part2Count = 0;
                }

                if (numContainers == _part2Containers) _part2Count++;
            }

            if (capacity >= 150) return;

            for (var nextIndex = index + 1; nextIndex < _data.Length; nextIndex++) FindCombinations(nextIndex, capacity, numContainers);
        }

        protected override void TransformData(IEnumerable<string> data) => _data = data.Select(Int32.Parse).ToArray();
    }
}
