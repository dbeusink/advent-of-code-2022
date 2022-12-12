namespace advent_of_code_2022.Puzzles;

internal abstract class PuzzleBase
{
    public string Name { get; private set; }
    protected string[]? Input { get; private set; }

    public PuzzleBase(string puzzleName)
    {
        Name = puzzleName;
    }

    public void Initialize()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidDataException("Could not initialize puzzle because it's name is not set.");
        }

        Input = InputLoader.LoadPuzzleInputByName(Name);
        Console.WriteLine($"Puzzle '{Name}': Read {Input?.Length ?? 0} lines as puzzle input");
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