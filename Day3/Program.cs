// See https://aka.ms/new-console-template for more information
const string FilePath = "input.txt";

var input = File.ReadAllLines(FilePath);

// Part 1
var unorderedItems = new char[input.Length];
for (var j = 0; j < input.Length; j++)
{
    var rucksack = input[j];
    var firstCompartment = new HashSet<char>();
    var secondCompartment = new HashSet<char>();
    var compartmentSize = rucksack.Length / 2;
    for (var i = 0; i < compartmentSize; i++)
    {
        var firstCompartmentItem = rucksack[i];
        var secondCompartmentItem = rucksack[i + compartmentSize];
        firstCompartment.Add(firstCompartmentItem);
        secondCompartment.Add(secondCompartmentItem);
        if (firstCompartment.Contains(secondCompartmentItem))
        {
            unorderedItems[j] = secondCompartmentItem;
            break;
        }
        else if (secondCompartment.Contains(firstCompartmentItem))
        {
            unorderedItems[j] = firstCompartmentItem;
            break;
        }
    }
}

// Part 2
var elvesInGroup = 3;
var elvesGroupNumber = input.Length / elvesInGroup;
var elvesGroups = new char[elvesGroupNumber];
for (var i = 0; i < elvesGroupNumber; i++)
{
    var sets = new HashSet<char>[elvesInGroup];
    for (var j = 0; j < elvesInGroup; j++)
    {
        var current = new HashSet<char>();
        foreach (var item in input[elvesInGroup * i + j])
        {
            current.Add(item);
        }
        sets[j] = current;
    }

    var distinct = sets[0];
    for (var k = 1; k < sets.Length; k++)
    {
        distinct.IntersectWith(sets[k]);
    }

    elvesGroups[i] = distinct.Single();
}

int CountPriorities(IEnumerable<char> data)
{
    return data.Aggregate(0, (current, item) => item switch
    {
        >= 'a' and <= 'z' => current += (int)(item - 'a') + 1,
        >= 'A' and <= 'Z' => current += (int)(item - 'A') + 1 + 26,
        _ => current
    });
}

var sumOfUnorderedItems = CountPriorities(unorderedItems);
Console.WriteLine($"The sum of the priorites for unordered items: {sumOfUnorderedItems}");

var sumOfElvesItemGroups = CountPriorities(elvesGroups);
Console.WriteLine($"The sum of the priorites for elves groups: {sumOfElvesItemGroups}");