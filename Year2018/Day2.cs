namespace Moyba.AdventOfCode.Year2018
{
    public class Day2(IEnumerable<string> data) : IPuzzle
    {
        private readonly string[] _data = data.ToArray();

        private int _checksum;
        private string? _prototypeID;

        public Task ComputeAsync()
        {
            var doubleLetters = 0;
            var tripleLetters = 0;
            foreach (var id in _data)
            {
                var letterCounts = id.GroupBy(_ => _).Select(_ => _.Count()).ToHashSet();
                if (letterCounts.Contains(2)) doubleLetters++;
                if (letterCounts.Contains(3)) tripleLetters++;
            }

            _checksum = doubleLetters * tripleLetters;

            var root = new TrieNode();
            foreach (var id in _data)
            {
                if (root.ExistsOrAdd(id.ToCharArray(), out _prototypeID)) return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        [Solution("7936")]
        public string SolvePartOne() => $"{_checksum}";

        [Solution("lnfqdscwjyteorambzuchrgpx")]
        public string SolvePartTwo() => _prototypeID ?? String.Empty;

        private class TrieNode(int index = -1, int _ignoreIndex = -1, char[]? _initial = null)
        {
            private readonly TrieNode[] _next = new TrieNode[27];
            private readonly int _nextIndex = index + 1;

            private bool _isInitialized;

            public bool ExistsOrAdd(char[] characters, out string? value)
            {
                if (_nextIndex == characters.Length)
                {
                    char[] result = [ ..characters[.._ignoreIndex], ..characters[(_ignoreIndex+1)..] ];
                    value = String.Join("", result);
                    return true;
                }

                this.EnsureInitialized();

                value = null;

                var nextCharacter = characters[_nextIndex];
                var characterIndex = _GetCharacterIndex(nextCharacter);

                if (_ignoreIndex == -1)
                {
                    if (_next[0].ExistsOrAdd(characters, out value)) return true;
                }

                if (_next[characterIndex] == null) _next[characterIndex] = new TrieNode(_nextIndex, _ignoreIndex, characters);
                else return _next[characterIndex].ExistsOrAdd(characters, out value);

                return false;
            }

            private void EnsureInitialized()
            {
                if (_isInitialized) return;
                _isInitialized = true;

                if (_ignoreIndex == -1)
                {
                    _next[0] = new TrieNode(_nextIndex, _nextIndex, _initial);
                }

                if (_initial == null) return;

                var nextCharacter = _initial[_nextIndex];
                var characterIndex = _GetCharacterIndex(nextCharacter);
                _next[characterIndex] = new TrieNode(_nextIndex, _ignoreIndex, _initial);
            }

            private static int _GetCharacterIndex(char c) => c - '`';
        }
    }
}
