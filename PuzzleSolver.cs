using System.Reflection;
using advent_of_code_2022.Puzzles;

namespace advent_of_code_2022
{
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
                Console.WriteLine($"Puzzle '{name}': Initializing puzzle...");
                puzzle.Initialize();
                Console.WriteLine($"Puzzle '{name}': Solving part 1...");
                var resultPart1 = puzzle.SolvePart1();
                Console.WriteLine($"Puzzle '{name}': Output part 1: {resultPart1}");
                var resultPart2 = puzzle.SolvePart2();
                Console.WriteLine($"Puzzle '{name}': Output part 2: {resultPart2}");
            }
            else
            {
                throw new InvalidOperationException($"Could not find a puzzle with name '{name}'");
            }
        }

        private void RegisterPuzzles()
        {
            // Auto-discover puzzles using reflection
            foreach (var puzzleType in Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.BaseType == typeof(PuzzleBase) && x.GetConstructor(Type.EmptyTypes) != null))
            {
                var puzzle = Activator.CreateInstance(puzzleType) as PuzzleBase 
                    ?? throw new InvalidOperationException($"Could not create a instance of type '{puzzleType}'");

                _puzzles.Add(puzzle.Name, puzzle);
                Console.WriteLine($"Registered puzzle with name '{puzzle.Name}'");
            }
        }
    }
}