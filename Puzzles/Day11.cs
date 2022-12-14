using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace advent_of_code_2022.Puzzles;

internal class Day11 : PuzzleBase
{
    public Day11() : base(nameof(Day11)) { }

    public override string SolvePart1()
    {

        AssertInputLoaded();

        var group = new MonkeyGroup();
        var sb = new StringBuilder();

        for(int i=0; i<Input!.Length; i++)
        {   
            var line = Input![i];
            if (string.IsNullOrEmpty(line))
            {
                new Monkey(group, sb.ToString());
                sb.Clear();
            }
            else if (i == Input!.Length - 1)
            {
                sb.AppendLine(line);
                new Monkey(group, sb.ToString());
            }
            else
            {
                sb.AppendLine(line);
            }
        }

        for(int i=0; i<20; i++)
        {
            foreach(var monkey in group.Monkeys)
            {
                monkey.Value.Inspect();
            }
        }
        return group.Monkeys.Select(x => x.Value.InspectionCount).OrderByDescending(x => x).Take(2).Aggregate(1, (x,y) => x * y).ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();

        var group = new MonkeyGroup();
        var sb = new StringBuilder();

        for(int i=0; i<Input!.Length; i++)
        {   
            var line = Input![i];
            if (string.IsNullOrEmpty(line))
            {
                new Monkey(group, sb.ToString());
                sb.Clear();
            }
            else if (i == Input!.Length - 1)
            {
                sb.AppendLine(line);
                new Monkey(group, sb.ToString(), true);
            }
            else
            {
                sb.AppendLine(line);
            }
        }

        for(int i=0; i<10000; i++)
        {
            foreach(var monkey in group.Monkeys)
            {
                monkey.Value.Inspect();
            }
        }
        return group.Monkeys.Select(x => x.Value.InspectionCount).OrderByDescending(x => x).Take(2).Aggregate(1, (x,y) => x * y).ToString();
    }

    private class MonkeyGroup
    {
        public Dictionary<int, Monkey> Monkeys { get; } = new();

        public void Register(Monkey monkey)
        {
            Monkeys.Add(monkey.Id, monkey);
        }

        public void TossItemTo(Item item, int monkey)
            => Monkeys[monkey].ReceiveItem(item);
    }

    private class Item
    {
        public BigInteger WorryLevel { get; set; }

        public Item(BigInteger initialWorryLevel)
        {
            WorryLevel = initialWorryLevel;
        }

        public void PerformOperation(Func<BigInteger,BigInteger> operation)
            => WorryLevel = operation(WorryLevel);
    }

    private class Monkey
    {
        private const bool _verboseMode = false;

        private static Regex _initRegex
            = new Regex(@"Mo.+(\d{1,}):\n.+: (.+)\n.+= (.+)\n.+by (\d{1,})\n.+key (\d{1,})\n.+key (\d{1,})", RegexOptions.Compiled);

        private MonkeyGroup _group;
        private Func<BigInteger,BigInteger> _operation;
        private readonly bool _part2;

        public int Id { get; }
        public int InspectionCount { get; private set; }
        public List<Item> Items { get; private set; }
        public Action<Item> Action { get; private set;}

        public Monkey(MonkeyGroup group, string input, bool part2 = false)
        {
            _group = group;
            var result = _initRegex.Match(input);
            Id = int.Parse(result.Groups[1].Value);
            Items = new(result.Groups[2].Value.Split(", ").Select(x => new Item(int.Parse(x))));
            _operation = ParseExpression(result.Groups[3].Value);
            Action = (x) =>
            {
                if (x.WorryLevel % int.Parse(result.Groups[4].Value) == 0)
                {
                    PrintWhenVerbose($"\tCurrent worry level is divisible by {result.Groups[4].Value}.");
                    PrintWhenVerbose($"\tItem with worry level {x.WorryLevel} is thrown to monkey {result.Groups[5].Value}.");
                    group.TossItemTo(x, int.Parse(result.Groups[5].Value));
                }
                else
                {
                    PrintWhenVerbose($"\tCurrent worry level is not divisible by {result.Groups[4].Value}.");
                    PrintWhenVerbose($"\tItem with worry level {x.WorryLevel} is thrown to monkey {result.Groups[6].Value}.");
                    group.TossItemTo(x, int.Parse(result.Groups[6].Value));
                }
            };
            _part2 = part2;
            _group.Register(this);
        }

        public void ReceiveItem(Item item)
        {
            Items.Add(item);
        }

        public void Inspect()
        {
            PrintWhenVerbose($"Monkey {Id}:");
            foreach(var item in Items)
            {
                PrintWhenVerbose($"Monkey inspects an item with a worry level of {item.WorryLevel}.");
                item.PerformOperation(_operation);
                PrintWhenVerbose($"\tWorry level changed to {item.WorryLevel}.");
                if (!_part2)
                {
                    item.PerformOperation(x => x / 3);
                    PrintWhenVerbose($"\tMonkey gets bored with item. Worry level is divided by 3 to {item.WorryLevel}.");
                }
                Action(item);
                InspectionCount++;
            }
            Items.Clear();
        }

        private Func<BigInteger,BigInteger> ParseExpression(string expression)
        {
            var expressionParts = expression.Split(' ');
            return (expressionParts[0], expressionParts[1], expressionParts[2]) switch
            {
                ("old", var operand, "old") => operand switch
                {
                    "+" => (x) => x + x,
                    "-" => (x) => x - x,
                    "*" => (x) => x * x,
                    "/" => (x) => x / x,
                    _ => throw new InvalidOperationException($"Unhandled operand '{operand}'")
                },
                ("old", var operand, var right)
                    when right.All(char.IsNumber) => operand switch
                {
                    "+" => (x) => x + int.Parse(right),
                    "-" => (x) => x - int.Parse(right),
                    "*" => (x) => x * int.Parse(right),
                    "/" => (x) => x / int.Parse(right),
                    _ => throw new InvalidOperationException($"Unhandled operand '{operand}'")
                },
                (var left, var operand, var right)
                    when left.All(char.IsNumber) && right.All(char.IsNumber) => operand switch
                {
                    "+" => (x) => int.Parse(left) + int.Parse(right),
                    "-" => (x) => int.Parse(left) - int.Parse(right),
                    "*" => (x) => int.Parse(left) * int.Parse(right),
                    "/" => (x) => int.Parse(left) / int.Parse(right),
                    _ => throw new InvalidOperationException($"Unhandled operand '{operand}'")
                },
                _ => throw new InvalidOperationException($"Unsupported expression '{expression}'")
            };
        }

        private static void PrintWhenVerbose(string message)
        {
            if (_verboseMode)
            {
                Console.WriteLine(message);
            }
        }
    }
}