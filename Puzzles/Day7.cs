using System.Data;
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

        // Loop through all directories below 100000
        var dirs = new List<Directory>();
        AddSubDirs(root);

        return dirs.Sum(x => x.GetTotalSize()).ToString();

        void AddSubDirs(Directory dir)
        {
            foreach(var sub in dir.Subdirectories.Values)
            {
                if (sub.GetTotalSize() <= 100000)
                {
                    dirs.Add(sub);
                }
                AddSubDirs(sub);
            }
        }
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();
        return string.Empty;
    }

    private class TerminalInterpreter
    {
        private const string _rootCommand = "$ cd /";
        private Directory? _current;
        private readonly string[] _commands;
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
                throw new InvalidDataException($"Input file is not in the correct format! Expected '{_rootCommand}' got '{_commands[_currentPosition]}'");
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
            var listing = _commands.Skip(_currentPosition+1).TakeWhile(x => !x.StartsWith('$')).ToArray();
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
            int count = Files.Sum(x => x.Size);
            count += Subdirectories.Values.Sum(x => x.GetTotalSize());
            return count;
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