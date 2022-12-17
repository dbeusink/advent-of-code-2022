using System.Text.Json.Nodes;

namespace advent_of_code_2022.Puzzles;

// This was by far the hardest one so far. Part 1 was very easy because I immediately took the JsonNode parsing approach.
// Part 2 on the on the other side... yeez, it took me way to long to figure that one out due to a little bug in the code :(
//
internal class Day13 : PuzzleBase
{
    public Day13() : base(nameof(Day13)) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();

        var result = GetPackets()
            .Chunk(2) // Get the packets in pairs
            .Select((packets, index) => packets[0].CompareTo(packets[1]) < 0 ? index + 1 : 0) // Compare returns -1 when a packet is in the right order
            .Sum();

        return result.ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();

        var dividerPackets = new Packet[] { new Packet("[[2]]"), new Packet("[[6]]") };

        // Jup overkill, but I hate fixed input. When the Elves will update the distress protocol
        // we will be ready for some more divider packets ;)
        //
        var result = GetPackets()
            .Concat(dividerPackets)
            .OrderBy(x => x)
            .Select((p, i) => new { Index = i + 1, Packet = p })
            .IntersectBy(dividerPackets, x => x.Packet)
            .Aggregate(1, (aggregation, x) => x.Index * aggregation);
    
        return result.ToString();
    }

    private IEnumerable<Packet> GetPackets()
        => Input!.Where(x => x.StartsWith('[')).Select(x => new Packet(x));

    private class Packet : IComparable<Packet>
    {
        public JsonArray Message { get; }

        public Packet(string message)
        {
            Message = JsonNode.Parse(message)!.AsArray();
        }

        public int CompareTo(Packet? other)
        {
            return Compare(Message, other!.Message);
        }

        private int Compare(JsonNode leftNode, JsonNode rightNode)
        {
            if (leftNode is JsonValue leftValue && rightNode is JsonValue rightValue)
            {  
                var left = leftValue.GetValue<int>();
                var right = rightValue.GetValue<int>();
                return left - right;
            }
            else
            {
                var leftArray = leftNode as JsonArray ?? new JsonArray() { leftNode.GetValue<int>() };
                var rightArray = rightNode as JsonArray ?? new JsonArray() { rightNode.GetValue<int>() };

                // Zip the arrays as far as possible and compare
                return Enumerable.Zip(leftArray, rightArray).Select(x => Compare(x.First!, x.Second!))
                    .FirstOrDefault(x => x != 0, leftArray.Count - rightArray.Count);
            }
        }
    }
}