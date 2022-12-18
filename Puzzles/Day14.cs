namespace advent_of_code_2022.Puzzles;

internal class Day14 : PuzzleBase
{
    public Day14() : base(nameof(Day14)) {}

    public override string SolvePart1()
    {
        AssertInputLoaded();

        var simulator = new SandSimulator(Input!, 500, false);
        var units = simulator.Run();

        if (GlobalSettings.EnableVisualizations)
        {
            simulator.Plot();
        }

        return units.ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();

        var simulator = new SandSimulator(Input!, 500, true);
        var units = simulator.Run();

        return units.ToString();
    }

    private class GridHelper
    {
        private int _resizeCount = 1;

        public Position[,] ResizeFloor(Position[,] source)
        {
            // We resize the floor with the resizeCount value,
            // every resize the resizeCount will be multiplied by 2.
            //
            var height = source.GetUpperBound(0) + 1;
            var newWidth =  source.GetUpperBound(1) - source.GetLowerBound(1) + 1 + (_resizeCount * 2);
            var newWidthLowerBound = source.GetLowerBound(1) - _resizeCount;

            // Using Array.CreateInstance to use lower bounds for width
            var newGrid = (Position[,])Array.CreateInstance(typeof(Position),
                new int[] { height, newWidth },
                new int[] { 0, newWidthLowerBound });

            // Initialize with old values
            for (int i = 0; i <= source.GetUpperBound(0); i++)
            {
                for (int j = source.GetLowerBound(1); j <= source.GetUpperBound(1); j++)
                {
                    newGrid[i, j] = source[i, j];
                }
            }

            // Initialize null values
            for (int i = 0; i <= newGrid.GetUpperBound(0); i++)
            {
                for (int j = newGrid.GetLowerBound(1); j <= newGrid.GetUpperBound(1); j++)
                {
                    if (newGrid[i, j] == null)
                    {
                        if (i == newGrid.GetUpperBound(0))
                        {
                            newGrid[i, j] = new Position(j, i, PositionState.Rock);
                        }
                        else
                        {
                            newGrid[i, j] = new Position(j, i, PositionState.Air);
                        }
                    }
                }
            }
            _resizeCount *= 2; // Multiply by two to decrease the amount of resizing

            return newGrid;
        }

