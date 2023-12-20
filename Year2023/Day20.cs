using Moyba.AdventOfCode.Utility;

namespace Moyba.AdventOfCode.Year2023
{
    using Signal = (string sourceName, string targetName, Day20.Pulse pulse);

    public class Day20 : IPuzzle
    {
        private readonly IDictionary<string, IModule> _modules = new Dictionary<string, IModule>();
        private readonly Queue<Signal> _signalQueue = new Queue<Signal>();

        public Day20(string[] data)
        {
            foreach (var line in data)
            {
                var parts = line.Split(" -> ");
                var targetNames = parts[1].Split(", ");
                switch (parts[0][0])
                {
                    case '%':
                    {
                        var name = parts[0][1..];
                        _modules.Add(name, new FlipFlopModule(name, targetNames, _signalQueue));
                        break;
                    }

                    case '&':
                    {
                        var name = parts[0][1..];
                        _modules.Add(name, new ConjunctionModule(name, targetNames, _signalQueue));
                        break;
                    }

                    default:
                    {
                        _modules.Add(parts[0], new BroadcastModule(parts[0], targetNames, _signalQueue));
                        break;
                    }
                }
            }

            foreach (var module in _modules.Values) module.UpdateConnections(_modules);
        }

        [PartOne("743871576")]
        [PartTwo()]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var lowPulseCount = 0L;
            var highPulseCount = 0L;
            for (var iteration = 0; iteration < 1000; iteration++)
            {
                _signalQueue.Enqueue(("button", "broadcaster", Pulse.Low));
                while (_signalQueue.TryDequeue(out var signal))
                {
                    (var sourceName, var targetName, var pulse) = signal;
                    if (pulse == Pulse.Low) lowPulseCount++; else highPulseCount++;

                    if (_modules.TryGetValue(targetName, out var target))
                    {
                        target.Receive(sourceName, pulse);
                    }
                }
            }

            yield return $"{lowPulseCount * highPulseCount}";

            // rx(kc) = lcm(ph, vn, kt, hn)
            // ph(nh) = nx, dj, (qm), (mj), (zv), (tk), mc, (kh), ck, sr, jh, pt = 0b111101000011 = 3_907
            // vn(mf) = nr, (jp), ps, (fh), dp, (bt), rz, gq, (hc), hv, bz, rb = 0b111011010101 = 3_797
            // kt(fd) = bx, (jg), tv, qx, rt, kg, lg, qj, qt, gb, qs, bl = 0b111111111101 = 4_093
            // hn(kb) = tn, (md), hh, (tc), td, bm, (mr), rs, dh, lt, cq, kx = 0b111110110101 = 4_021

            yield return $"{LCM.Calculate(LCM.Calculate(3_907, 3_797), LCM.Calculate(4_093, 4_021))}";

            // for (var iteration = 1000L; iteration < Int64.MaxValue; iteration++)
            // {
            //     _signalQueue.Enqueue(("button", "broadcaster", Pulse.Low));
            //     while (_signalQueue.TryDequeue(out var signal))
            //     {
            //         (var sourceName, var targetName, var pulse) = signal;
            //         if (_modules.TryGetValue(targetName, out var target))
            //         {
            //             target.Receive(sourceName, pulse);
            //         }
            //         else if (pulse == Pulse.Low)
            //         {
            //             yield return $"{iteration + 1}";
            //             yield break;
            //         }
            //     }
            // }

            await Task.CompletedTask;
        }

        internal enum Pulse { Low, High }

        private interface IModule
        {
            string Name { get; }
            Pulse Value { get; }
            void UpdateConnections(IDictionary<string, IModule> modules);
            void Connect(string sourceName);
            void Receive(string sourceName, Pulse pulse);
        }

        private abstract class ModuleBase(string _name, string[] _targetNames) : IModule
        {
            public string Name => _name;
            public virtual Pulse Value => Pulse.Low;
            protected string[] TargetNames => _targetNames;

            public void UpdateConnections(IDictionary<string, IModule> modules)
            {
                foreach (var targetName in this.TargetNames)
                {
                    if (modules.TryGetValue(targetName, out var target))
                    {
                        target.Connect(this.Name);
                    }
                }
            }

            public virtual void Connect(string sourceName) { }

            public abstract void Receive(string sourceName, Pulse pulse);
        }

        private class BroadcastModule(string name, string[] targetNames, Queue<Signal> _signalQueue) : ModuleBase(name, targetNames)
        {
            public override void Receive(string sourceName, Pulse pulse)
            {
                foreach (var targetName in this.TargetNames) _signalQueue.Enqueue((this.Name, targetName, pulse));
            }
        }

        private class FlipFlopModule(string name, string[] targetNames, Queue<Signal> _signalQueue) : ModuleBase(name, targetNames)
        {
            private Pulse _value = Pulse.Low;

            public override Pulse Value => _value;

            public override void Receive(string sourceName, Pulse pulse)
            {
                switch (pulse)
                {
                    case Pulse.Low:
                        _value = _Invert(_value);
                        foreach (var targetName in this.TargetNames) _signalQueue.Enqueue((this.Name, targetName, this.Value));
                        break;
                }
            }

            private static Pulse _Invert(Pulse pulse) => pulse switch
            {
                Pulse.Low => Pulse.High,
                Pulse.High => Pulse.Low,
                _ => throw new Exception($"Invalid pulse: {pulse}")
            };
        }

        private class ConjunctionModule(string name, string[] targetNames, Queue<Signal> _signalQueue) : ModuleBase(name, targetNames)
        {
            private readonly IDictionary<string, Pulse> _inboundConnections = new Dictionary<string, Pulse>();

            public override Pulse Value => _inboundConnections.Values.All(_ => _ == Pulse.High) ? Pulse.Low : Pulse.High;

            public override void Connect(string sourceName)
            {
                _inboundConnections.Add(sourceName, Pulse.Low);
            }

            public override void Receive(string sourceName, Pulse pulse)
            {
                _inboundConnections[sourceName] = pulse;
                foreach (var targetName in this.TargetNames) _signalQueue.Enqueue((this.Name, targetName, this.Value));
            }
        }
    }
}
