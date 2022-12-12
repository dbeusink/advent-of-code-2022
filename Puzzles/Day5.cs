using System.Text.RegularExpressions;
namespace advent_of_code_2022.Puzzles;

internal class Day5 : PuzzleBase
{
    private Dictionary<int, Stack<char>> _stacks = new();

    public Day5() : base(nameof(Day5)) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();
        LoadStacks();

        var moveRegex = new Regex(@"move (\d{1,}) from (\d{1,}) to (\d{1,})");
        var moves = Input!.Skip(Array.IndexOf(Input!, string.Empty) + 1).Select(x =>
        {
            var match = moveRegex.Match(x);
            return new { Count = int.Parse(match.Groups[1].Value), From = int.Parse(match.Groups[2].Value), To = int.Parse(match.Groups[3].Value) };
        });

        foreach(var move in moves)
        {
            for(int i=0; i<move.Count; i++)
            {
                var item = _stacks[move.From].Pop();
                _stacks[move.To].Push(item);
            }
        }

        return string.Concat(_stacks.Select(x => x.Value.Peek()));
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();
        LoadStacks();

        var moveRegex = new Regex(@"move (\d{1,}) from (\d{1,}) to (\d{1,})");
        var moves = Input!.Skip(Array.IndexOf(Input!, string.Empty) + 1).Select(x =>
        {
            var match = moveRegex.Match(x);
            return new { Count = int.Parse(match.Groups[1].Value), From = int.Parse(match.Groups[2].Value), To = int.Parse(match.Groups[3].Value) };
        });

        foreach(var move in moves)
        {
            var itemsToMove = new List<char>();
            for(int i=0; i<move.Count; i++)
            {
                itemsToMove.Insert(0, _stacks[move.From].Pop()); // Last item will be on top
            }
            itemsToMove.ForEach(x => _stacks[move.To].Push(x));
        }

        return string.Concat(_stacks.Select(x => x.Value.Peek()));
    }

    private void LoadStacks()
    {
        // Create stacks
        if (_stacks.Count > 0)
        {
            _stacks.Clear();
        }
        var stackAmount = (Input![0].Length + 1) / 4; // Add virtual space to calculate correctly (last line contains no terminating space). Syntax = '[' + Letter + ']' + space = 4
        for(int i=1; i<=stackAmount; i++)
        {
            _stacks.Add(i, new Stack<char>());
        }

        // Read stacks from bottom to top
        var lastCrateLine = Array.IndexOf(Input!, string.Empty) - 2; // Remove 1 for numbers line
        for(int i=lastCrateLine; i>=0; i--)
        {
            for(int j=1; j<=stackAmount; j++)
            {
                char crate = Input[i][((j-1)*4)+1];
                if (!char.IsWhiteSpace(crate))
                {
                    _stacks[j].Push(crate);
                }
            }
        }
    }
}