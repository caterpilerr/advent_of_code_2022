const string inputPath = "input.txt";
var input = File.ReadAllLines(inputPath);

var solution = new Solution(input);
Console.WriteLine($"Part one answer: {solution.PartOne()}");
Console.WriteLine($"Part two answer: {solution.PartTwo()}");

internal class Solution
{
    private readonly int _mapHeight;
    private readonly int _mapWidth;
    private readonly Point _start;
    private readonly Point _finish;
    private readonly Dictionary<Point, char> _startBlizzards = new();
    private readonly Dictionary<int, HashSet<Point>> _blizzardsAtTimeCache = new();

    private readonly Dictionary<char, Point> _directions = new()
    {
        ['>'] = new Point(0, 1),
        ['<'] = new Point(0, -1),
        ['v'] = new Point(1, 0),
        ['^'] = new Point(-1, 0)
    };

    public Solution(IReadOnlyList<string> input)
    {
        for (var row = 1; row < input.Count - 1; row++)
        {
            for (var col = 1; col < input[0].Length - 1; col++)
            {
                if (input[row][col] != '.')
                {
                    _startBlizzards[new Point(row - 1, col - 1)] = input[row][col];
                }
            }
        }

        _mapHeight = input.Count - 2;
        _mapWidth = input[0].Length - 2;
        var startY = input[0].IndexOf('.') - 1;
        _start = new Point(-1, startY);
        var endY = input[^1].IndexOf('.') - 1;
        _finish = new Point(_mapHeight, endY);
    }

    public int PartOne()
    {
        return Route(0, _start, _finish);
    }

    public int PartTwo()
    {
        var firstTime = Route(0, _start, _finish);
        var secondTime = Route(firstTime, _finish, _start);
        var thirdTime = Route(secondTime, _start, _finish);

        return thirdTime;
    }

    private int Route(int timeStart, Point start, Point end)
    {
        var seen = new HashSet<(int, Point)>();
        var queue = new Queue<(int, Point)>();
        queue.Enqueue((timeStart, start));
        while (true)
        {
            var (time, location) = queue.Dequeue();
            if (seen.Contains((time, location)))
                continue;

            seen.Add((time, location));
            var blizzards = GetBlizzards(time + 1);
            if (!blizzards.Contains(location))
                queue.Enqueue((time + 1, location));

            foreach (var direction in _directions.Values)
            {
                var newLocation = new Point(location.X + direction.X, location.Y + direction.Y);
                if (newLocation == end)
                    return time + 1;

                if (newLocation.X < 0 ||
                    newLocation.X >= _mapHeight ||
                    newLocation.Y < 0 ||
                    newLocation.Y >= _mapWidth)
                    continue;

                if (!blizzards.Contains(newLocation))
                    queue.Enqueue((time + 1, newLocation));
            }
        }
    }

    private HashSet<Point> GetBlizzards(int time)
    {
        if (_blizzardsAtTimeCache.ContainsKey(time))
            return _blizzardsAtTimeCache[time];

        var newMap = new HashSet<Point>();
        foreach (var (location, symbol) in _startBlizzards)
        {
            var offset = _directions[symbol];
            var newX = (location.X + offset.X * time) % _mapHeight;
            var newY = (location.Y + offset.Y * time) % _mapWidth;
            var newCoordinate = new Point(newX >= 0 ? newX : newX + _mapHeight,
                newY >= 0 ? newY : newY + _mapWidth);
            newMap.Add(newCoordinate);
        }

        _blizzardsAtTimeCache[time] = newMap;
        return newMap;
    }
}

internal record struct Point(int X, int Y);