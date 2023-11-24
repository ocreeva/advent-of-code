namespace Moyba.AdventOfCode.Year2015
{
    public class Day18 : SolutionBase
    {
        private int[,] _grid = new int[102,102];
        private int[,] _gridForPart2 = new int[102,102];

        [Expect("821")]
        protected override string SolvePart1()
        {
            for (var iteration = 0; iteration < 100; iteration++)
            {
                var nextGrid = new int[102,102];
                for (var y = 1; y < 101; y++)
                {
                    for (var x = 1; x < 101; x++)
                    {
                        var adjacent = CountAdjacent(_grid, x, y);
                        switch (adjacent)
                        {
                            case 3:
                                nextGrid[x, y] = 1;
                                break;

                            case 2:
                                nextGrid[x, y] = _grid[x, y];
                                break;
                        }
                    }
                }

                _grid = nextGrid;
            }

            var count = 0;
            for (var y = 1; y < 101; y++)
            {
                for (var x = 1; x < 101; x++)
                {
                    count += _grid[x, y];
                }
            }

            return $"{count}";
        }

        [Expect("886")]
        protected override string SolvePart2()
        {
            _grid = _gridForPart2;
            _grid[1, 1] = 1;
            _grid[1, 100] = 1;
            _grid[100, 1] = 1;
            _grid[100, 100] = 1;

            for (var iteration = 0; iteration < 100; iteration++)
            {
                var nextGrid = new int[102,102];
                for (var y = 1; y < 101; y++)
                {
                    for (var x = 1; x < 101; x++)
                    {
                        if ((y == 1 || y == 100) && (x == 1 || x == 100))
                        {
                            nextGrid[x, y] = 1;
                            continue;
                        }

                        var adjacent = CountAdjacent(_grid, x, y);
                        switch (adjacent)
                        {
                            case 3:
                                nextGrid[x, y] = 1;
                                break;

                            case 2:
                                nextGrid[x, y] = _grid[x, y];
                                break;
                        }
                    }
                }

                _grid = nextGrid;
            }

            var count = 0;
            for (var y = 1; y < 101; y++)
            {
                for (var x = 1; x < 101; x++)
                {
                    count += _grid[x, y];
                }
            }

            return $"{count}";
        }

        private static int CountAdjacent(int[,] grid, int x, int y)
        {
            return grid[x - 1, y - 1]
                 + grid[x - 1, y    ]
                 + grid[x - 1, y + 1]
                 + grid[x    , y - 1]
                 + grid[x    , y + 1]
                 + grid[x + 1, y - 1]
                 + grid[x + 1, y    ]
                 + grid[x + 1, y + 1];
        }

        protected override void TransformData(IEnumerable<string> data)
        {
            var grid = data.ToArray();
            for (var y = 0; y < grid.Length; y++)
            {
                var line = grid[y];
                for (var x = 0; x < line.Length; x++)
                {
                    switch (line[x])
                    {
                        case '#':
                            _grid[x + 1, y + 1] = 1;
                            _gridForPart2[x + 1, y + 1] = 1;
                            break;
                    }
                }
            }
        }
    }
}
