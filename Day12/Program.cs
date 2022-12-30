// See https://aka.ms/new-console-template for more information
const string filePath = "input.txt";

var input = File.ReadAllLines(filePath);

var map = input.Select(x => x.ToCharArray()).ToArray();
var steps = new Dictionary<(int, int), int>();
var xLength = map.Length;
var yLength = map[0].Length;

var startPoint = FindStart(map);
var endPoint = FindEnd(map);

(int, int)? FindStart(IReadOnlyList<char[]> board)
{
    for (var x = 0; x < xLength; x++)
    {
        for (var y= 0; y < yLength; y++)
        {
            if (board[x][y] != 'S')
            {
                continue;
            }
            
            board[x][y] = 'a';
            return (x, y);
        }
    }

    return null;
}

(int, int)? FindEnd(IReadOnlyList<char[]> board)
{
    for (var x = 0; x < xLength; x++)
    {
        for (var y= 0; y < yLength; y++)
        {
            if (board[x][y] != 'E')
            {
                continue;
            }
            
            board[x][y] = 'z';
            return (x, y);
        }
    }

    return null;
}

var depth = 0;
var queue = new Queue<(int, int)>();
queue.Enqueue(endPoint!.Value);
var indexes = new (int X, int Y)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
var isNearestAPointFound = false;
while (queue.Count > 0)
{
    var neighbours = queue.Count;
    for (var k = 0; k < neighbours; k++)
    {
        var (x, y) = queue.Dequeue();
        if (steps.ContainsKey((x, y)))
        {
            continue;
        }

        if ((x, y) == startPoint)
        {
            Console.WriteLine($"Fewest steps to E: {depth}");
        }

        if (!isNearestAPointFound && map[x][y] == 'a')
        {
            Console.WriteLine($"Fewest steps to nearest a point: {depth}");
            isNearestAPointFound = true;
        }

        foreach (var index in indexes)
        {
            var newX = x + index.X;
            var newY = y + index.Y;
            if (newX < 0 || newX >= xLength ||
                newY < 0 || newY >= yLength ||
                map[x][y] - map[newX][newY] > 1) 
            {
                continue;
            }
            
            queue.Enqueue((newX , newY));
        }

        steps[(x, y)] = depth;
    }

    depth++;
}