using advent_of_code_2022;

var solver = new PuzzleSolver();

Console.WriteLine("Advent of code 2022");
if (args.Length == 1)
{
    solver.SolvePuzzle(args[0]);
}
else
{
    solver.SolveAllPuzzles();
}