// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;

const string filePath = "input.txt";
const string pattern =
    @"^Valve (?<name>[A-Z]{2}) has flow rate=(?<rate>\d+); (tunnels lead to valves|tunnel leads to valve) (?<valves>[A-Z, ]*)$";

var input = File.ReadAllLines(filePath);
var regex = new Regex(pattern, RegexOptions.Compiled);
var valves = (from line in input
    select regex.Match(line)
    into match
    let valveName = match.Groups["name"].Value
    let tunnels = match.Groups["valves"].Value.Split(", ")
    let flowRate = int.Parse(match.Groups["rate"].Value)
    select new Valve(valveName, flowRate, tunnels)).ToList();

foreach (var valve in valves)
{
    Console.WriteLine(valve);
}

var graph = new ValveGraph(valves);
var pathVariants = graph.GetPressureReliefVariants();

Console.WriteLine($"Max release pressure : {pathVariants.Max(x => x.Value)}");

// Part 2
pathVariants = graph.GetPressureReliefVariants(26);

var pathCombinations = new List<HashSet<Valve>[]>();
GetCombinations(pathVariants.Keys.ToArray(), new HashSet<HashSet<Valve>>(), pathCombinations, 2, 0);

var maxPairResult =
    (from combination in pathCombinations
        let ownPath = combination[0]
        let elephantPath = combination[1]
        where !ownPath.Overlaps(elephantPath)
        select pathVariants[ownPath] + pathVariants[elephantPath]).Prepend(0).Max();

Console.WriteLine($"Max release pressure with elephant: {maxPairResult}");

void GetCombinations<T>(IReadOnlyList<T> collection, ISet<T> current, ICollection<T[]> result, int k,
    int index)
{
    if (current.Count >= k)
    {
        var item = current.ToArray();
        result.Add(item);

        return;
    }

    for (var i = index; i < collection.Count; i++)
    {
        current.Add(collection[i]);
        GetCombinations(collection, current, result, k, i + 1);
        current.Remove(collection[i]);
    }
}

internal class ValveGraph
{
    public ValveGraph(IEnumerable<Valve> data)
    {
        Data = data.ToDictionary(x => x.Name);
    }

    private Dictionary<string, Valve> Data { get; }
    private Dictionary<string, int> NameMap { get; } = new();
    private int[][]? _distanceMatrix;

    public Dictionary<HashSet<Valve>, int> GetPressureReliefVariants(int time = 30)
    {
        // Using items dependent set comparer to use sets as dictionary keys
        var result = new Dictionary<HashSet<Valve>, int>(HashSet<Valve>.CreateSetComparer());
        var startState = new TunnelState(time, 0, Data["AA"], new HashSet<Valve>());
        var queue = new Queue<TunnelState>();
        queue.Enqueue(startState);
        while (queue.TryDequeue(out var tunnelState))
        {
            foreach (var (nextValve, distance) in GetDistanceFromValve(tunnelState.LastOpenedValveVale))
            {
                if (tunnelState.OpenedValves.Contains(nextValve) || nextValve.FlowRate == 0)
                {
                    continue;
                }

                var updatedTimeLeft = tunnelState.TimeLeft - 1 - distance;
                if (updatedTimeLeft <= 0)
                {
                    continue;
                }

                var newTunnelState = new TunnelState(
                    updatedTimeLeft,
                    tunnelState.ReleasedPressure + nextValve.FlowRate * updatedTimeLeft,
                    nextValve,
                    new List<Valve>(tunnelState.OpenedValves) { nextValve });

                if (result.TryGetValue(newTunnelState.OpenedValves, out var previousMaxValue))
                {
                    if (previousMaxValue < newTunnelState.ReleasedPressure)
                    {
                        result[newTunnelState.OpenedValves] = newTunnelState.ReleasedPressure;
                    }
                }
                else
                {
                    result[newTunnelState.OpenedValves] = newTunnelState.ReleasedPressure;
                }

                queue.Enqueue(newTunnelState);
            }
        }

        return result;
    }

    private IEnumerable<(Valve Valve, int Distance)> GetDistanceFromValve(Valve current)
    {
        var distances = GetMinimumDistanceMatrix();
        var currentValveIndex = NameMap[current.Name];
        foreach (var valve in Data.Values)
        {
            var nextValveIndex = NameMap[valve.Name];
            if (currentValveIndex == nextValveIndex)
            {
                continue;
            }

            yield return (valve, distances[currentValveIndex][nextValveIndex]);
        }
    }

    private int[][] GetMinimumDistanceMatrix()
    {
        if (_distanceMatrix is not null)
        {
            return _distanceMatrix;
        }

        // Distance matrix creation
        _distanceMatrix = new int[Data.Count][];
        var count = 0;
        foreach (var value in Data.Values)
        {
            NameMap[value.Name] = count;
            _distanceMatrix[count] = new int[Data.Count];
            for (var k = 0; k < Data.Count; k++)
            {
                _distanceMatrix[count][k] = int.MaxValue;
            }

            count++;
        }

        // Distance matrix initialization
        foreach (var valve in Data.Values)
        {
            var index = NameMap[valve.Name];
            foreach (var tunnel in valve.Tunnels)
            {
                var indexOfConnectedValve = NameMap[tunnel];
                _distanceMatrix[index][indexOfConnectedValve] = 1;
                _distanceMatrix[indexOfConnectedValve][index] = 1;
                _distanceMatrix[index][index] = 0;
            }
        }

        // Floyd-Warshall's algorithm
        for (var k = 0; k < _distanceMatrix.Length; k++)
        {
            for (var i = 0; i < _distanceMatrix.Length; i++)
            {
                if (i == k)
                {
                    continue;
                }

                for (var j = 0; j < _distanceMatrix.Length; j++)
                {
                    if (i == j || j == k)
                    {
                        continue;
                    }

                    var newDistance = int.MaxValue;
                    if (_distanceMatrix[i][k] != int.MaxValue &&
                        _distanceMatrix[k][j] != int.MaxValue)
                    {
                        newDistance = _distanceMatrix[i][k] + _distanceMatrix[k][j];
                    }

                    if (_distanceMatrix[i][j] > newDistance)
                    {
                        _distanceMatrix[i][j] = newDistance;
                    }
                }
            }
        }

        return _distanceMatrix;
    }
}

internal class Valve
{
    public Valve(string name, int flowRate, IEnumerable<string> tunnels)
    {
        Name = name;
        FlowRate = flowRate;
        Tunnels = tunnels.ToArray();
    }

    public string Name { get; }
    public int FlowRate { get; }
    public string[] Tunnels { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not Valve second)
        {
            return false;
        }

        return Name == second.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public override string ToString()
    {
        var tunnels = string.Join(", ", Tunnels);

        return
            $"Valve {Name} has flow rate={FlowRate}; tunnel leads to valve{(Tunnels.Length > 1 ? "s" : "")} {tunnels}";
    }
}

internal class TunnelState
{
    public TunnelState(int timeLeft, int releasedPressure, Valve lastOpenedValve, IEnumerable<Valve> openedValves)
    {
        TimeLeft = timeLeft;
        ReleasedPressure = releasedPressure;
        LastOpenedValveVale = lastOpenedValve;
        OpenedValves = openedValves.ToHashSet();
    }

    public int TimeLeft { get; }
    public Valve LastOpenedValveVale { get; }
    public HashSet<Valve> OpenedValves { get; }
    public int ReleasedPressure { get; }
}