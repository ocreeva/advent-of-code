using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2015
{
    public class Day14 : SolutionBase
    {
        private static readonly Regex Parser = new Regex(@"^(.+) can fly (\d+) km/s for (\d+) seconds?, but then must rest for (\d+) seconds?\.$", RegexOptions.Compiled);
        private (string name, int speed, int duration, int rest)[] _data = Array.Empty<(string, int, int, int)>();
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
            _data = data
                .Select(_ => Parser.Match(_))
                .Where(match => match.Success)
                .Select(match => {
                    var name = match.Groups[1].Value;
                    var speed = Int32.Parse(match.Groups[2].Value);
                    var duration = Int32.Parse(match.Groups[3].Value);
                    var rest = Int32.Parse(match.Groups[4].Value);
                    return (name, speed, duration, rest);
                })
                .ToArray();

            _distances = new int[_data.Length][];
            for (var index = 0; index < _distances.Length; index++)
            {
                _distances[index] = new int[2503];
                
                var distance = 0;
                var time = 0;
                var remainingDuration = _data[index].duration;
                var remainingRest = _data[index].rest;
                while (time < 2503)
                {
                    if (remainingDuration > 0)
                    {
                        distance += _data[index].speed;
                        remainingDuration--;
                    }
                    else
                    {
                        remainingRest--;
                        if (remainingRest == 0)
                        {
                            remainingDuration = _data[index].duration;
                            remainingRest = _data[index].rest;
                        }
                    }

                    _distances[index][time] = distance;
                    time++;
                }
            }
        }
    }
}