        public static Position[,] CreateGrid(string[] input, int start, bool withFloor)
        {
            var inputPositions = new List<Position>();
            foreach (var line in input)
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
                        // Vertical (|)
                        if (previous.X == x)
                        {
                            // Down
                            if (previous.Y > y)
                            {
                                for (int j = previous.Y - 1; j >= y; j--)
                                {
                                    inputPositions.Add(new Position(x, j, PositionState.Rock));
                                }
                            }
                            // Up
                            else if (previous.Y < y)
                            {
                                for (int j = previous.Y + 1; j <= y; j++)
                                {
                                    inputPositions.Add(new Position(x, j, PositionState.Rock));
                                }
                            }
                        }
                        // Horizontal (-)
                        else if (previous.Y == y)
                        {
                            // Left
                            if (previous.X > x)
                            {
                                for (int j = previous.X - 1; j >= x; j--)
                                {
                                    inputPositions.Add(new Position(j, y, PositionState.Rock));
                                }
                            }
                            // Right
                            else if (previous.X < x)
                            {
                                for (int j = previous.X + 1; j <= x; j++)
                                {
                                    inputPositions.Add(new Position(j, y, PositionState.Rock));
                                }
                            }
                        }
                    }
                }
            }

            // Create grid
            var height = inputPositions.Max(x => x.Y) + (withFloor ? 3 : 1); // Floor adds two rows
            var minWidth = inputPositions.Min(x => x.X);
            var maxWidth = inputPositions.Max(x => x.X) + 1;

            // Using Array.CreateInstance to use lower bounds in the second dimension for width
            var grid = (Position[,])Array.CreateInstance(typeof(Position),
                new int[] { height, maxWidth - minWidth },
                new int[] { 0, minWidth });

            // Initialize grid (all air)
            for (int i = 0; i <= grid.GetUpperBound(0); i++)
            {
                for (int j = grid.GetLowerBound(1); j <= grid.GetUpperBound(1); j++)
                {
                    grid[i, j] = new Position(j, i, PositionState.Air);
                }
            }

            // Map input positions on grid
            inputPositions.ForEach(p => grid[p.Y, p.X] = p);
            grid[0, start].State = PositionState.Start;

            // Add floor when running for part 2
            if (withFloor)
            {
                for (int i = grid.GetLowerBound(1); i <= grid.GetUpperBound(1); i++)
                {
                    grid[height - 1, i].State = PositionState.Rock;
                }
            }

            return grid;
        }

    }

    private class SandSimulator
    {
        private readonly bool _hasFloor;
        private readonly GridHelper _gridHelper;
        private readonly Position _startPosition;
        private Position[,] _grid;
        private Position _currentPosition
;
        public SandSimulator(string[] input, int start, bool hasFloor)
        {
            _gridHelper = new GridHelper();
            _grid = GridHelper.CreateGrid(input, start, hasFloor);
            _currentPosition = _startPosition = _grid[0, start];
            _hasFloor = hasFloor;
        }

        public int Run()
        {
            int count = 0;
            while (SimulateStep() && _startPosition.State != PositionState.Sand)
            {
                _currentPosition = _startPosition;
                count++;
            }

            if (_startPosition.State == PositionState.Sand)
            {
                count++;
            }

            return count;
        }

        private bool SimulateStep()
        {
            var current = _currentPosition;
            var next = _grid[_currentPosition.Y + 1, _currentPosition.X]; // 1 position below current

            // Step 1: Simulate the unit of sand falling down in the air
            while (next.State == PositionState.Air)
            {
                current = next;
                if (current.Y == _grid.GetUpperBound(0))
                {
                    return false; // The unit of sand has fallen into the abyss
                }

                next = _grid[current.Y + 1, current.X];
            }

            // Check if we need to resize the (not so) infinite floor
            if (_hasFloor && next.Y == _grid.GetUpperBound(0) &&
                (next.X - 1 < _grid.GetLowerBound(1) || next.X + 1 > _grid.GetUpperBound(1)))
            {
                _grid = _gridHelper.ResizeFloor(_grid);
            }

            // Step 2: Check if we can move diagonal left
            if (next.X - 1 < _grid.GetLowerBound(1))
            {
                return false; // Oh noes, the unit of sand has fallen into the abyss
            }
            else
            {
                var diagonal = _grid[next.Y, next.X - 1];
                if (diagonal.State == PositionState.Air)
                {
                    // The position diagonal position seems to be air,
                    // apply the regular simulation from this starting position (recursive).
                    //
                    _currentPosition = diagonal;
                    return SimulateStep();
                }
            }

            // Step 3: Check if we can move diagonal right
            if (next.X + 1 > _grid.GetUpperBound(1))
            {
                return false; // Oh noes, the unit of sand has fallen into the abyss
            }
            else
            {
                var diagonal = _grid[next.Y, next.X + 1];
                if (diagonal.State == PositionState.Air)
                {
                    // The position diagonal position seems to be air,
                    // apply the regular simulation from this starting position (recursive).
                    //
                    _currentPosition = diagonal;
                    return SimulateStep();
                }
            }

            // The unit of sand has come to a stop
            current.State = PositionState.Sand;
            _currentPosition = current;

            return true;
        }

        public void Plot()
        {
            for (int i = 0; i <= _grid.GetUpperBound(0); i++)
            {
                for (int j = _grid.GetLowerBound(1); j <= _grid.GetUpperBound(1); j++)
                {
                    var sign = _grid[i, j].State switch
                    {
                        PositionState.Air => " ",
                        PositionState.Rock => "#",
                        PositionState.Sand => "\u001b[38;5;81mO\u001b[0m",
                        PositionState.Start => "+",
                        _ => "~"
                    };
                    Console.Write(sign);
                }
                Console.WriteLine();
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