// See https://aka.ms/new-console-template for more information

const string inputPath = "input.txt";
var input = File.ReadAllLines(inputPath);
var cubes = input.Select(line =>
{
    var coordinates = line.Split(',')
        .Select(int.Parse)
        .ToArray();

    return new Point(coordinates[0], coordinates[1], coordinates[2]);
}).ToHashSet();

var minX = int.MaxValue;
var maxX = int.MinValue;
var minY = int.MaxValue;
var maxY = int.MinValue;
var minZ = int.MaxValue;
var maxZ = int.MinValue;
foreach (var cube in cubes)
{
    minX = Math.Min(minX, cube.X);
    maxX = Math.Max(maxX, cube.X);
    minY = Math.Min(minY, cube.Y);
    maxY = Math.Max(maxY, cube.Y);
    minZ = Math.Min(minZ, cube.Z);
    maxZ = Math.Max(maxZ, cube.Z);
}

var offsets = new Point[]
{
    new(1, 0, 0),
    new(-1, 0, 0),
    new(0, 1, 0),
    new(0, -1, 0),
    new(0, 0, 1),
    new(0, 0, -1)
};

var totalSurface = 0;
var exteriorSurface = 0;
foreach (var pointToCheck in from cube in cubes from offset in offsets select cube + offset)
{
    if (!cubes.Contains(pointToCheck))
    {
        totalSurface++;
    }

    if (CachedCanGetOut(pointToCheck))
    {
        exteriorSurface++;
    }
}

Console.WriteLine($"The surface of scanned lava droplet is equal to {totalSurface}");
Console.WriteLine($"The exterior surface of scanned lava droplet is equal to {exteriorSurface}");

bool CachedCanGetOut(Point point)
{
    var cache = new Dictionary<Point, bool>();
    if (cache.ContainsKey(point))
    {
        return cache[point];
    }

    var value = CanGetOut(point);
    cache[point] = value;

    return value;
}

bool CanGetOut(Point point)
{
    var queue = new Queue<Point>();
    queue.Enqueue(point);
    var seen = new HashSet<Point>();
    while (queue.Count > 0)
    {
        var currentPoint = queue.Dequeue();
        if (seen.Contains(currentPoint))
        {
            continue;
        }

        seen.Add(currentPoint);
        if (cubes.Contains(currentPoint))
        {
            continue;
        }

        if (currentPoint.X < minX ||
            currentPoint.X > maxX ||
            currentPoint.Y < minY ||
            currentPoint.Y > maxY ||
            currentPoint.Z < minZ ||
            currentPoint.Z > maxZ)
        {
            return true;
        }

        foreach (var offset in offsets)
        {
            queue.Enqueue(currentPoint + offset);
        }
    }

    return false;
}

internal record struct Point(int X, int Y, int Z)
{
    public static Point operator +(Point first, Point second) =>
        new(first.X + second.X,
            first.Y + second.Y,
            first.Z + second.Z);
}