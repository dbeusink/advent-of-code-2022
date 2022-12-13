using System.Numerics;

namespace advent_of_code_2022.Puzzles;

internal class Day9 : PuzzleBase
{
    public Day9() : base(nameof(Day9)) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();

        var head = new Knot();
        var tail = new Knot();
        RunMoves((move) =>
        {
            head.Move(move);
            tail.Follow(head);
        });

        return tail.History.Count.ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();

        var knots = new Knot[10];
        for (int i = 0; i < 10; i++)
        {
            knots[i] = new Knot();
        }

        var head = knots[0];
        var tail = knots[9];

        RunMoves((move) =>
        {
            head.Move(move);
            for (int i = 1; i < knots.Length; i++)
            {
                knots[i].Follow(knots[i - 1]);
            }
        });

        return tail.History.Count.ToString();
    }

    private void RunMoves(Action<Vector2> stepAction)
    {
        foreach (var line in Input!)
        {
            var direction = line[0];
            var steps = int.Parse(line[2..]);

            for (int i = 0; i < steps; i++)
            {
                Vector2 move = direction switch
                {
                    'U' => new Vector2(0, 1),
                    'D' => new Vector2(0, -1),
                    'L' => new Vector2(-1, 0),
                    'R' => new Vector2(1, 0),
                    _ => throw new InvalidOperationException($"'{line[0]}' is not a valid direction")
                };

                stepAction(move);
            }
        }
    }

    private class Knot
    {
        public Vector2 CurrentPosition { get; private set; }

        public HashSet<Vector2> History { get; } = new();

        public Knot()
        {
            CurrentPosition = new Vector2(0, 0);
            History.Add(CurrentPosition);
        }

        public void Move(Vector2 vector)
        {
            CurrentPosition += vector;
            History.Add(CurrentPosition);
        }

        public void Follow(Knot other)
        {
            if (GetDistanceTo(other) > 1)
            {
                // Get direction
                var dif = other.CurrentPosition - CurrentPosition;
                Vector2 move = dif switch
                {
                    { X: var x, Y: var y } when x == 0 && y == 0 => new Vector2(0, 0), // No movement
                    { X: var x, Y: var y } when x == 0 && y > 0 => new Vector2(0, 1),  // Up
                    { X: var x, Y: var y } when x == 0 && y < 0 => new Vector2(0, -1), // Down
                    { X: var x, Y: var y } when x < 0 && y == 0 => new Vector2(-1, 0), // Left
                    { X: var x, Y: var y } when x > 0 && y == 0 => new Vector2(1, 0),  // Right
                    { X: var x, Y: var y } when x < 0 && y > 0 => new Vector2(-1, 1),  // Left-Up
                    { X: var x, Y: var y } when x < 0 && y < 0 => new Vector2(-1, -1), // Left-Down
                    { X: var x, Y: var y } when x > 0 && y > 0 => new Vector2(1, 1),   // Right-Up
                    { X: var x, Y: var y } when x > 0 && y < 0 => new Vector2(1, -1),  // Right-Down
                    _ => throw new InvalidOperationException("Could not calculate next move to follow other")
                };

                Move(move);
            }
        }

        public int GetDistanceTo(Knot otherKnot)
            => Convert.ToInt32((otherKnot.CurrentPosition - CurrentPosition).Length());
    }
}