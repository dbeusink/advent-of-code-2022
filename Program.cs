using advent_of_code_2022;

Console.WriteLine("\u001b[1;92m-- Advent of code 2022 --\u001b[0m");

var commands = GetCommands();
GlobalSettings.LoadFromCommands(commands);

try
{
    var solver = new PuzzleSolver();
    if (commands.TryGetValue('d', out var day))
    {
        if (day.All(char.IsNumber))
        {
            solver.SolvePuzzle($"Day{day}");
        }
        else
        {
            Console.WriteLine($"\u001b[1;91m{day} is not a valid day input. Please try again with a valid day number.\u001b[0m");
        }
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

Dictionary<char, string> GetCommands()
{
    var commands = new Dictionary<char, string>();
    for(int i=0; i<args.Length; i++)
    {
        switch(args[i])
        {
            case "-d":
            {
                commands.Add('d', args[i++ + 1]);
                break;
            }
            case "-v":
            {
                commands.Add('v', string.Empty);
                break;
            }
        }
    }

    return commands;
}