using System.Text.RegularExpressions;
using Reindeer = (string name, int speed, int duration, int rest);

namespace Moyba.AdventOfCode.Year2015
{
    public class Day14 : SolutionBase
    {
        private static readonly Regex Parser = new Regex(@"^(.+) can fly (\d+) km/s for (\d+) seconds?, but then must rest for (\d+) seconds?\.$", RegexOptions.Compiled);

        private Reindeer[] _reindeer = Array.Empty<Reindeer>();
        private int[][] _distances = Array.Empty<int[]>();

        [Expect("2640")]
        protected override string SolvePart1()
        {
            var maxDistance = _distances.Max(_ => _[2502]);

            return $"{maxDistance}";
        }

        [Expect("1102")]
        protected override string SolvePart2()
        {
            int[] maxDistances = Enumerable.Range(0, 2503).Select(time => _distances.Max(_ => _[time])).ToArray();
            var maxScore = _distances.Max(_ => {
                var timesInLead = 0;
                for (var time = 0; time < 2503; time++)
                {
                    if (_[time] < maxDistances[time]) continue;
                    timesInLead++;
                }

                return timesInLead;
            });
            return $"{maxScore}";
        }

        protected override void TransformData(IEnumerable<string> data)
        {
            _reindeer = Parser.TransformData<Reindeer>(data).ToArray();

            _distances = new int[_reindeer.Length][];
            for (var index = 0; index < _distances.Length; index++)
            {
                _distances[index] = new int[2503];
                
                var distance = 0;
                var time = 0;
                var remainingDuration = _reindeer[index].duration;
                var remainingRest = _reindeer[index].rest;
                while (time < 2503)
                {
                    if (remainingDuration > 0)
                    {
                        distance += _reindeer[index].speed;
                        remainingDuration--;
                    }
                    else
                    {
                        remainingRest--;
                        if (remainingRest == 0)
                        {
                            remainingDuration = _reindeer[index].duration;
                            remainingRest = _reindeer[index].rest;
                        }
                    }

                    _distances[index][time] = distance;
                    time++;
                }
            }
        }
    }
}
