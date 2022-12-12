using System.Text.RegularExpressions;
namespace advent_of_code_2022.Puzzles;

internal class Day4 : PuzzleBase
{
    public Day4() : base(nameof(Day4)) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();
        var result = Input!.Count(x => 
            {
                var match = Regex.Match(x, @"(\d{1,})-(\d{1,}),(\d{1,})-(\d{1,})");
                var elfA = new { Start = int.Parse(match.Groups[1].Value), End = int.Parse(match.Groups[2].Value) };
                var elfB = new { Start = int.Parse(match.Groups[3].Value), End = int.Parse(match.Groups[4].Value) };
                return (elfB.Start >= elfA.Start && elfB.End <= elfA.End) || (elfA.Start >= elfB.Start && elfA.End <= elfB.End);
            });

        return result.ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();
        var result = Input!.Count(x => 
            {
                var match = Regex.Match(x, @"(\d{1,})-(\d{1,}),(\d{1,})-(\d{1,})");
                int startA = int.Parse(match.Groups[1].Value);
                int endA = int.Parse(match.Groups[2].Value);
                int startB = int.Parse(match.Groups[3].Value);
                int endB = int.Parse(match.Groups[4].Value);
                
                var elfA = Enumerable.Range(startA, endA - startA + 1);
                var elfB = Enumerable.Range(startB, endB - startB + 1);
                return elfA.Intersect(elfB).Any();
            });

        return result.ToString();
    }
}