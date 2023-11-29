namespace Moyba.AdventOfCode.Year2015
{
    public class Day24 : SolutionBase
    {
        private long[] _weights = Array.Empty<long>();

        private long _targetWeight;
        private long _minPackages;
        private long _quantumEntanglement;

        [Expect("11266889531")]
        protected override string SolvePart1()
        {
            _minPackages = Int64.MaxValue;
            _targetWeight = _weights.Sum() / 3;

            for (var index = 0; index < _weights.Length; index++)
            {
                this.OptimizePackages(index, 0, 0, 1);
            }

            return $"{_quantumEntanglement}";
        }

        [Expect("77387711")]
        protected override string SolvePart2()
        {
            _minPackages = Int64.MaxValue;
            _targetWeight = _weights.Sum() / 4;

            for (var index = 0; index < _weights.Length; index++)
            {
                this.OptimizePackages(index, 0, 0, 1);
            }

            return $"{_quantumEntanglement}";
        }

        protected override void TransformData(IEnumerable<string> data) => _weights = data.Select(Int64.Parse).Reverse().ToArray();

        private void OptimizePackages(long currentIndex, long currentCount, long currentWeight, long currentEntanglement)
        {
            currentCount++;
            if (currentCount > _minPackages) return;

            var weight = _weights[currentIndex];
            currentWeight += weight;
            if (currentWeight > _targetWeight) return;

            currentEntanglement *= weight;

            if (currentWeight == _targetWeight)
            {
                if ((currentCount < _minPackages) || (currentEntanglement < _quantumEntanglement))
                {
                    _minPackages = currentCount;
                    _quantumEntanglement = currentEntanglement;
                }

                return;
            }

            for (var index = currentIndex + 1; index < _weights.Length; index++)
            {
                this.OptimizePackages(index, currentCount, currentWeight, currentEntanglement);
            }
        }
    }
}
