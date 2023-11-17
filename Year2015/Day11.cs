namespace Moyba.AdventOfCode.Year2015
{
    public class Day11 : SolutionBase<string>
    {
        private char[] _data = Array.Empty<char>();

        protected override string ReadInput(IEnumerable<string> input) => input.First();

        [Expect("hxbxxyzz")]
        protected override string SolvePart1()
        {
            IncrementUntilValid();

            return new string(_data.Reverse().ToArray());
        }

        [Expect("hxcaabcc")]
        protected override string SolvePart2()
        {
            IncrementUntilValid();

            return new string(_data.Reverse().ToArray());
        }

        private void IncrementUntilValid()
        {
            while (!HasFirstDoubleLetter()) IncrementLetter(2);

            do
            {
                var index = IncrementLetter();
                if (index >= 2) while (!HasFirstDoubleLetter()) IncrementLetter(2);
            } while (!IsValidPassword());
        }

        private bool HasFirstDoubleLetter()
        {
            for (var index = 2; index < _data.Length - 1; index++)
            {
                if (_data[index] != _data[index + 1]) continue;
                return true;
            }

            return false;
        }

        private bool IsValidPassword()
        {
            // three sequential letters
            if (!Enumerable.Range(0, _data.Length - 2).Any(index => _data[index] == _data[index + 1] + 1 && _data[index] == _data[index + 2] + 2)) return false;

            // two different letter pairs
            bool foundOne = false;
            for (var index = 0; index < _data.Length - 1; index++)
            {
                if (_data[index] != _data[index + 1]) continue;

                if (foundOne) return true;

                foundOne = true;
                index++;
            }

            return false;
        }

        private int IncrementLetter(int index = 0)
        {
            switch (_data[index])
            {
                case 'z':
                    _data[index] = 'a';
                    return IncrementLetter(index + 1);

                default:
                    _data[index]++;
                    break;
            }

            switch (_data[index])
            {
                // skip confusing letters
                case 'i':
                case 'l':
                case 'o':
                    _data[index]++;
                    break;
            }

            return index;
        }

        protected override void TransformData(string data) => _data = data.Reverse().ToArray();
    }
}
