namespace advent_of_code_2022.Puzzles;

internal class Day14 : PuzzleBase
{
    public Day14() : base(nameof(Day14)) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();
        GlobalSettings.EnableVisualizations = true;

        var grid = GetGrid(500);
        var simulator = new SandSimulator(grid, 500);
        var units = simulator.GetUnits();

        return units.ToString(); ;
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();

        return "";
    }

    private Position[,] GetGrid(int start)
    {
        var inputPositions = new List<Position>();
        foreach (var line in Input!)
        {
            var coords = line.Split(" -> ").Select(x => x.Split(',')).ToArray();
            for (int i = 0; i < coords.Length; i++)
            {
                var x = int.Parse(coords[i][0]);
                var y = int.Parse(coords[i][1]);

                if (i == 0)
                {
                    inputPositions.Add(new Position(x, y, PositionState.Rock));
                }
                else
                {
                    var previous = inputPositions.Last();
                    // Move vertical
                    if (previous.X == x)
                    {
                        // Move down
                        if (previous.Y > y)
                        {
                            for (int j = previous.Y - 1; j >= y; j--)
                            {
                                inputPositions.Add(new Position(x, j, PositionState.Rock));
                            }
                        }
                        // Move up
                        else
                        {
                            for (int j = previous.Y + 1; j <= y; j++)
                            {
                                inputPositions.Add(new Position(x, j, PositionState.Rock));
                            }
                        }
                    }
                    // Move horizontal
                    else
                    {
                        // Move left
                        if (previous.X > x)
                        {
                            for (int j = previous.X - 1; j >= x; j--)
                            {
                                inputPositions.Add(new Position(j, y, PositionState.Rock));
                            }
                        }
                        // Move right
                        else
                        {
                            for (int j = previous.X + 1; j <= x; j++)
                            {
                                inputPositions.Add(new Position(j, x, PositionState.Rock));
                            }
                        }
                    }
                }
            }
        }

        // Create grid
        var height = inputPositions.Max(x => x.Y) + 1;
        var minWidth = inputPositions.Min(x => x.X);
        var maxWidth = inputPositions.Max(x => x.X) + 1;

        // Using Array.CreateInstance to use lower bounds for width
        var grid = (Position[,])Array.CreateInstance(typeof(Position),
            new int[] { height, maxWidth - minWidth },
            new int[] { 0, minWidth });

        // Initialize grid
        for (int i = 0; i <= grid.GetUpperBound(0); i++)
        {
            for (int j = grid.GetLowerBound(1); j <= grid.GetUpperBound(1); j++)
            {
                grid[i, j] = new Position(j, i, PositionState.Air);
            }
        }

        // Map positions on grid
        inputPositions.ForEach(p => grid[p.Y, p.X] = p);
        grid[0, start].State = PositionState.Start;

        return grid;
    }

    private class SandSimulator
    {
        private readonly Position[,] _grid;
        private readonly Position _startPosition;
        private Position _currentPosition
;
        public SandSimulator(Position[,] grid, int start)
        {
            _grid = grid;
            _currentPosition = _startPosition = _grid[0, start];
        }

        public int GetUnits()
        {
            int count = 0;
            Console.Clear();
            Console.CursorVisible = false;

            while (SimulateStep())
            {
                _currentPosition = _startPosition;
                count++;
                //Plot(false);
                //Thread.Sleep(25);
            }
            Plot(true);

            return count;
        }

        private bool SimulateStep()
        {
            var current = _currentPosition;
            var next = _grid[_currentPosition.Y + 1, _currentPosition.X];

            while (next.State == PositionState.Air)
            {
                current = next;

                if (current.Y == _grid.GetUpperBound(0))
                {
                    return false;
                }

                next = _grid[current.Y + 1, current.X];
            }

            // Diagonal left
            if (next.X - 1 < _grid.GetLowerBound(1))
            {
                return false;
            }
            else
            {
                var left = _grid[current.Y, current.X - 1];
                var diagLeft = _grid[next.Y, next.X - 1];
                if (diagLeft.State == PositionState.Air && left.State == PositionState.Air)
                {
                    _currentPosition = diagLeft;
                    return SimulateStep();
                }
            }

            // Diagonal right
            if (next.X + 1 > _grid.GetUpperBound(1))
            {
                return false;
            }
            else
            {
                var right = _grid[current.Y, current.X + 1];
                var diagRight = _grid[next.Y, next.X + 1];
                if (diagRight.State == PositionState.Air && right.State == PositionState.Air)
                {
                    _currentPosition = diagRight;
                    return SimulateStep();
                }
            }

            current.State = PositionState.Sand;
            _currentPosition = current;

            return true;
        }

        public void Plot(bool lastPlot)
        {
            if (!GlobalSettings.EnableVisualizations)
            {
                return;
            }

            var initialCursorLeft = Console.CursorLeft;
            var initialCursorTop = Console.CursorTop;

            for (int i = 0; i <= _grid.GetUpperBound(0); i++)
            {
                var lowerBound = _grid.GetLowerBound(1);
                for (int j = lowerBound; j <= _grid.GetUpperBound(1); j++)
                {
                    var sign = _grid[i, j].State switch
                    {
                        PositionState.Air => '.',
                        PositionState.Rock => '#',
                        PositionState.Sand => 'O',
                        PositionState.Start => '+',
                        _ => '~'
                    };
                    Console.Write(sign);
                }
                Console.WriteLine();
            }

            if (lastPlot)
            {
                Console.SetCursorPosition(initialCursorLeft, Console.CursorTop + 1);
                Console.CursorVisible = true;
            }
            else
            {
                Console.SetCursorPosition(initialCursorLeft, initialCursorTop);
            }
        }
    }

    private class Position
    {
        public int X { get; }
        public int Y { get; }
        public PositionState State { get; set; }

        public Position(int x, int y, PositionState initialState)
        {
            X = x;
            Y = y;
            State = initialState;
        }
    }

    public enum PositionState
    {
        Air,
        Rock,
        Sand,
        Start
    }
}