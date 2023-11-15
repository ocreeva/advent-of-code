namespace Moyba.AdventOfCode.Year2015
{
    public class Day7 : SolutionBase
    {
        private IDictionary<string, Wire> _wires = new Dictionary<string, Wire>();
        private string _part1 = String.Empty;

        [Expect("16076")]
        protected override string SolvePart1()
        {
            var value = _wires["a"].GetValue(_wires);
            _part1 = $"{value}";
            return _part1;
        }

        [Expect("2797")]
        protected override string SolvePart2()
        {
            _wires["b"] = new Wire("b", _part1);
            foreach (var wire in _wires.Values) wire.Reset();

            var value = _wires["a"].GetValue(_wires);
            return $"{value}";
        }

        protected override void TransformData(IEnumerable<string> data) => _wires = data
            .Select(d => d.Split(" -> "))
            .ToDictionary(x => x[1], x => new Wire(x[1], x[0]));

        private enum Operation
        {
            Unspecified,
            AND,
            OR,
            LSHIFT,
            RSHIFT,
            NOT,
        }
        private static readonly IDictionary<string, Operation> OperationLookup = new Dictionary<string, Operation>
        {
            { "AND", Operation.AND },
            { "OR", Operation.OR },
            { "LSHIFT", Operation.LSHIFT },
            { "RSHIFT", Operation.RSHIFT },
            { "NOT", Operation.NOT },
        };

        private class Wire(string key, string logic)
        {
            private readonly string _key = key;
            private readonly string _logic = logic;
            private ushort? _value;

            public ushort GetValue(IDictionary<string, Wire> wires)
            {
                if (_value.HasValue) return _value.Value;

                var operation = Operation.Unspecified;
                ushort value = 0;
                var parts = _logic.Split(' ');
                foreach (var token in parts)
                {
                    if (OperationLookup.ContainsKey(token))
                    {
                        operation = OperationLookup[token];
                        continue;
                    }

                    ushort partial = wires.ContainsKey(token) ? wires[token].GetValue(wires) : UInt16.Parse(token);
                    switch (operation)
                    {
                        case Operation.Unspecified:
                            value = partial;
                            break;

                        case Operation.AND:
                            value &= partial;
                            break;

                        case Operation.OR:
                            value |= partial;
                            break;

                        case Operation.LSHIFT:
                            value <<= partial;
                            break;

                        case Operation.RSHIFT:
                            value >>= partial;
                            break;

                        case Operation.NOT:
                            value = (ushort)~partial;
                            break;
                    }
                }

                _value = value;
                return value;
            }

            public void Reset()
            {
                _value = null;
            }
        }
    }
}
