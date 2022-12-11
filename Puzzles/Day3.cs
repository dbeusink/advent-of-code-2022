using System.Diagnostics;
namespace advent_of_code_2022.Puzzles;

internal class Day3 : PuzzleBase
{
    public Day3() : base(nameof(Day3)) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();
        var result = Input!.Select(x =>
        {
            var a = x[..(x.Length / 2)].ToCharArray();
            var b = x[(x.Length / 2)..].ToCharArray();
            return a.Intersect(b).Single();
        })
        .Sum(x => GetItemPriority(x));

        return result.ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();
        var result = Input!.Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / 3)
            .Select(x =>
            {
                var items = x.Select(x => x.Value.ToArray()).ToArray();
                return items[0].Intersect(items[1]).Intersect(items[2]).Single();
            })
            .Sum(x => GetItemPriority(x));

        return result.ToString();
    }

    public static int GetItemPriority(char item)
    {
        if (item >= 'a' && item <= 'z')
        {
            return (short)item - (short)'a' + 1;
        }
        else if (item >= 'A' && item <= 'Z')
        {
            return (short)item - (short)'A' + 27;
        }

        throw new InvalidOperationException($"Could not get item priority for item {item}");
    }
}