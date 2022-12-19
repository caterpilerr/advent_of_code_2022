// See https://aka.ms/new-console-template for more information
const string filePath = "input.txt";

var input = File.ReadAllLines(filePath);
var data = new int[input.Length][];
for (var i = 0; i < input.Length; i++)
{
    var elvesPair = input[i].Split(',');
    data[i] = elvesPair.SelectMany(
        elfData => elfData.Split('-').Select(int.Parse)
    ).ToArray();
}

var invalidPairs = 0;
var overlappingPairs = 0;
foreach (var item in data)
{
    // Part 1
    var startDelta = item[0] - item[2];
    var endDelta = item[1] - item[3];
    if (startDelta == 0 ||
        endDelta == 0 ||
        startDelta > 0 == endDelta < 0)
    {
        invalidPairs++;
    }

    // Part 2
    var firstDelta = item[2] - item[1];
    var secondDelta = item[0] - item[3];
    if (firstDelta > 0 && secondDelta < 0 ||
        firstDelta < 0 && secondDelta > 0)
    {
        continue;
    }

    overlappingPairs++;
}

Console.WriteLine($"The number of invalid elves pairs: {invalidPairs}");
Console.WriteLine($"The number of overlapping elves pairs: {overlappingPairs}");

