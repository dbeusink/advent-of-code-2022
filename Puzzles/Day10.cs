namespace advent_of_code_2022.Puzzles;

internal class Day10 : PuzzleBase
{
    public Day10() : base(nameof(Day10)) { }

    public override string SolvePart1()
    {

        AssertInputLoaded();

        var cycles = GetCycles();
        var clockCyclesToInspect = new List<int>() { 20, 60, 100, 140, 180, 220 };
        var signalStrength = cycles.Select((x, i) => new { Cycle = i + 1, Register = x })
            .Where(x => clockCyclesToInspect.Contains(x.Cycle))
            .Sum(x => x.Cycle * x.Register);

        return signalStrength.ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();

        var cycles = GetCycles();

        var displayCol = 0;
        var displayRow = 0;
        foreach (var cycle in cycles)
        {
            if (displayCol >= cycle - 1 && displayCol <= cycle + 1)
            {
                Console.Write('#');
            }
            else
            {
                Console.Write('.');
            }

            if (displayCol == 39)
            {
                displayCol = 0;
                displayRow++;
                Console.WriteLine();
            }
            else
            {
                displayCol++;
            }
        }

        return "LOOK UP :)";
    }

    private List<int> GetCycles()
    {
        var x = 1;
        var cycles = new List<int>(Input!.Length) { x };

        foreach (var instruction in Input!)
        {
            if (instruction == "noop")
            {
                cycles.Add(x);
            }
            else
            {
                var addition = int.Parse(instruction[5..]);
                cycles.Add(x);
                cycles.Add(x += addition);
            }
        }

        return cycles;
    }
}