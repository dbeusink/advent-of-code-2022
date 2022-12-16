using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace advent_of_code_2022.Puzzles;

internal class Day13 : PuzzleBase
{
    public Day13() : base(nameof(Day13)) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();

        var packets = GetPackets().ToArray();
        var validator = new DistressMessageValidator();

        var result = GetPackets().Select((packet, index) =>
            validator.IsValidPacket(packet) ? index + 1 : 0)
            .Sum();

        return result.ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();

        return "";
    }

    private IEnumerable<Tuple<string, string>> GetPackets()
    {
        foreach(var pair in Input!.Where(x => x.StartsWith('[')).Chunk(2))
        {
            yield return new Tuple<string, string>(pair[0], pair[1]);
        }
    }

    private class DistressMessageValidator
    {
        private bool _isStart = false;
        public List<string> Packets { get; set; } = new();

        public bool IsValidPacket(Tuple<string, string> packetPair)
        {
            _isStart = false;
            var left = JsonNode.Parse(packetPair.Item1)!.AsArray();
            var right = JsonNode.Parse(packetPair.Item2)!.AsArray();

            return IsValidPacketPair(left, right) > 0;
        }

        private int IsValidPacketPair(JsonArray left, JsonArray right)
        {
            for(int i=0; i<left.Count; i++)
            {
                if (i >= right.Count)
                {
                    return -1;
                }

                if (left[i] is JsonValue leftJsonValue && right[i] is JsonValue rightJsonValue)
                {
                    var leftValue = leftJsonValue.GetValue<int>();
                    var rightValue = rightJsonValue.GetValue<int>();
                    if (leftValue != rightValue)
                    {
                        return (leftValue < rightValue) ? 1 : -1;
                    }
                }
                else
                {
                    var leftArray = left[i] is JsonArray al ? al : new JsonArray(){ left[i]!.GetValue<int>() };
                    var rightArray = right[i] is JsonArray ar ? ar : new JsonArray(){ right[i]!.GetValue<int>() };
                    var result = IsValidPacketPair(leftArray, rightArray);
                    if (result != 0)
                    {
                        return result;
                    }
                }
            }

            if (right.Count > left.Count)
            {
                return 1;
            }

            return 0;
        }
    }
}