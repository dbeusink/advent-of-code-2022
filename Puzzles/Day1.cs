namespace advent_of_code_2022.Puzzles;

internal class Day1 : PuzzleBase
{
    public Day1() : base(nameof(Day1)) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();
        var elfFoodBags = GetElfFoodBags();
        var totalCalorieBags = CalculateTotalCalorieBags(elfFoodBags).Max();
        return totalCalorieBags.ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();
        var elfFoodBags = GetElfFoodBags();
        var totalCaloriesTop3Elfs = CalculateTotalCalorieBags(elfFoodBags)
            .OrderByDescending(x => x)
            .Take(3)
            .Sum();
        return totalCaloriesTop3Elfs.ToString();
    }

    private List<List<int>> GetElfFoodBags()
    {
        var bags = new List<List<int>>();
        bool newElf = true;
        List<int> currentElf = new();
        foreach(var line in Input ?? Enumerable.Empty<string>())
        {
            if (newElf)
            {
                currentElf = new List<int>();
                bags.Add(currentElf);
                newElf = false;
            }

            if (string.IsNullOrEmpty(line))
            {
                newElf = true;
            }
            else
            {
                currentElf!.Add(int.Parse(line));
            }
        }
        return bags;
    }

    private List<int> CalculateTotalCalorieBags(List<List<int>> elfs)
    {
        var result = new List<int>(elfs.Count);
        foreach(var elf in elfs)
        {
            result.Add(elf.Sum());
        }
        return result;
    }
}