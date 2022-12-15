using System.Reflection;
using advent_of_code_2022.Puzzles;
namespace advent_of_code_2022;

internal class PuzzleSolver
{
    private readonly Dictionary<string, PuzzleBase> _puzzles = new();

    public PuzzleSolver()
    {
        RegisterPuzzles();
    }

    public void SolveAllPuzzles()
    {
        foreach(var puzzleName in _puzzles.Keys)
        {
            SolvePuzzle(puzzleName);
        }
    }

    public void SolvePuzzle(string name)
    {
        if (_puzzles.TryGetValue(name, out var puzzle))
        {
            try
            {
                puzzle.Initialize();
                Console.WriteLine($"Puzzle \u001b[1;92m'{name}'\u001b[0m: Solving puzzle...");
                var resultPart1 = puzzle.SolvePart1();
                Console.WriteLine($"Puzzle \u001b[1;92m'{name}'\u001b[0m: Output part 1: \u001b[1;96m{resultPart1}\u001b[0m");
                var resultPart2 = puzzle.SolvePart2();
                Console.WriteLine($"Puzzle \u001b[1;92m'{name}'\u001b[0m: Output part 2: \u001b[1;96m{resultPart2}\u001b[0m");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Puzzle \u001b[1;92m'{name}'\u001b[0m: \u001b[1;91mException! {ex.Message}\u001b[0m");
            }
        }
        else
        {
            Console.WriteLine($"Puzzle \u001b[1;92m'{name}'\u001b[0m: \u001b[1;93mPuzzle not found!\u001b[0m");
        }
    }

    private void RegisterPuzzles()
    {
        // Auto-discover puzzles using reflection
        foreach (var puzzleType in Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => x.BaseType == typeof(PuzzleBase) && x.GetConstructor(Type.EmptyTypes) != null)
            .OrderBy(x => x.Name[4..]))
        {
            var puzzle = Activator.CreateInstance(puzzleType) as PuzzleBase 
                ?? throw new InvalidOperationException($"Could not create a instance of type '{puzzleType}'");

            _puzzles.Add(puzzle.Name, puzzle);
        }

        Console.WriteLine("Puzzles discovered:");
        foreach (var chunk in _puzzles.Keys.Chunk(5))
        {
            Console.WriteLine($"\u001b[1;96m{string.Join('\t', chunk)}\u001b[0m");
        }
    }
}