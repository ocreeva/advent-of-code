namespace Moyba.AdventOfCode.Year2023
{
    using Lens = (string label, int focalLength);

    public class Day15(string[] _data) : IPuzzle
    {
        private readonly string[] _sequence = _data.Single().Split(',');

        [PartOne("504449")]
        [PartTwo("262044")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var hashes = _sequence.Select(this.ComputeHashes).ToArray();

            yield return $"{hashes.Select(_ => _.step).Sum()}";

            var boxes = new List<Lens>[256];
            for (var index = 0; index < 256; index++) boxes[index] = new List<Lens>();

            for (var index = 0; index < _sequence.Length; index++)
            {
                var boxIndex = hashes[index].label;
                var box = boxes[boxIndex];

                var step = _sequence[index];
                var operationIndex = 0; while (Char.IsLetter(step[operationIndex])) operationIndex++;
                var label = step[..operationIndex];

                switch (step[operationIndex])
                {
                    case '=':
                        var focalLength = Int32.Parse(step[(operationIndex + 1)..]);
                        var foundLabel = false;
                        for (var lensIndex = 0; lensIndex < box.Count && !foundLabel; lensIndex++)
                        {
                            var lens = box[lensIndex];
                            if (!label.Equals(lens.label)) continue;

                            foundLabel = true;
                            lens.focalLength = focalLength;
                            box[lensIndex] = lens;
                        }

                        if (!foundLabel) boxes[boxIndex].Add((label, focalLength));
                        break;

                    case '-':
                        for (var lensIndex = 0; lensIndex < box.Count; lensIndex++)
                        {
                            var lens = box[lensIndex];
                            if (!label.Equals(lens.label)) continue;

                            box.RemoveAt(lensIndex);
                            break;
                        }
                        break;
                }
            }

            var power = 0;
            for (var boxIndex = 0; boxIndex < 256; boxIndex++)
            {
                var box = boxes[boxIndex];
                for (var lensIndex = 0; lensIndex < box.Count; lensIndex++)
                {
                    power += (boxIndex + 1) * (lensIndex + 1) * box[lensIndex].focalLength;
                }
            }

            yield return $"{power}";

            await Task.CompletedTask;
        }

        private (int step, int label) ComputeHashes(string step)
        {
            var hash = 0;

            var index = 0;
            while (Char.IsLetter(step[index])) hash = 17 * (hash + step[index++]) % 256;

            var labelHash = hash;
            while (index < step.Length) hash = 17 * (hash + step[index++]) % 256;

            return (hash, labelHash);
        }
    }
}
