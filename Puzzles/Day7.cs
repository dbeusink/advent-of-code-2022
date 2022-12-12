namespace advent_of_code_2022.Puzzles;

internal class Day7 : PuzzleBase
{
    public Day7() : base(nameof(Day7)) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();
        var root = new Directory(null, "/");
        var interpreter = new TerminalInterpreter(root, Input!);
        interpreter.Execute();

        // Get all directories wit a size <= 100000
        var dirs = new List<Directory>();
        AddDirectoriesBelowSize(root, 100000);

        var result = dirs.Sum(x => x.GetTotalSize());
        return result.ToString();

        // Recursive :)
        void AddDirectoriesBelowSize(Directory directory, int maxSize)
        {
            foreach(var sub in directory.Subdirectories.Values)
            {
                if (sub.GetTotalSize() <= maxSize)
                {
                    dirs.Add(sub);
                }
                AddDirectoriesBelowSize(sub, maxSize);
            }
        }
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();
        var root = new Directory(null, "/");
        var interpreter = new TerminalInterpreter(root, Input!);
        interpreter.Execute();

        // Calculate disk space required
        var unusedDiskSpace = 70000000 - root.GetTotalSize();
        var diskSpaceRequired = 30000000 - unusedDiskSpace;

        // Flatten directories
        var allDirectories = new List<Directory>() { root };
        AddDirectories(root);

        var result = allDirectories.OrderBy(x => x.GetTotalSize())
            .First(x => x.GetTotalSize() >= diskSpaceRequired)
            .GetTotalSize();

        return result.ToString();

        // Recursive :)
        void AddDirectories(Directory directory)
        {
            foreach(var sub in directory.Subdirectories.Values)
            {
                allDirectories.Add(sub);
                AddDirectories(sub);
            }
        }
    }

    private class TerminalInterpreter
    {
        private const string _rootCommand = "$ cd /";

        private readonly string[] _commands;
        private Directory? _current;
        private int _currentPosition;

        public TerminalInterpreter(Directory root, string[] commands)
        {
            _current = root;
            _commands = commands;
        }

        public void Execute()
        {
            // Verify start
            if (_commands[_currentPosition] != _rootCommand)
            {
                throw new InvalidDataException($"Input file is not in the correct format! " + 
                    "Expected '{_rootCommand}' got '{_commands[_currentPosition]}'");
            }

            _currentPosition = 1;
            while(_currentPosition < _commands.Length - 1)
            {
                var currentLine = _commands[_currentPosition];
                if (currentLine.StartsWith('$'))
                {
                    var command = currentLine[2..4];
                    _currentPosition += command switch
                    {
                        "ls" => ExecuteList(),
                        "cd" => ExecuteChangeDirectory(currentLine[5..]),
                        _ => throw new InvalidOperationException($"Command '{command}' is not supported")
                    };
                }
            }
        }

        private int ExecuteList()
        {
            var listing = _commands.Skip(_currentPosition+1)
                .TakeWhile(x => !x.StartsWith('$'))
                .ToArray();

            _current?.AddListing(listing);
            return listing.Length + 1;
        }

        private int ExecuteChangeDirectory(string directory)
        {
            if (directory == "..")
            {
                _current = _current?.Parent;
            }
            else
            {
                _current = _current?.Subdirectories[directory];
            }

            return 1;
        }
    }

    private class Directory
    {
        private int _totalSizeCache = -1;

        public string Name { get; }
        public Directory? Parent { get; }
        public Dictionary<string, Directory> Subdirectories { get; } = new();
        public List<File> Files { get; } = new();

        public Directory(Directory? parent, string name)
        {
            Parent = parent;
            Name = name;
        }

        public void AddListing(string[] listing)
        {
            foreach(var listItem in listing)
            {
                if (listItem.StartsWith("dir"))
                {
                    var name = listItem[4..];
                    Subdirectories.Add(name, new Directory(this, name));
                }
                else
                {
                    Files.Add(new File(listItem));
                }
            }
        }

        public int GetTotalSize()
        {
            if (_totalSizeCache == -1)
            {
                _totalSizeCache = Files.Sum(x => x.Size) + Subdirectories.Values.Sum(x => x.GetTotalSize());
            }

            return _totalSizeCache;
        }
    }

    private class File
    {
        public string Name { get; }
        public int Size { get; }

        public File(string input)
        {
            var splitted = input.Split(' ');
            Size = int.Parse(splitted[0]);
            Name = splitted[1];
        }
    }
}