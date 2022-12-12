using advent_of_code_2022;

Console.WriteLine("-- Advent of code 2022 --");
try
{
    var solver = new PuzzleSolver();
    if (args.Length == 1)
    {
        solver.SolvePuzzle(args[0]);
    }
    else
    {
        solver.SolveAllPuzzles();
    }
}
catch(Exception ex)
{
    Console.WriteLine($"Unhandled exception not caught by {nameof(PuzzleSolver)}: {ex.Message}");
}