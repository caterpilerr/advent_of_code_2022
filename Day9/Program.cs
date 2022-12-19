// See https://aka.ms/new-console-template for more information
const string filePath = "input.txt";

var input = File.ReadAllLines(filePath);

// Part 1
var set1 = new HashSet<(int X, int Y)>();
var head = (X: 0, Y: 0);
var followUp = head;
set1.Add(followUp);
foreach (var line in input)
{
    var data = line.Split(' ');
    var direction = data[0][0];
    var length = int.Parse(data[1]);
    for (var i = 0; i < length; i++)
    {
        switch (direction)
        {
            case 'U':
                head.Y++;
                break;
            case 'R':
                head.X++;
                break;
            case 'L':
                head.X--;
                break;
            case 'D':
                head.Y--;
                break;
        }

        followUp = GetFollowUp(head, followUp);
        
        if (!set1.Contains(followUp)) 
        {
            set1.Add(followUp);
        }
    }
}

Console.WriteLine($"Number of unique visited cells for part 1: {set1.Count}");

// Part 2
var set2 = new HashSet<(int X, int Y)>();
head = (X: 0, Y: 0);
set2.Add(head);
var tail = new List<(int X, int Y)>();
for (var i = 0; i < 9; i++)
{
    tail.Add(head);
}

foreach (var line in input)
{
    var data = line.Split(' ');
    var direction = data[0][0];
    var length = int.Parse(data[1]);
    for (var i = 0; i < length; i++)
    {
        switch (direction)
        {
            case 'U':
                head.Y++;
                break;
            case 'R':
                head.X++;
                break;
            case 'L':
                head.X--;
                break;
            case 'D':
                head.Y--;
                break;
        }

        var current = head;
        for (var j = 0; j < tail.Count; j++)
        {
            var iModified = GetFollowUp(current, tail[j]);
            if (iModified == tail[j])
            {
                break;
            }

            tail[j] = iModified;
            current = tail[j];
        }

        set2.Add(tail[^1]);
    }
}

Console.WriteLine($"Number of unique visited cells for part 2: {set2.Count}");

(int X, int Y) GetFollowUp((int, int) first, (int, int) second)
{
    var deltaX = first.Item1 - second.Item1;
    var deltaY = first.Item2 - second.Item2;
    if (Math.Abs(deltaX) <= 1 && Math.Abs(deltaY) <= 1)
    {
        return second;
    }

    switch (deltaX)
    {
        case > 0:
            second.Item1++;
            break;
        case < 0:
            second.Item1--;
            break;
    }

    switch (deltaY)
    {
        case > 0:
            second.Item2++;
            break;
        case < 0:
            second.Item2--;
            break;
    }

    return second;
}
