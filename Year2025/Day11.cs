namespace Moyba.AdventOfCode.Year2025
{
    using ComplexPaths = (long None, long Fft, long Dac, long Both);

    public class Day11(string[] _data) : IPuzzle
    {
        private const string _Dac = "dac";
        private const string _Fft = "fft";
        private const string _Out = "out";
        private const string _Svr = "svr";
        private const string _You = "you";

        private readonly IDictionary<string, string[]> _outputs = _data
            .Select(_ => _.Split(' '))
            .ToDictionary(_ => _[0][..^1], _ => _[1..]);

        [PartOne("786")]
        [PartTwo("495845045016588")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var pathsToOut = new Dictionary<string, long> { { _Out, 1L } };
            var devices = new Queue<string>(_outputs.Keys);
            while (!pathsToOut.ContainsKey(_You) && devices.TryDequeue(out var device))
            {
                if (!_outputs[device].All(pathsToOut.ContainsKey))
                {
                    devices.Enqueue(device);
                    continue;
                }

                pathsToOut.Add(device, _outputs[device].Sum(_ => pathsToOut[_]));
            }

            yield return $"{pathsToOut[_You]}";

            var complexPathsToOut = new Dictionary<string, ComplexPaths> { { _Out, (1, 0, 0, 0) } };
            devices = new Queue<string>(_outputs.Keys);
            while (!complexPathsToOut.ContainsKey(_Svr) && devices.TryDequeue(out var device))
            {
                if (!_outputs[device].All(complexPathsToOut.ContainsKey))
                {
                    devices.Enqueue(device);
                    continue;
                }

                complexPathsToOut.Add(
                    device,
                    _outputs[device]
                        .Select(_ => complexPathsToOut[_])
                        .Aggregate((a, b) => (
                            a.None + b.None,
                            a.Fft + b.Fft,
                            a.Dac + b.Dac,
                            a.Both + b.Both
                        ))
                );

                switch (device)
                {
                    case _Dac:
                        var paths = complexPathsToOut[device];
                        complexPathsToOut[device] = (0L, 0L, paths.None + paths.Dac, paths.Fft + paths.Both);
                        break;

                    case _Fft:
                        paths = complexPathsToOut[device];
                        complexPathsToOut[device] = (0L, paths.None + paths.Fft, 0L, paths.Dac + paths.Both);
                        break;
                }
            }

            yield return $"{complexPathsToOut[_Svr].Both}";

            await Task.CompletedTask;
        }
    }
}
