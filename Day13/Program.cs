// See https://aka.ms/new-console-template for more information
using System.Text;
const string filePath = "input.txt";

var input = File.ReadAllLines(filePath);

// Part 1
var result = 0;
for (var i = 0; i < input.Length; i += 3)
{
    var leftInput = input[i];
    var rightInput = input[i + 1];

    var leftPacket = ParsePacket(leftInput);
    var rightPacket = ParsePacket(rightInput);

    var valid = PacketsComparer.CheckPackets(leftPacket, rightPacket);
    var index = i / 3 + 1;
    if (valid.HasValue)
    {
        if (valid.Value)
        {
            result += index;
        }
    }
    else
    {
        throw new InvalidOperationException();
    }
}

Console.WriteLine($"The sum of indices of valid pairs: {result}");

// Part 2
var packets = new List<List<object>>();
for (var i = 0; i < input.Length; i += 3)
{
    var leftInput = input[i];
    var rightInput = input[i + 1];

    var leftPacket = ParsePacket(leftInput);
    var rightPacket = ParsePacket(rightInput);

    packets.Add(leftPacket);
    packets.Add(rightPacket);
}

var firstDecoder = new List<object> { new List<object> { 2 } };
var secondDecoder = new List<object> { new List<object> { 6 } };
packets.Add(firstDecoder);
packets.Add(secondDecoder);
packets.Sort(new PacketsComparer());
var result2 = 1;
for (var i = 0; i < packets.Count; i++)
{
    PrintPacket(packets[i]);
    var firstCheck = PacketsComparer.CheckPackets(packets[i], firstDecoder);
    if (!firstCheck.HasValue)
    {
        result2 *= i + 1;
    }
    var secondCheck = PacketsComparer.CheckPackets(packets[i], secondDecoder);
    if (!secondCheck.HasValue)
    {
        result2 *= i + 1;
    }

    Console.Write('\n');
}

Console.WriteLine($"The decoder key for the distress signal: {result2}");

void PrintPacket(IReadOnlyList<object> packet)
{
    Console.Write('[');
    for (var i = 0; i < packet.Count; i++)
    {
        var item = packet[i];
        switch (item)
        {
            case List<object> subPacket:
                PrintPacket(subPacket);
                break;
            case int number:
                Console.Write(number);
                break;
        }

        if (i < packet.Count - 1)
        {
            Console.Write(',');
        }
    }

    Console.Write(']');
}

List<object> ParsePacket(string packetInput)
{
    var stack = new Stack<List<object>>();
    List<object> current = null!;
    var i = 0;
    while (i < packetInput.Length)
    {
        if (packetInput[i] == '[')
        {
            var list = new List<object>();
            if (current is not null)
            {
                current.Add(list);
                stack.Push(current);
            }

            current = list;
        }
        else if (char.IsDigit(packetInput[i]))
        {
            var numberInput = new StringBuilder();
            while (char.IsDigit(packetInput[i]))
            {
                numberInput.Append(packetInput[i]);
                i++;
            }

            var item = int.Parse(numberInput.ToString());
            current.Add(item);
            continue;
        }
        else if (packetInput[i] == ']')
        {
            if (stack.Count > 0)
            {
                current = stack.Pop();
            }
        }

        i++;
    }

    return current;
}

internal class PacketsComparer : IComparer<List<object>>
{
    public int Compare(List<object>? x, List<object>? y)
    {
        var result = CheckPackets(x!, y!);
        if (!result.HasValue) return 0;
        if (result.Value)
        {
            return -1;
        }

        return 1;
    }

    public static bool? CheckPackets(object left, object right)
    {
        if (left is int l && right is int r)
        {
            if (l < r)
            {
                return true;
            }

            if (l > r)
            {
                return false;
            }

            return null;
        }

        if (left is List<object> lArray && right is List<object> rArray)
        {
            var leftIndex = 0;
            var rightIndex = 0;
            while (leftIndex < lArray.Count && rightIndex < rArray.Count)
            {
                var leftItem = lArray[leftIndex];
                var rightItem = rArray[rightIndex];

                var result = CheckPackets(leftItem, rightItem);
                if (result is not null)
                {
                    return result;
                }

                leftIndex++;
                rightIndex++;
            }

            if (leftIndex < lArray.Count)
            {
                return false;
            }

            if (rightIndex < rArray.Count)
            {
                return true;
            }

            return null;
        }

        if (left is int leftSingle)
        {
            var leftModified = new List<object> { leftSingle };
            return CheckPackets(leftModified, right);
        }

        if (right is int rightSingle)
        {
            var rightModified = new List<object> { rightSingle };
            return CheckPackets(left, rightModified);
        }

        return null;
    }
}