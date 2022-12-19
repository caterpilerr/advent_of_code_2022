// See https://aka.ms/new-console-template for more information
const string FilePath = "input.txt";

var input = File.ReadAllLines(FilePath);

const int Addx_Cycles = 2;
const int Noop_Cycles = 1;

// Part 1 & 2
var reg_x = 1;
var cycle = 0;
var result = new List<int>();
foreach (var line in input)
{
    var data = line.Split(' ');
    var op = data[0];
    var cycle_length = 0;
    var value = 0;
    switch (op)
    {
        case "addx":
            cycle_length = Addx_Cycles;
            value = int.Parse(data[1]);
            break;
        case "noop":
            cycle_length = Noop_Cycles;
            value = 0;
            break;
    }

    for (var i = 0; i < cycle_length; i++)
    {
        cycle++;
        var offset = (cycle - 1) % 40;
        if (offset >= reg_x - 1 &&
            offset <= reg_x + 1)
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

        CheckRegX(cycle, reg_x, result);
    }

    reg_x += value;
}

Console.WriteLine($"The sum of signal strength: {result.Sum()}");

void CheckRegX(int cycle, int reg, ICollection<int> sum)
{
    if ((cycle - 20) % 40 == 0)
    {
        sum.Add(cycle * reg);
    }
}