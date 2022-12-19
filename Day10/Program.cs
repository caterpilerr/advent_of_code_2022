// See https://aka.ms/new-console-template for more information
const string filePath = "input.txt";

var input = File.ReadAllLines(filePath);

const int addXCycles = 2;
const int noopCycles = 1;

// Part 1 & 2
var regX = 1;
var cycle = 0;
var result = new List<int>();
foreach (var line in input)
{
    var data = line.Split(' ');
    var op = data[0];
    var cycleLength = 0;
    var value = 0;
    switch (op)
    {
        case "addx":
            cycleLength = addXCycles;
            value = int.Parse(data[1]);
            break;
        case "noop":
            cycleLength = noopCycles;
            value = 0;
            break;
    }

    for (var i = 0; i < cycleLength; i++)
    {
        cycle++;
        DrawPixel(cycle, regX);
        CheckRegX(cycle, regX, result);
    }

    regX += value;
}

Console.WriteLine($"The sum of signal strength: {result.Sum()}");

void CheckRegX(int currentCycle, int reg, ICollection<int> sum)
{
    if ((currentCycle - 20) % 40 == 0)
    {
        sum.Add(currentCycle * reg);
    }
}

void DrawPixel(int currentCycle, int currentRegX)
{
    var offset = (currentCycle - 1) % 40;
    if (offset >= currentRegX - 1 &&
        offset <= currentRegX + 1)
    {
        Console.Write('#');
    }
    else
    {
        Console.Write('.');
    }

    if (offset == 39)
    {
        Console.Write('\n');
    } 
}