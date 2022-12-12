using advent_of_code_2022;

Console.WriteLine("-- Advent of code 2022 --");
var solver = new PuzzleSolver();
try
{
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
    Console.WriteLine($"Failed to solve one or more puzzle(s): {ex.Message}");
}