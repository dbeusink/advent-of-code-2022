using System.Numerics;

namespace advent_of_code_2022.Puzzles;

internal class Day9 : PuzzleBase
{
    public Day9() : base(nameof(Day9)) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();
        return Solve(2).Last().Visited.Count.ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();
        return Solve(10).Last().Visited.Count.ToString();
    }

    private Knot[] Solve(int knotCount)
    {
        var knots = new Knot[knotCount];
        for (int i = 0; i < knotCount; i++)
        {
            knots[i] = new Knot();
        }

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

                knots[0].Move(move);
                for (int j = 1; j < knots.Length; j++)
                {
                    knots[j].Follow(knots[j - 1]);
                }
            }
        }

        return knots;
    }

    private class Knot
    {
        public Vector2 CurrentPosition { get; private set; }

        public HashSet<Vector2> Visited { get; } = new();

        public Knot()
        {
            CurrentPosition = new Vector2(0, 0);
            Visited.Add(CurrentPosition);
        }

        public void Move(Vector2 vector)
        {
            CurrentPosition += vector;
            Visited.Add(CurrentPosition);
        }

        public void Follow(Knot other)
        {
            if (GetDistanceTo(other) > 1)
            {
                var dif = other.CurrentPosition - CurrentPosition;
                var move = new Vector2(Math.Sign(dif.X), Math.Sign(dif.Y));
                Move(move);
            }
        }

        public int GetDistanceTo(Knot otherKnot)
            => Convert.ToInt32((otherKnot.CurrentPosition - CurrentPosition).Length());
    }
}