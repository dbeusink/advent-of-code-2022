using System.Text;
using System.Text.RegularExpressions;

namespace advent_of_code_2022.Puzzles;

internal class Day11 : PuzzleBase
{
    public Day11() : base(nameof(Day11)) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();

        var gang = GetMonkeyGang();
        gang.Run(20, x => x / 3);

        return gang.CalculateMonkeyBusinessLevel().ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();

        var gang = GetMonkeyGang();
        var mod = gang.Monkeys.Values.Aggregate(1, (divisor, monkey) => divisor * monkey.TestDivisor);
        gang.Run(10000, x => x % mod);

        return gang.CalculateMonkeyBusinessLevel().ToString();
    }

    private MonkeyGang GetMonkeyGang()
    {
        var gang = new MonkeyGang();
        var sb = new StringBuilder();

        for (int i = 0; i < Input!.Length; i++)
        {
            var line = Input![i];
            if (string.IsNullOrEmpty(line))
            {
                gang.AddMonkey(sb.ToString());
                sb.Clear();
            }
            else if (i == Input!.Length - 1)
            {
                sb.AppendLine(line);
                gang.AddMonkey(sb.ToString());
            }
            else
            {
                sb.AppendLine(line);
            }
        }

        return gang;
    }

    private class MonkeyGang
    {
        public Dictionary<int, Monkey> Monkeys { get; } = new();

        public void Run(int rounds, Func<long, long> operationAfterInspection)
        {
            for (int i = 0; i < rounds; i++)
            {
                foreach (var monkey in Monkeys.Values)
                {
                    monkey.Inspect(operationAfterInspection);
                }
            }
        }

        public void AddMonkey(string monkeyInput)
        {
            var monkey = new Monkey(this, monkeyInput);
            Monkeys.Add(monkey.Id, monkey);
        }

        public long CalculateMonkeyBusinessLevel()
            => Monkeys.Values.Select(x => x.InspectionCount)
                .OrderByDescending(x => x)
                .Take(2)
                .Aggregate(1L, (x, y) => x * y);

        public void TossItemTo(long item, int monkey)
            => Monkeys[monkey].ReceiveItem(item);
    }

    private class Monkey
    {
        private static Regex _initRegex
            = new Regex(@"Mo.+(\d{1,}):\n.+: (.+)\n.+= (.+)\n.+by (\d{1,})\n.+key (\d{1,})\n.+key (\d{1,})",
                RegexOptions.Compiled);

        private MonkeyGang _gang;
        private Func<long, long> _inspectOperation;

        public int Id { get; }
        public Queue<long> Items { get; private set; }
        public int InspectionCount { get; private set; }
        public int TestDivisor { get; }
        public int MonkeyToTossToWhenTrue { get; }
        public int MonkeyToTossToWhenFalse { get; }

        public Monkey(MonkeyGang group, string input)
        {
            _gang = group;
            var result = _initRegex.Match(input);
            Id = int.Parse(result.Groups[1].Value);
            Items = new(result.Groups[2].Value.Split(", ").Select(long.Parse));
            _inspectOperation = ParseOperationExpression(result.Groups[3].Value);
            TestDivisor = int.Parse(result.Groups[4].Value);
            MonkeyToTossToWhenTrue = int.Parse(result.Groups[5].Value);
            MonkeyToTossToWhenFalse = int.Parse(result.Groups[6].Value);
        }

        public void ReceiveItem(long item)
            => Items.Enqueue(item);

        public void Inspect(Func<long, long> operationAfterInspection)
        {
            while (Items.TryDequeue(out var item))
            {
                item = _inspectOperation(item);
                item = operationAfterInspection(item);
                _gang.TossItemTo(item, item % TestDivisor == 0 ? MonkeyToTossToWhenTrue : MonkeyToTossToWhenFalse);
                InspectionCount++;
            }
        }

        private Func<long, long> ParseOperationExpression(string expression)
        {
            var expressionParts = expression.Split(' ');
            return (expressionParts[0], expressionParts[1], expressionParts[2]) switch
            {
                ("old", var operand, "old") => operand switch
                {
                    "+" => (x) => checked(x + x),
                    "-" => (x) => checked(x - x),
                    "*" => (x) => checked(x * x),
                    "/" => (x) => checked(x / x),
                    _ => throw new InvalidOperationException($"Unhandled operand '{operand}'")
                },
                ("old", var operand, var right)
                    when right.All(char.IsNumber) => operand switch
                    {
                        "+" => (x) => checked(x + int.Parse(right)),
                        "-" => (x) => checked(x - int.Parse(right)),
                        "*" => (x) => checked(x * int.Parse(right)),
                        "/" => (x) => checked(x / int.Parse(right)),
                        _ => throw new InvalidOperationException($"Unhandled operand '{operand}'")
                    },
                (var left, var operand, var right)
                    when left.All(char.IsNumber) && right.All(char.IsNumber) => operand switch
                    {
                        "+" => (x) => checked(int.Parse(left) + int.Parse(right)),
                        "-" => (x) => checked(int.Parse(left) - int.Parse(right)),
                        "*" => (x) => checked(int.Parse(left) * int.Parse(right)),
                        "/" => (x) => checked(int.Parse(left) / int.Parse(right)),
                        _ => throw new InvalidOperationException($"Unhandled operand '{operand}'")
                    },
                _ => throw new InvalidOperationException($"Unsupported expression '{expression}'")
            };
        }
    }
}