namespace advent_of_code_2022.Puzzles;

internal abstract class PuzzleBase
{
    public string Name { get; }
    public bool IsLongRunning { get; }
    protected string[]? Input { get; private set; }

    public PuzzleBase(string puzzleName, bool longRunning = false)
    {
        Name = puzzleName;
        IsLongRunning = longRunning;
    }

    public void Initialize()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidDataException("Could not initialize puzzle because it's name is not set.");
        }

        Input = InputLoader.LoadPuzzleInputByName(Name);
        Console.WriteLine($"Puzzle \u001b[1;92m'{Name}'\u001b[0m: Read \u001b[1;95m{Input?.Length ?? 0}\u001b[0m lines as puzzle input");
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