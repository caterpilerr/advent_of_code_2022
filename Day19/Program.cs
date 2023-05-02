// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text.RegularExpressions;

const string filePath = "input.txt";
const string pattern =
    @"^Blueprint (?<id>\d+): Each ore robot costs (?<oreRobotOre>\d+) ore\. " +
    @"Each clay robot costs (?<clayRobotOre>\d+) ore\. " +
    @"Each obsidian robot costs (?<obsidianRobotOre>\d+) ore and (?<obsidianRobotClay>\d+) clay\. " +
    @"Each geode robot costs (?<geodeRobotOre>\d+) ore and (?<geodeRobotObsidian>\d+) obsidian\.$";

var input = File.ReadAllLines(filePath);
var regex = new Regex(pattern, RegexOptions.Compiled);
var blueprints = (from line in input
    select regex.Match(line)
    into match
    let id = int.Parse(match.Groups["id"].Value)
    let oreRobot = new OreRobotRecipe(
        int.Parse(match.Groups["oreRobotOre"].Value))
    let clayRobot = new ClayRobotRecipe(
        int.Parse(match.Groups["clayRobotOre"].Value))
    let obsidianRobot = new ObsidianRobotRecipe(
        int.Parse(match.Groups["obsidianRobotOre"].Value),
        int.Parse(match.Groups["obsidianRobotClay"].Value))
    let geodeRobot = new GeodeRobotRecipe(
        int.Parse(match.Groups["geodeRobotOre"].Value),
        int.Parse(match.Groups["geodeRobotObsidian"].Value))
    select new Blueprint(id, oreRobot, clayRobot, obsidianRobot, geodeRobot)).ToList();

var factory = new Factory(blueprints, 24);
var timer = new Stopwatch();
timer.Start();
var qualityLevel = factory.GetQualityLevel();
timer.Stop();

Console.WriteLine($"Geodes quality level is {qualityLevel}");
Console.WriteLine($"Elapsed time execution is {timer.ElapsedMilliseconds} ms");

// Part 2
factory = new Factory(blueprints.Take(3), 32);
timer.Restart();
var geodesMultiplication = factory.GetGeodesMultiplication();
timer.Stop();

Console.WriteLine($"Geodes multiplication is {geodesMultiplication}");
Console.WriteLine($"Elapsed time execution is {timer.ElapsedMilliseconds} ms");

internal class Factory
{
    private readonly List<Blueprint> _blueprints;
    private readonly int _time;
    private readonly Resource[] _robotTypes = Enum.GetValues<Resource>();

    public Factory(IEnumerable<Blueprint> blueprints, int time)
    {
        _blueprints = blueprints.ToList();
        _time = time;
    }

    public int GetQualityLevel()
    {
        return _blueprints.Sum(x => x.Id * GetGeodesMaxNumber(x));
    }

    public int GetGeodesMultiplication()
    {
        return _blueprints.Aggregate(1, (mult, blueprint) => mult * GetGeodesMaxNumber(blueprint));
    }

    private int GetGeodesMaxNumber(Blueprint blueprint)
    {
        var queue = new Queue<FactoryState>();
        var hash = new HashSet<FactoryState>();

        var maxGeodes = 0;
        var startState = new FactoryState(blueprint, _time);
        queue.Enqueue(startState);
        while (queue.Count > 0)
        {
            var state = queue.Dequeue();
            if (hash.Contains(state))
            {
                continue;
            }

            hash.Add(state);
            var totalGeodes = state.Resources[Resource.Geode] + state.Robots[Resource.Geode] * state.TimeLeft;
            maxGeodes = Math.Max(maxGeodes, totalGeodes);

            // Trying to build new robots
            foreach (var robotType in _robotTypes)
            {
                var newState = state.Build(robotType);
                if (newState is not null)
                {
                    queue.Enqueue(newState);
                }
            }
        }

        return maxGeodes;
    }
}

internal class Blueprint
{
    public int Id { get; }
    public readonly Dictionary<Resource, Dictionary<Resource, int>> RobotRecipe;

    public Blueprint(int id,
        OreRobotRecipe oreRobotRecipe,
        ClayRobotRecipe clayRobotRecipe,
        ObsidianRobotRecipe obsidianRobotRecipe,
        GeodeRobotRecipe geodeRobotRecipe)
    {
        Id = id;
        RobotRecipe = new Dictionary<Resource, Dictionary<Resource, int>>
        {
            [Resource.Ore] = new() { { Resource.Ore, oreRobotRecipe.Ore } },
            [Resource.Clay] = new() { { Resource.Ore, clayRobotRecipe.Ore } },
            [Resource.Obsidian] = new()
                { { Resource.Ore, obsidianRobotRecipe.Ore }, { Resource.Clay, obsidianRobotRecipe.Clay } },
            [Resource.Geode] = new()
                { { Resource.Ore, geodeRobotRecipe.Ore }, { Resource.Obsidian, geodeRobotRecipe.Obsidian } }
        };
    }

