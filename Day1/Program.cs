// See https://aka.ms/new-console-template for more information
const string filePath= "input.txt";

var input = File.ReadAllLines(filePath);
var top3Elves = new[] {0, 0, 0};
var i = 0;
while(i < input.Length)
{
    var elfCalories = 0;
    while(i < input.Length && input[i] != string.Empty)
    {
        elfCalories += int.Parse(input[i]);
        i++;
    }

    CheckWithTopElves(top3Elves, elfCalories);
    i++;
}

var sum =  top3Elves.Sum();

Console.WriteLine($"Top elf calories: {top3Elves[0]}");
Console.WriteLine($"Max calories among top three elves: {sum}");

void CheckWithTopElves(IList<int> topElves, int newElf)
{
    if (topElves[0] < newElf)
    {
        topElves[2] = top3Elves[1];
        topElves[1] = top3Elves[0];
        topElves[0] = newElf;
    }
    else if (topElves[1] < newElf)
    {
        topElves[2] = top3Elves[1];
        topElves[1] = newElf;
    }
    else if (topElves[2] < newElf)
    {
        topElves[2] = newElf;
    }
}
