using System.Diagnostics;
using System.Reflection;
namespace advent_of_code_2022.Puzzles
{
    internal abstract class PuzzleBase
    {
        public string Name { get; protected set; } = string.Empty;
        protected string[]? Input { get; set; }

        public PuzzleBase(string puzzleName)
        {
            Name = puzzleName;
        }

        public virtual void Initialize()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new InvalidDataException("Could not initialize puzzle because it's name is not set.");
            }

            Input = InputLoader.LoadPuzzleInputByName(Name);
            Console.WriteLine($"Loaded {Input?.Length ?? 0} lines as puzzle input for puzzle '{Name}'");
        }

        public void AssertInputLoaded()
        {
            if (Input == null || Input.Length == 0)
            {
                throw new InvalidOperationException($"The input for puzzle '{Name}' is not loaded!");
            }
        }

        public abstract string SolvePart1();
        public abstract string SolvePart2();
    }
}