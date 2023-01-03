// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

var sensorsInput = new List<(int X, int Y)>();
var beaconsInput = new List<(int X, int Y)>();
var regex = new Regex(@"-?\d+");
foreach (var line in input)
{
    var values = regex.Matches(line);
    var sensorX = int.Parse(values[0].Value);
    var sensorY = int.Parse(values[1].Value);
    sensorsInput.Add((sensorX, sensorY));

    var beaconX = int.Parse(values[2].Value);
    var beaconY = int.Parse(values[3].Value);
    beaconsInput.Add((beaconX, beaconY));
}

const int yCheckCoordinate = 2000000;
var invalidPositions = FindInvalidPositions(beaconsInput, sensorsInput, null, null, yCheckCoordinate);
var invalidPositionsCount = invalidPositions.Count;

// 4827924
// Remove places occupied by beacons
var checkedPoints = new HashSet<(int, int)>();
foreach (var beacon in beaconsInput.Where(beacon => !checkedPoints.Contains(beacon)))
{
    if (beacon.Y == yCheckCoordinate && invalidPositions.InIntervals(beacon.X))
    {
        invalidPositionsCount--;
    }

    checkedPoints.Add(beacon);
}

Console.WriteLine($"Number of invalid positions for row {yCheckCoordinate}: {invalidPositionsCount}");

// Part 2
const int beaconXMaxCoordinate = 4000000;
const int beaconXMinCoordinate = 0;
const int beaconYMaxCoordinate = 4000000;
const int beaconYMinCoordinate = 0;
for (var y = beaconYMinCoordinate; y <= beaconYMaxCoordinate; y++)
{
    var interval = FindInvalidPositions(beaconsInput, sensorsInput, beaconXMinCoordinate, beaconXMaxCoordinate, y);
    var gaps = interval.GetGaps();
    if (gaps.Count > 0)
    {
        foreach (var gap in gaps)
        {
            Console.WriteLine($"Found gap in row {y}: {gap[0]} - {gap[1]}");
            for (var current = gap[0] + 1; current < gap[1]; current++)
            {
                Console.WriteLine($"Found valid distress beacon position: {current}, {y}");
                Console.WriteLine($"Tuning frequency is equal to {current * 4000000L + y}");
            }
        }
    }
}

Intervals FindInvalidPositions(IReadOnlyList<(int X, int Y)> beacons, IReadOnlyList<(int X, int Y)> sensors, int? xMin,
    int? xMax, int y)
{
    var intervals = new Intervals(xMin, xMax);
    for (var i = 0; i < sensors.Count; i++)
    {
        var sensor = sensors[i];
        var beacon = beacons[i];
        var range = Math.Abs(sensor.X - beacon.X) + Math.Abs(sensor.Y - beacon.Y);
        var xDelta = range - Math.Abs(sensor.Y - y);
        if (xDelta >= 0)
        {
            var intervalStart = sensor.X - xDelta;
            var intervalEnd = sensor.X + xDelta;
            intervals.Add(new[] { intervalStart, intervalEnd });

            // Console.WriteLine("Result:");
            // foreach (var g in intervals.Data)
            // {
            //     Console.Write($"{g[0]} - {g[1]} ,");
            // }
            //
            // Console.Write("\n");
        }
    }

    return intervals;
}

internal class Intervals
{
    private readonly List<int[]> _table = new();
    private readonly int? _xMin;
    private readonly int? _xMax;

    public List<int[]> Data => _table;

    public Intervals(int? xMin, int? xMax)
    {
        _xMax = xMax;
        _xMin = xMin;
    }

    public int Count => _table.Aggregate(0, (sum, x) => sum + x[1] - x[0] + 1);

    public List<int[]> GetGaps()
    {
        var gaps = new List<int[]>();
        if (_table.Count < 1)
        {
            return gaps;
        }

        if (_xMin is not null)
        {
            if (_xMin < _table[0][0])
            {
                gaps.Add(new[] { _xMin.Value, _table[0][0] });
            }
        }

        for (var i = 0; i < _table.Count - 1; i++)
        {
            if (_table[i + 1][0] - _table[i][1] > 1)
            {
                gaps.Add(new[] { _table[i][1], _table[i + 1][0] });
            }
        }

        if (_xMax is not null)
        {
            if (_xMax > _table[^1][1])
            {
                gaps.Add(new[] { _table[^1][1], _xMax.Value });
            }
        }

        return gaps;
    }

    public bool InIntervals(int x)
    {
        return _table.Any(interval => x >= interval[0] && x <= interval[1]);
    }

    public void Add(int[] interval)
    {
        if (_xMin is not null && interval[0] < _xMin)
        {
            interval[0] = _xMin.Value;
        }

        if (_xMax is not null && interval[1] > _xMax)
        {
            interval[1] = _xMax.Value;
        }

        var i = 0;
        while (i < _table.Count)
        {
            if (interval[0] < _table[i][0])
            {
                if (interval[1] < _table[i][0])
                {
                    _table.Insert(i, interval);
                    return;
                }

                _table[i][0] = interval[0];
                break;
            }

            if (interval[0] <= _table[i][1])
            {
                break;
            }

            i++;
        }

        if (i == _table.Count)
        {
            _table.Add(interval);
            return;
        }

        var j = _table.Count - 1;
        while (j >= 0)
        {
            if (interval[1] > _table[j][1])
            {
                _table[j][1] = interval[1];
                break;
            }

            if (interval[1] >= _table[j][0])
            {
                break;
            }

            j--;
        }

        var delta = j - i;
        if (delta > 0)
        {
            _table[i][1] = _table[j][1];
            _table.RemoveRange(i + 1, delta);
        }
    }
}