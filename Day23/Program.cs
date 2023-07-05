using System.Text;

const string inputPath = "input.txt";
const char elf = '#';
var input = File.ReadAllLines(inputPath);
var elves = new HashSet<Point>();
for (var i = 0; i < input.Length; i++)
{
    for (var j = 0; j < input[i].Length; j++)
    {
        if (input[i][j] == elf)
        {
            elves.Add(new Point(j, i));
        }
    }
}

var solution = new Solution(elves);
var partOneAnswer = solution.PartOne();
Console.WriteLine(partOneAnswer);

var partTwoAnswer = solution.PartTwo();
Console.WriteLine(partTwoAnswer);


internal class Solution
{
    private const int NumberOfRounds = 10;
    private readonly HashSet<Point> _elves;

    public Solution(HashSet<Point> elves)
    {
        _elves = elves;
    }

    private readonly Dictionary<int, List<Point>> _directions = new()
    {
        // North direction
        [0] = new List<Point>
        {
            new(0, -1),
            new(-1, -1),
            new(1, -1),
        },
        // South direction
        [1] = new List<Point>
        {
            new(0, 1),
            new(-1, 1),
            new(1, 1),
        },
        // West direction
        [2] = new List<Point>
        {
            new(-1, 0),
            new(-1, -1),
            new(-1, +1),
        },
        // East direction
        [3] = new List<Point>
        {
            new(1, 0),
            new(1, -1),
            new(1, 1),
        }
    };

    public int PartOne()
    {
        var currentElfPositions = new HashSet<Point>(_elves);
        var roundStartDirection = 0;
        for (var round = 0; round < NumberOfRounds; round++)
        {
            var elvesProposals = new Dictionary<Point, List<Point>>();
            foreach (var elf in currentElfPositions)
            {
                var currentDirection = roundStartDirection;
                var proposals = new List<Point>();
                foreach (var _ in _directions)
                {
                    var isAllAdjacentFree = _directions[currentDirection]
                        .All(v => !currentElfPositions.Contains(new Point(elf.X + v.X, elf.Y + v.Y)));
                    if (isAllAdjacentFree)
                    {
                        proposals.Add(new Point(elf.X + _directions[currentDirection][0].X,
                            elf.Y + _directions[currentDirection][0].Y));
                    }

                    currentDirection = (currentDirection + 1) % _directions.Count;
                }

                if (proposals.Count <= 0 || proposals.Count == _directions.Count)
                {
                    continue;
                }

                var elfProposal = proposals[0];
                if (elvesProposals.ContainsKey(elfProposal))
                {
                    elvesProposals[elfProposal].Add(elf);
                }
                else
                {
                    elvesProposals[elfProposal] = new List<Point> { elf };
                }
            }

            foreach (var (newPosition, elves) in elvesProposals)
            {
                if (elves.Count != 1) continue;
                currentElfPositions.Remove(elves[0]);
                currentElfPositions.Add(newPosition);
            }

            elvesProposals.Clear();
            roundStartDirection = (roundStartDirection + 1) % _directions.Count;
        }

        return CountScore(currentElfPositions);
    }

    public int PartTwo()
    {
        var currentElfPositions = new HashSet<Point>(_elves);
        var currentRound = 1;
        var roundStartDirection = 0;
        while (true)
        {
            var elvesProposals = new Dictionary<Point, List<Point>>();
            foreach (var elf in currentElfPositions)
            {
                var currentDirection = roundStartDirection;
                var proposals = new List<Point>();
                foreach (var _ in _directions)
                {
                    var isAllAdjacentFree = _directions[currentDirection]
                        .All(v => !currentElfPositions.Contains(new Point(elf.X + v.X, elf.Y + v.Y)));
                    if (isAllAdjacentFree)
                    {
                        proposals.Add(new Point(elf.X + _directions[currentDirection][0].X,
                            elf.Y + _directions[currentDirection][0].Y));
                    }

                    currentDirection = (currentDirection + 1) % _directions.Count;
                }

                if (proposals.Count <= 0 || proposals.Count == _directions.Count)
                {
                    continue;
                }

                var elfProposal = proposals[0];
                if (elvesProposals.ContainsKey(elfProposal))
                {
                    elvesProposals[elfProposal].Add(elf);
                }
                else
                {
                    elvesProposals[elfProposal] = new List<Point> { elf };
                }
            }

            var movedElves = 0;
            foreach (var (newPosition, elves) in elvesProposals)
            {
                if (elves.Count > 1)
                {
                    continue;
                }

                currentElfPositions.Remove(elves[0]);
                currentElfPositions.Add(newPosition);
                movedElves++;
            }

            if (movedElves == 0)
            {
                return currentRound;
            }

            currentRound++;
            elvesProposals.Clear();
            roundStartDirection = (roundStartDirection + 1) % _directions.Count;
        }
    }

    private static int CountScore(IReadOnlySet<Point> elves)
    {
        var score = 0;
        var minY = elves.Min(e => e.Y);
        var maxY = elves.Max(e => e.Y);
        for (var y = minY; y <= maxY; y++)
        {
            var minX = elves.Min(e => e.X);
            var maxX = elves.Max(e => e.X);
            for (var x = minX; x <= maxX; x++)
            {
                if (!elves.Contains(new Point(x, y)))
                {
                    score++;
                }
            }
        }

        return score;
    }

    private static void PrintMap(IReadOnlySet<Point> elves)
    {
        var sb = new StringBuilder();
        var minY = elves.Min(e => e.Y);
        var maxY = elves.Max(e => e.Y);
        for (var y = minY; y <= maxY; y++)
        {
            sb.Clear();
            var minX = elves.Min(e => e.X);
            var maxX = elves.Max(e => e.X);
            for (var x = minX; x <= maxX; x++)
            {
                sb.Append(elves.Contains(new Point(x, y)) ? '#' : '.');
            }

            Console.WriteLine(sb.ToString());
        }

        Console.WriteLine("----------------------");
    }
}


internal record struct Point(int X, int Y);