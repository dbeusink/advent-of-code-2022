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

        var packets = GetPackets().ToArray();
        var validator = new DistressMessageValidator();

        foreach(var packet in GetPackets())
        {
            validator.IsValidPacket(packet);
        }
        var debug = validator.GetIndicesOfSeparators(2, 6).ToList();
        var result = debug.Aggregate(1, (x,y) => x * y);

        return result.ToString();
    }

    private IEnumerable<Tuple<string, string>> GetPackets()
    {
        foreach (var pair in Input!.Where(x => x.StartsWith('[')).Chunk(2))
        {
            yield return new Tuple<string, string>(pair[0], pair[1]);
        }
    }

    private class DistressMessageValidator
    {
        private bool _newPacket;
        private List<int> _validatedMessageHeaders = new();

        public bool IsValidPacket(Tuple<string, string> packetPair)
        {
            _newPacket = true;
            var left = JsonNode.Parse(packetPair.Item1)!.AsArray();
            var right = JsonNode.Parse(packetPair.Item2)!.AsArray();

            return IsValidPacketPair(left, right) > 0;
        }

        public IEnumerable<int> GetIndicesOfSeparators(params int[] separators)
        {
            var offset = 1;
            var orderedMessages = _validatedMessageHeaders.OrderBy(x => x);
            foreach(var separator in separators)
            {
                yield return orderedMessages.TakeWhile(x => x <= separator).Count() + offset++;
            }

        }

        private int IsValidPacketPair(JsonArray left, JsonArray right)
        {
            for (int i = 0; i < left.Count; i++)
            {
                if (i >= right.Count)
                {
                    // Store headers
                    if (_newPacket)
                    {
                        var leftNode = left[i];
                        if (leftNode is JsonValue leftValue)
                        {
                            _validatedMessageHeaders.Add(leftValue.GetValue<int>());
                        }
                        else
                        {
                            while (leftNode is JsonArray array)
                            {
                                if (array.Count == 0)
                                {
                                    _validatedMessageHeaders.Add(0);
                                    break;
                                }
                                else if (array[0] is JsonValue value)
                                {
                                    _validatedMessageHeaders.Add(value.GetValue<int>());
                                    break;
                                }
                                else
                                {
                                    leftNode = array[0];
                                }
                            }
                        }
                        _validatedMessageHeaders.Add(0);
                        _newPacket = false;
                    }
                    return -1;
                }

                if (left[i] is JsonValue leftJsonValue && right[i] is JsonValue rightJsonValue)
                {
                    var leftValue = leftJsonValue.GetValue<int>();
                    var rightValue = rightJsonValue.GetValue<int>();

                    // Store headers
                    if (_newPacket)
                    {
                        _validatedMessageHeaders.Add(leftValue);
                        _validatedMessageHeaders.Add(rightValue);
                        _newPacket = false;
                    }

                    if (leftValue != rightValue)
                    {
                        return (leftValue < rightValue) ? 1 : -1;
                    }
                }
                else
                {
                    var leftArray = left[i] is JsonArray al ? al : new JsonArray() { left[i]!.GetValue<int>() };
                    var rightArray = right[i] is JsonArray ar ? ar : new JsonArray() { right[i]!.GetValue<int>() };
                    var result = IsValidPacketPair(leftArray, rightArray);
                    if (result != 0)
                    {
                        return result;
                    }
                }
            }

            if (right.Count > left.Count)
            {
                // Store headers
                if (_newPacket)
                {
                    _validatedMessageHeaders.Add(0);
                    var rightNode = right[0];
                    if (rightNode is JsonValue rightValue)
                    {
                        _validatedMessageHeaders.Add(rightValue.GetValue<int>());
                    }
                    else
                    {
                        while (rightNode is JsonArray array)
                        {
                            if (array.Count == 0)
                            {
                                _validatedMessageHeaders.Add(0);
                                break;
                            }
                            else if (array[0] is JsonValue value)
                            {
                                _validatedMessageHeaders.Add(value.GetValue<int>());
                                break;
                            }
                            else
                            {
                                rightNode = array[0];
                            }
                        }
                    }
                    _newPacket = false;
                }

                return 1;
            }

            return 0;
        }
    }
}