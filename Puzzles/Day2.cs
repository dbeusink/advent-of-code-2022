namespace advent_of_code_2022.Puzzles;

internal class Day2 : PuzzleBase
{
    public Day2() : base(nameof(Day2)) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();
        var rounds = new List<RockPaperScissorsRound>();
        foreach (var line in Input ?? throw new InvalidOperationException("Input cannot be null"))
        {
            rounds.Add(GetRoundPart1(line));
        }
        var totalScore = rounds.Sum(x => x.CalculateScore());
        return totalScore.ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();
        var rounds = new List<RockPaperScissorsRound>();
        foreach (var line in Input ?? throw new InvalidOperationException("Input cannot be null"))
        {
            rounds.Add(GetRoundPart2(line));
        }
        var totalScore = rounds.Sum(x => x.CalculateScore());
        return totalScore.ToString();
    }

    public enum RockPaperScissors
    {
        Unknown = 0,
        Rock = 1,
        Paper = 2,
        Scissors = 3,
    }

    public enum Score
    {
        Lose = 0,
        Draw = 3,
        Win = 6,
    }

    private RockPaperScissorsRound GetRoundPart1(string input)
    {
        if (input.Length == 3 && char.IsWhiteSpace(input[1]))
        {
            var opponent = GetRockPaperScissorsByChar(input[0]);
            var player = GetRockPaperScissorsByChar(input[2]);
            return new RockPaperScissorsRound(opponent, player);
        }
        else
        {
            throw new InvalidDataException($"Input file is not correctly formatted. Line: {input} must be of format 'A X'");
        }
    }

    private RockPaperScissorsRound GetRoundPart2(string input)
    {
        if (input.Length == 3 && char.IsWhiteSpace(input[1]))
        {
            var opponent = GetRockPaperScissorsByChar(input[0]);
            var result = GetScoreResultByChar(input[2]);
            var player = result switch
            {
                Score.Lose => opponent switch
                {
                    RockPaperScissors.Rock => RockPaperScissors.Scissors,
                    RockPaperScissors.Paper => RockPaperScissors.Rock,
                    RockPaperScissors.Scissors => RockPaperScissors.Paper,
                    _ => throw new InvalidDataException($"Could not calculate losing hand for opponent '{opponent}' with round result '{result}'")
                },
                Score.Draw => opponent,
                Score.Win => opponent switch
                {
                    RockPaperScissors.Rock => RockPaperScissors.Paper,
                    RockPaperScissors.Paper => RockPaperScissors.Scissors,
                    RockPaperScissors.Scissors => RockPaperScissors.Rock,
                    _ => throw new InvalidDataException($"Could not calculate winning hand for opponent '{opponent}' with round result '{result}'")
                },
                _ => throw new InvalidDataException($"Could not calculate hand for opponent '{opponent}' with round result '{result}'")
            };
            
            return new RockPaperScissorsRound(opponent, player);
        }
        else
        {
            throw new InvalidDataException($"Input file is not correctly formatted. Line: {input} must be of format 'A X'");
        }
    }

    private RockPaperScissors GetRockPaperScissorsByChar(char symbol)
    {
        return symbol switch
        {
            'A' or 'X' => RockPaperScissors.Rock,
            'B' or 'Y' => RockPaperScissors.Paper,
            'C' or 'Z' => RockPaperScissors.Scissors,
            _ => throw new InvalidDataException($"Could not get rock, paper, or scissors from symbol '{symbol}'")
        };
    }

    private Score GetScoreResultByChar(char symbol)
    {
        return symbol switch
        {
            'X' => Score.Lose,
            'Y' => Score.Draw,
            'Z' => Score.Win,
            _ => throw new InvalidDataException($"Could not get score from symbol '{symbol}'")
        };
    }

    private class RockPaperScissorsRound
    {
        private readonly RockPaperScissors _opponent;
        private readonly RockPaperScissors _player;

        public RockPaperScissorsRound(RockPaperScissors opponent, RockPaperScissors player)
        {
            _opponent = opponent;
            _player = player;
        }

        public int CalculateScore()
        {
            return _player switch
            {
                RockPaperScissors.Rock => _opponent switch
                {
                    RockPaperScissors.Rock => (int)Score.Draw + (int)_player,
                    RockPaperScissors.Paper => (int)Score.Lose + (int)_player,
                    RockPaperScissors.Scissors => (int)Score.Win + (int)_player,
                    _ => throw new InvalidDataException($"Could not calculate score for player '{_player}' with opponent '{_opponent}'")
                },
                RockPaperScissors.Paper => _opponent switch
                {
                    RockPaperScissors.Rock => (int)Score.Win + (int)_player,
                    RockPaperScissors.Paper => (int)Score.Draw + (int)_player,
                    RockPaperScissors.Scissors => (int)Score.Lose + (int)_player,
                    _ => throw new InvalidDataException($"Could not calculate score for player '{_player}' with opponent '{_opponent}'")
                },
                RockPaperScissors.Scissors => _opponent switch
                {
                    RockPaperScissors.Rock => (int)Score.Lose + (int)_player,
                    RockPaperScissors.Paper => (int)Score.Win + (int)_player,
                    RockPaperScissors.Scissors => (int)Score.Draw + (int)_player,
                    _ => throw new InvalidDataException($"Could not calculate score for player '{_player}' with opponent '{_opponent}'")
                },
                _ => throw new InvalidDataException($"Could not calculate score for player '{_player}' with opponent '{_opponent}'")
            };
        }
    }
}