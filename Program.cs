using advent_of_code_2022;

Console.WriteLine("\u001b[1;92m-- Advent of code 2022 --\u001b[0m");
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
    Console.WriteLine($"\u001b[1;91mUnhandled exception not caught by {nameof(PuzzleSolver)}: {ex.Message}\u001b[0m");
}