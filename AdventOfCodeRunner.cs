using advent_of_code_2022.Puzzles;

namespace advent_of_code_2022
{
    public class AdventOfCodeRunner
    {
        private readonly PuzzleSolver _solver;

        public AdventOfCodeRunner()
        {
            _solver = new PuzzleSolver();
            RegisterPuzzles();
        }

        private void RegisterPuzzles()
        {
            _solver.RegisterPuzzle(new Day1());
        }

        public void Run(string puzzleName)
        {
            _solver.SolvePuzzle(puzzleName);
        }
    }
}