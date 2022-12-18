using System.Text.RegularExpressions;
namespace advent_of_code_2022.Puzzles;

internal class Day15 : PuzzleBase
{
    public Day15() : base(nameof(Day15), longRunning: true) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();

        var sensors = new SensorNetwork(Input!);
        var result = sensors.GetNumberOfPositionsWithoutDistressBeacon(2000000);

        return result.ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();

        var sensors = new SensorNetwork(Input!);
        var result = sensors.GetDistressBeaconTuningFrequency(4000000);

        return result.ToString();
    }

    private class SensorNetwork
    {
        private readonly List<Sensor> _sensors = new();

        public SensorNetwork(string[] input)
        {
            foreach (var line in input)
            {
                _sensors.Add(new Sensor(line));
            }
        }

        public long GetNumberOfPositionsWithoutDistressBeacon(long y)
        {
            long positions = 0;
            long min = GetMinX();
            long max = GetMaxX();
            for (long x = min; x <= max; x++)
            {
                if (_sensors.Where(s => s.IsCoveredBySensor(x, y, false)).Any())
                {
                    positions++;
                }
            }
            return positions;
        }

        public long GetDistressBeaconTuningFrequency(long most)
        {
            var justOutsidePerimeterSet = _sensors[0].GetPointsNotCoveredJustOutsidePerimeter(most).ToHashSet();;
            foreach(var sensor in _sensors.Skip(1))
            {
                justOutsidePerimeterSet.UnionWith(sensor.GetPointsNotCoveredJustOutsidePerimeter(most));
            }
            var distressBeacon = justOutsidePerimeterSet.Where(x => _sensors.All(s => !s.IsCoveredBySensor(x.Item1, x.Item2, true))).Single();

            return (distressBeacon.Item1 * 4000000) + distressBeacon.Item2;
        }

        private long GetMinX()
            => _sensors.Min(x => x.GetMinX());

        private long GetMaxX()
            => _sensors.Max(x => x.GetMaxX());
    }

    private class Sensor
    {
        public Tuple<long, long> Coordinates { get; }
        public Tuple<long, long> Beacon { get; }
        public long ManhattanDistance { get; }

        private static readonly Regex _readingRegex =
            new Regex(@".+x=(-\d{1,}|\d{1,}).+y=(-\d{1,}|\d{1,}).+x=(-\d{1,}|\d{1,}).+y=(-\d{1,}|\d{1,})",
                RegexOptions.Compiled);

        public Sensor(string reading)
        {
            var match = _readingRegex.Match(reading);
            Coordinates = new(long.Parse(match.Groups[1].Value), long.Parse(match.Groups[2].Value));
            Beacon = new(long.Parse(match.Groups[3].Value), long.Parse(match.Groups[4].Value));
            ManhattanDistance = CalculateManhattanDistance(Beacon.Item1, Beacon.Item2);
        }

        public bool IsCoveredBySensor(long x, long y, bool includeBeacon)
        {
            if (!includeBeacon && Beacon.Item1 == x && Beacon.Item2 == y)
            {
                return false;
            }

            return CalculateManhattanDistance(x, y) <= ManhattanDistance;
        }

        public IEnumerable<Tuple<long, long>> GetPointsNotCoveredJustOutsidePerimeter(long most)
        {
            // Calculating manhattan perimeter for each quadrant
            //      0
            //  T   |   T
            //  L   |   R
            //      |
            // 0----x----X
            //  B   |   B
            //  L   |   R
            //      |
            //      Y
            
            // Start with T -> TL
            var currentX = Coordinates.Item1;
            var currentY = Coordinates.Item2 - (ManhattanDistance + 1);

            // Quadrant: TL
            for(int i=0; i<ManhattanDistance + 1; i++)
            {
                if (currentX >= 0 && currentX <= most && currentY >= 0 && currentY <= most)
                {
                    yield return new Tuple<long,long>(currentX, currentY);
                }

                currentX--;
                currentY++;
            }

            // Quadrant: BL
            for(int i=0; i<ManhattanDistance + 1; i++)
            {
                if (currentX >= 0 && currentX <= most && currentY >= 0 && currentY <= most)
                {
                    yield return new Tuple<long,long>(currentX, currentY);
                }

                currentX++;
                currentY++;
            }

            // Quadrant: BR
            for(int i=0; i<ManhattanDistance + 1; i++)
            {
                if (currentX >= 0 && currentX <= most && currentY >= 0 && currentY <= most)
                {
                    yield return new Tuple<long,long>(currentX, currentY);
                }

                currentX++;
                currentY--;
            }

            // Quadrant: TR
            for(int i=0; i<ManhattanDistance + 1; i++)
            {
                if (currentX >= 0 && currentX <= most && currentY >= 0 && currentY <= most)
                {
                    yield return new Tuple<long,long>(currentX, currentY);
                }

                currentX--;
                currentY--;
            }
        }

        public long GetMinX() => Coordinates.Item1 - ManhattanDistance;

        public long GetMaxX() => Coordinates.Item1 + ManhattanDistance;

        private long CalculateManhattanDistance(long x, long y)
        {
            return Math.Abs(x - Coordinates.Item1) + Math.Abs(y - Coordinates.Item2);
        }
    }
}