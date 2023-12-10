namespace Moyba.AdventOfCode.Year2023
{
    using Coord = (int x, int y);

    public class Day10(string[] _data) : IPuzzle
    {
        private readonly Coord _start = Enumerable.Range(0, _data.Length)
            .SelectMany(y => Enumerable.Range(0, _data[y].Length).Where(x => _data[y][x] == 'S').Select(x => (x, y)))
            .Single();

        [PartOne("6846")]
        [PartTwo("325")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var enumerables = new List<IEnumerable<Coord>>();

            if (_start.x > 0)
            {
                switch (_data[_start.y][_start.x - 1])
                {
                    case 'F':
                    case 'L':
                    case '-':
                        enumerables.Add(_FollowPipes((-1, 0)));
                        break;
                }
            }

            if (_start.x <= _data[0].Length - 1)
            {
                switch (_data[_start.y][_start.x + 1])
                {
                    case 'J':
                    case '7':
                    case '-':
                        enumerables.Add(_FollowPipes((1, 0)));
                        break;
                }
            }

            if (_start.y > 0)
            {
                switch (_data[_start.y - 1][_start.x])
                {
                    case '7':
                    case 'F':
                    case '|':
                        enumerables.Add(_FollowPipes((0, -1)));
                        break;
                }
            }

            if (_start.y <= _data.Length - 1)
            {
                switch (_data[_start.y + 1][_start.x])
                {
                    case 'J':
                    case 'L':
                    case '|':
                        enumerables.Add(_FollowPipes((0, 1)));
                        break;
                }
            }

            var loop = new HashSet<Coord> { _start };
            using (IEnumerator<Coord> enumerator1 = enumerables[0].GetEnumerator(), enumerator2 = enumerables[1].GetEnumerator())
            {
                for (var step = 1; enumerator1.MoveNext() && enumerator2.MoveNext(); step++)
                {
                    loop.Add(enumerator1.Current);
                    loop.Add(enumerator2.Current);
                    if (enumerator1.Current == enumerator2.Current)
                    {
                        yield return $"{step}";
                        break;
                    }
                }
            }

            var interior = 0;
            var height = _data.Length;
            var width = _data[0].Length;
            for (var yStart = 2; yStart < height + width; yStart++)
            {
                var inLoop = false;
                
                for (int x = 0, y = yStart; y > 0 && x < width; x++, y--)
                {
                    if (loop.Contains((x, y)))
                    {
                        switch (_data[y][x])
                        {
                            case 'F':
                            case 'J':
                                break;

                            default:
                                inLoop = !inLoop;
                                break;
                        }
                    }
                    else if (inLoop) interior++;
                }
            }

            yield return $"{interior}";

            await Task.CompletedTask;
        }

        private IEnumerable<Coord> _FollowPipes(Coord heading)
        {
            Coord position = (_start.x + heading.x, _start.y + heading.y);
            while (_data[position.y][position.x] != 'S')
            {
                yield return position;

                switch (_data[position.y][position.x])
                {
                    case '-':
                    case '|':
                        // no heading change
                        break;

                    case 'J':
                    case 'F':
                        heading = (-heading.y, -heading.x);
                        break;

                    case '7':
                    case 'L':
                        heading = (heading.y, heading.x);
                        break;

                    default:
                        throw new Exception($"Pipe not found!");
                }

                position = (position.x + heading.x, position.y + heading.y);
            }
        }
    }
}