    public override string ToString() =>
        $"Blueprint {Id}:\n" +
        $"  Each ore robot costs {RobotRecipe[Resource.Ore][Resource.Ore]} ore.\n" +
        $"  Each clay robot costs {RobotRecipe[Resource.Clay][Resource.Ore]} ore.\n" +
        $"  Each obsidian robot costs {RobotRecipe[Resource.Obsidian][Resource.Ore]} ore and {RobotRecipe[Resource.Obsidian][Resource.Clay]} clay.\n" +
        $"  Each geode robot costs {RobotRecipe[Resource.Geode][Resource.Ore]} ore and {RobotRecipe[Resource.Geode][Resource.Obsidian]} obsidian.\n";
}

internal enum Resource
{
    Ore,
    Clay,
    Obsidian,
    Geode
}

internal class FactoryState
{
    private readonly Blueprint _blueprint;

    public FactoryState(Blueprint blueprint, int timeLeft)
    {
        _blueprint = blueprint;
        TimeLeft = timeLeft;
        foreach (var recipe in blueprint.RobotRecipe.Values)
        {
            foreach (var (type, cost) in recipe)
            {
                if (_robotsLimit[type] < cost)
                {
                    _robotsLimit[type] = cost;
                }
            }
        }
    }

    private FactoryState(Blueprint blueprint,
        int timeLeft,
        Dictionary<Resource, int> robots,
        Dictionary<Resource, int> resources,
        Dictionary<Resource, int> robotsLimit)
    {
        _blueprint = blueprint;
        TimeLeft = timeLeft;
        Robots = robots;
        Resources = resources;
        _robotsLimit = robotsLimit;
    }

    public int TimeLeft { get; }

    private readonly Dictionary<Resource, int> _robotsLimit = new()
    {
        [Resource.Ore] = 0,
        [Resource.Clay] = 0,
        [Resource.Obsidian] = 0
    };

    public readonly Dictionary<Resource, int> Robots = new()
    {
        [Resource.Ore] = 1,
        [Resource.Clay] = 0,
        [Resource.Obsidian] = 0,
        [Resource.Geode] = 0
    };

    public readonly Dictionary<Resource, int> Resources = new()
    {
        [Resource.Ore] = 0,
        [Resource.Clay] = 0,
        [Resource.Obsidian] = 0,
        [Resource.Geode] = 0
    };

    public FactoryState? Build(Resource robotType)
    {
        if (_robotsLimit.ContainsKey(robotType) && Robots[robotType] >= _robotsLimit[robotType])
        {
            return null;
        }

        var timeToBuild = TimeToWaitForNewRobot(robotType);
        if (timeToBuild <= 0)
        {
            return null;
        }

        var newTime = TimeLeft - timeToBuild;
        if (newTime <= 0)
        {
            return null;
        }

        var newRobots = new Dictionary<Resource, int>(Robots);
        newRobots[robotType] += 1;

        var newResources = new Dictionary<Resource, int>(Resources);
        foreach (var type in Resources.Keys)
        {
            newResources[type] += Robots[type] * timeToBuild;
            if (_blueprint.RobotRecipe[robotType].ContainsKey(type))
            {
                newResources[type] -= _blueprint.RobotRecipe[robotType][type];
            }

            if (_robotsLimit.ContainsKey(type))
            {
                newResources[type] = Math.Min(newResources[type], _robotsLimit[type] * newTime);
            }
        }

        return new FactoryState(_blueprint, newTime, newRobots, newResources, _robotsLimit);
    }

    private int TimeToWaitForNewRobot(Resource robotType)
    {
        var maxTimeToWaitForResource = 0;
        foreach (var (type, amount) in _blueprint.RobotRecipe[robotType])
        {
            if (Robots[type] == 0)
            {
                return -1;
            }

            var resourceNeeded = amount - Resources[type];
            if (resourceNeeded <= 0)
                continue;

            var timeToWaitForResource = resourceNeeded / Robots[type] + (resourceNeeded % Robots[type] != 0 ? 1 : 0);
            maxTimeToWaitForResource = Math.Max(maxTimeToWaitForResource, timeToWaitForResource);
        }

        return maxTimeToWaitForResource + 1;
    }

    public override int GetHashCode()
    {
        var robotsHash =
            Robots.Aggregate(0, (acc, pair) => HashCode.Combine(acc, HashCode.Combine(pair.Key, pair.Value)));
        var resourcesHash =
            Resources.Aggregate(0, (acc, pair) => HashCode.Combine(acc, HashCode.Combine(pair.Key, pair.Value)));

        return HashCode.Combine(TimeLeft, robotsHash, resourcesHash);
    }

    public override bool Equals(object? obj)
    {
        if (obj is FactoryState other)
        {
            return TimeLeft == other.TimeLeft &&
                   Robots.Count == other.Robots.Count &&
                   Robots.All(item => other.Robots.ContainsKey(item.Key) && item.Value == other.Robots[item.Key]) &&
                   Resources.Count == other.Resources.Count &&
                   Resources.All(item =>
                       other.Resources.ContainsKey(item.Key) && item.Value == other.Resources[item.Key]);
        }

        return false;
    }
}

internal record struct OreRobotRecipe(int Ore);

internal record struct ClayRobotRecipe(int Ore);

internal record struct ObsidianRobotRecipe(int Ore, int Clay);

internal record struct GeodeRobotRecipe(int Ore, int Obsidian);