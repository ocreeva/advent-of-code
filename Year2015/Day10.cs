using AdventOfCode.Utility;

namespace Moyba.AdventOfCode.Year2015
{
    public class Day10 : SolutionBase<string>
    {
        private int _length = 0;
        private int[] _data = [];

        protected override string ReadInput(IEnumerable<string> input) => input.First();

        [Expect("329356")]
        protected override string SolvePart1()
        {
            for (var iteration = 0; iteration < 40; iteration++)
            {
                var next = new int[_length];
                for (var index = 0; index < _data.Length; index++)
                {
                    if (_data[index] == 0) continue;

                    foreach (var element in LookAndSay.GetElementTransform(index)) next[element] += _data[index];
                }

                _data = next;
            }

            var result = 0;
            for (var index = 0; index < _data.Length; index++)
            {
                result += _data[index] * LookAndSay.GetElementSequence(index).Length;
            }

            return $"{result}";
        }

        [Expect("4666278")]
        protected override string SolvePart2()
        {
            for (var iteration = 0; iteration < 10; iteration++)
            {
                var next = new int[_length];
                for (var index = 0; index < _data.Length; index++)
                {
                    if (_data[index] == 0) continue;

                    foreach (var element in LookAndSay.GetElementTransform(index)) next[element] += _data[index];
                }

                _data = next;
            }

            var result = 0;
            for (var index = 0; index < _data.Length; index++)
            {
                result += _data[index] * LookAndSay.GetElementSequence(index).Length;
            }

            return $"{result}";
        }

        protected override void TransformData(string data)
        {
            _length = LookAndSay.Elements.Length;
            _data = new int[_length];

            var name = LookAndSay.GetElementName(data);
            var index = LookAndSay.GetElementIndex(name);
            _data[index] = 1;
        }
    }
}
