namespace advent_of_code_2022.Puzzles;

internal class Day6 : PuzzleBase
{
    public Day6() : base(nameof(Day6)) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();
        return GetFirstStartOfDistinctSequence(4).ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();
        return GetFirstStartOfDistinctSequence(14).ToString();
    }

    private int GetFirstStartOfDistinctSequence(int sequenceSize)
    {
        var message = Input![0];
        int currentPos = 0;
        while(currentPos < message.Length - sequenceSize)
        {
            if(message[currentPos..(currentPos+sequenceSize)].Distinct().Count() == sequenceSize)
            {
                break;
            }
            currentPos++;
        }

        return currentPos + sequenceSize;
    }
}