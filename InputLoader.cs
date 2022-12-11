namespace advent_of_code_2022;

internal static class InputLoader
{
    public static string[] LoadPuzzleInputByName(string puzzleName)
    {
        var path = Path.Combine("Input", $"{puzzleName}.txt");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Could not load puzzle input from path '{path}'. " +
                "Please make sure that the puzzle input is available.", path);
        }

        return File.ReadAllLines(path);
    }
}