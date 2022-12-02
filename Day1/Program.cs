// See https://aka.ms/new-console-template for more information
const string FilePath= "input.txt";

var input = File.ReadAllLines(FilePath);
var top3Elves = new int[] {0, 0, 0};
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

void CheckWithTopElves(int[] topElves, int newElf)
{
    if (top3Elves[0] < newElf)
    {
        top3Elves[2] = top3Elves[1];
        top3Elves[1] = top3Elves[0];
        top3Elves[0] = newElf;
    }
    else if (top3Elves[1] < newElf)
    {
        top3Elves[2] = top3Elves[1];
        top3Elves[1] = newElf;
    }
    else if (top3Elves[2] < newElf)
    {
        top3Elves[2] = newElf;
    }
}
