// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

const string inputPath = "input.txt";
const string inputPattern = @"(?<map>\D+)\n\n(?<command>\w+)";
var input = File.ReadAllText(inputPath);
var match = Regex.Match(input, inputPattern);
var commands = match.Groups["command"].Value;
var rawMap = match.Groups["map"].Value.Split("\n");

var partOne = PartOne.Solve(rawMap, commands);

Console.WriteLine($"Part one answer: {partOne}");

const int faceLength = 50;
const int maxEdgeIndex = faceLength - 1;
// Location of the cube's faces in the input file.
var inputOffsets = new (int RowOffset, int ColumnOffset)[]
{
    (0, 1),
    (0, 2),
    (1, 1),
    (2, 0),
    (2, 1),
    (3, 0)
};

// Wrap rules for each face of the cube
var rules = new[]
{
    // Face 0
    new WrapRule[]
    {
        new((old, _) => new Coordinate2D(old.Y, 0), Direction.Right, 5),
        new((old, _) => new Coordinate2D(old.X, 0), Direction.Right, 1),
        new((old, _) => new Coordinate2D(0, old.Y), Direction.Down, 2),
        new((old, maxIndex) => new Coordinate2D(maxIndex - old.X, 0), Direction.Right, 3)
    },
    // Face 1
    new WrapRule[]
    {
        new((old, maxIndex) => new Coordinate2D(maxIndex, old.Y), Direction.Up, 5),
        new((old, maxIndex) => new Coordinate2D(maxIndex - old.X, maxIndex), Direction.Left, 4),
        new((old, maxIndex) => new Coordinate2D(old.Y, maxIndex), Direction.Left, 2),
        new((old, maxIndex) => new Coordinate2D(old.X, maxIndex), Direction.Left, 0)
    },
    // Face 2
    new WrapRule[]
    {
        new((old, maxIndex) => new Coordinate2D(maxIndex, old.Y), Direction.Up, 0),
        new((old, maxIndex) => new Coordinate2D(maxIndex, old.X), Direction.Up, 1),
        new((old, _) => new Coordinate2D(0, old.Y), Direction.Down, 4),
        new((old, _) => new Coordinate2D(0, old.X), Direction.Down, 3)
    },
    // Face 3
    new WrapRule[]
    {
        new((old, _) => new Coordinate2D(old.Y, 0), Direction.Right, 2),
        new((old, _) => new Coordinate2D(old.X, 0), Direction.Right, 4),
        new((old, _) => new Coordinate2D(0, old.Y), Direction.Down, 5),
        new((old, maxIndex) => new Coordinate2D(maxIndex - old.X, 0), Direction.Right, 0)
    },
    // Face 4
    new WrapRule[]
    {
        new((old, maxIndex) => new Coordinate2D(maxIndex, old.Y), Direction.Up, 2),
        new((old, maxIndex) => new Coordinate2D(maxIndex - old.X, maxIndex), Direction.Left, 1),
        new((old, maxIndex) => new Coordinate2D(old.Y, maxIndex), Direction.Left, 5),
        new((old, maxIndex) => new Coordinate2D(old.X, maxIndex), Direction.Left, 3)
    },
    // Face 5
    new WrapRule[]
    {
        new((old, maxIndex) => new Coordinate2D(maxIndex, old.Y), Direction.Up, 3),
        new((old, maxIndex) => new Coordinate2D(maxIndex, old.X), Direction.Up, 4),
        new((old, _) => new Coordinate2D(0, old.Y), Direction.Down, 1),
        new((old, _) => new Coordinate2D(0, old.X), Direction.Down, 0)
    }
};

var coordinateWrapper = new Wrapper(rules, maxEdgeIndex);
var gameMap = new GameMap(rawMap, faceLength, inputOffsets);
var partTwo = new PartTwo(coordinateWrapper, gameMap).Solve(commands);

Console.WriteLine($"Part two answer: {partTwo}");
Console.WriteLine(gameMap);

internal static class PartOne
{
    private const char WallCell = '#';
    private const char EmptyCell = '.';
    private const char VoidCell = ' ';

    public static int Solve(string[] rawMap, string command)
    {
        var maxLength = rawMap.Max(line => line.Length);
        var gameMap = rawMap.Select(line =>
        {
            line = line.TrimEnd(Environment.NewLine.ToCharArray());
            var difference = maxLength - line.Length;
            return difference > 0 ? (line + new string(' ', difference)).ToCharArray() : line.ToCharArray();
        }).ToArray();

        var currentCoordinate = FindStart(gameMap);
        var direction = (X: 0, Y: 1);
        var index = 0;
        var sb = new StringBuilder();
        while (index < command.Length)
        {
            var symbol = command[index];
            if (char.IsAsciiLetter(symbol))
            {
                var steps = int.Parse(sb.ToString());
                sb.Clear();
                currentCoordinate = Move(steps, gameMap, currentCoordinate, direction);
                direction = symbol switch
                {
                    'R' => (direction.Y, -direction.X),
                    'L' => (-direction.Y, direction.X),
                    _ => throw new InvalidOperationException()
                };
            }
            else
            {
                sb.Append(command[index]);
            }

            index++;
        }

        var facingScore = new Dictionary<(int, int), int>
        {
            [(0, 1)] = 0,
            [(1, 0)] = 1,
            [(0, -1)] = 2,
            [(-1, 0)] = 3
        };

        currentCoordinate = Move(int.Parse(sb.ToString()), gameMap, currentCoordinate, direction);

        return 1000 * (currentCoordinate.X + 1) + 4 * (currentCoordinate.Y + 1) + facingScore[direction];
    }

    private static (int X, int Y) FindStart(IReadOnlyList<char[]> map)
    {
        for (var row = 0; row < map.Count; row++)
        {
            for (var col = 0; col < map[0].Length; col++)
            {
                if (map[row][col] != EmptyCell)
                {
                    continue;
                }

                return (row, col);
            }
        }

        throw new InvalidOperationException();
    }

    private static (int X, int Y) Move(int steps, IReadOnlyList<char[]> map, (int X, int Y) position,
        (int X, int Y) vector)
    {
        var mapHeight = map.Count;
        var mapWidth = map[0].Length;
        var nextPosition = (X: (mapHeight + position.X + vector.X) % mapHeight,
            Y: (mapWidth + position.Y + vector.Y) % mapWidth);
        while (steps > 0)
        {
            if (map[nextPosition.X][nextPosition.Y] == WallCell)
            {
                return position;
            }

            if (map[nextPosition.X][nextPosition.Y] != VoidCell)
            {
                position = nextPosition;
                map[nextPosition.X][nextPosition.Y] = '@';
                steps--;
            }

            nextPosition = (X: (mapHeight + nextPosition.X + vector.X) % mapHeight,
                Y: (mapWidth + nextPosition.Y + vector.Y) % mapWidth);
        }

        return position;
    }
}

internal class PartTwo
{
    private readonly Wrapper _wrapper;
    private readonly GameMap _map;
    private CubeCoordinate _currentCoordinate;

    private readonly Dictionary<Direction, Coordinate2D> _moveVectors = new()
    {
        [Direction.Up] = new Coordinate2D(-1, 0),
        [Direction.Right] = new Coordinate2D(0, 1),
        [Direction.Down] = new Coordinate2D(1, 0),
        [Direction.Left] = new Coordinate2D(0, -1)
    };

    private readonly Dictionary<Direction, Direction> _rightRotation = new()
    {
        [Direction.Up] = Direction.Right,
        [Direction.Right] = Direction.Down,
        [Direction.Down] = Direction.Left,
        [Direction.Left] = Direction.Up
    };

    private readonly Dictionary<Direction, Direction> _leftRotation = new()
    {
        [Direction.Up] = Direction.Left,
        [Direction.Right] = Direction.Up,
        [Direction.Down] = Direction.Right,
        [Direction.Left] = Direction.Down
    };

    private readonly Dictionary<Direction, int> _facingScore = new()
    {
        [Direction.Right] = 0,
        [Direction.Down] = 1,
        [Direction.Left] = 2,
        [Direction.Up] = 3
    };

    public PartTwo(Wrapper wrapper, GameMap map)
    {
        _wrapper = wrapper;
        _map = map;
        _currentCoordinate = _map.GetStart();
    }

    private void Move(int steps)
    {
        while (steps > 0)
        {
            var nextCoordinate = _currentCoordinate;
            nextCoordinate.Coordinate2D += _moveVectors[nextCoordinate.Direction];
            nextCoordinate = _wrapper.Transform(nextCoordinate);
            if (_map.Get(nextCoordinate) == GameMap.WallCell)
            {
                return;
            }

            _map.Set(nextCoordinate, GameMap.VisitedCell);
            _currentCoordinate = nextCoordinate;
            steps--;
        }
    }

    public int Solve(string command)
    {
        var index = 0;
        var sb = new StringBuilder();
        while (index < command.Length)
        {
            var symbol = command[index];
            if (char.IsAsciiLetter(symbol))
            {
                var steps = int.Parse(sb.ToString());
                sb.Clear();
                Move(steps);
                _currentCoordinate.Direction = symbol switch
                {
                    'R' => _rightRotation[_currentCoordinate.Direction],
                    'L' => _leftRotation[_currentCoordinate.Direction],
                    _ => throw new InvalidOperationException()
                };
            }
            else
            {
                sb.Append(command[index]);
            }

            index++;
        }

        Move(int.Parse(sb.ToString()));

        var rawCoordinate = _map.GetRawCoordinate(_currentCoordinate);

        return 1000 * (rawCoordinate.X + 1) + 4 * (rawCoordinate.Y + 1) + _facingScore[_currentCoordinate.Direction];
    }
}

internal class GameMap
{
    public const char WallCell = '#';
    public const char VisitedCell = '@';
    private readonly char[][][] _cube;
    private readonly char[][] _rawMap;
    private readonly int _faceLength;
    private readonly (int RowOffset, int ColumnOffset)[] _inputOffsets;

    public Coordinate2D GetRawCoordinate(CubeCoordinate coordinate)
    {
        var offsets = _inputOffsets[coordinate.Face];
        var x = coordinate.Coordinate2D.X;
        var y = coordinate.Coordinate2D.Y;

        var rawColumnOffset = offsets.ColumnOffset * _faceLength + y;
        var rawRowOffset = offsets.RowOffset * _faceLength + x;

        return new Coordinate2D(rawRowOffset, rawColumnOffset);
    }

    public GameMap(IEnumerable<string> rawMap, int faceLength, (int, int)[] inputOffsets)
    {
        _faceLength = faceLength;
        _rawMap = rawMap.Select(line => line.ToCharArray()).ToArray();
        _inputOffsets = inputOffsets;
        var cube = new char[6][][];
        for (var i = 0; i < _inputOffsets.Length; i++)
        {
            cube[i] = FaceParse(_rawMap, _faceLength, _inputOffsets[i].RowOffset, _inputOffsets[i].ColumnOffset);
        }

        _cube = cube;
    }

    private static char[][] FaceParse(IReadOnlyList<char[]> rawMap, int faceLength, int rowBlock, int columnBlock)
    {
        var face = new char[faceLength][];
        var startColumn = columnBlock * faceLength;
        var startRow = rowBlock * faceLength;
        for (var row = startRow; row < startRow + faceLength; row++)
        {
            face[row - startRow] = new char[faceLength];
            for (var col = startColumn; col < startColumn + faceLength; col++)
            {
                face[row - startRow][col - startColumn] = rawMap[row][col];
            }
        }

        return face;
    }

    public char Get(CubeCoordinate coordinate)
    {
        var face = coordinate.Face;
        var x = coordinate.Coordinate2D.X;
        var y = coordinate.Coordinate2D.Y;

        return _cube[face][x][y];
    }

    public void Set(CubeCoordinate coordinate, char value)
    {
        var face = coordinate.Face;
        var x = coordinate.Coordinate2D.X;
        var y = coordinate.Coordinate2D.Y;
        _cube[face][x][y] = value;

        var rawCoordinate = GetRawCoordinate(coordinate);
        _rawMap[rawCoordinate.X][rawCoordinate.Y] = value;
    }

    public CubeCoordinate GetStart()
    {
        var face = _cube[0];
        for (var x = 0; x < face.Length; x++)
        {
            for (var y = 0; y < face[0].Length; y++)
            {
                if (face[x][y] == WallCell)
                {
                    continue;
                }

                return new CubeCoordinate(0, new Coordinate2D(x, y), Direction.Right);
            }
        }

        throw new InvalidOperationException();
    }

    public override string ToString()
    {
        return string.Join('\n', _rawMap.Select(line => new string(line)));
    }
}

internal struct Coordinate2D
{
    public Coordinate2D(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }

    public static Coordinate2D operator +(Coordinate2D a, Coordinate2D b)
    {
        return new Coordinate2D(a.X + b.X, a.Y + b.Y);
    }
}

internal enum Direction
{
    Up,
    Left,
    Down,
    Right
}

internal struct CubeCoordinate
{
    public CubeCoordinate(int face, Coordinate2D coordinate2D, Direction direction)
    {
        Face = face;
        Coordinate2D = coordinate2D;
        Direction = direction;
    }

    public int Face { get; }
    public Coordinate2D Coordinate2D { get; set; }
    public Direction Direction { get; set; }
}

internal class WrapRule
{
    public WrapRule(Func<Coordinate2D, int, Coordinate2D> coordinateTransformation,
        Direction directionTransformation, int face)
    {
        CoordinateTransformation = coordinateTransformation;
        DirectionTransformation = directionTransformation;
        Face = face;
    }

    public int Face { get; }
    public Func<Coordinate2D, int, Coordinate2D> CoordinateTransformation { get; }
    public Direction DirectionTransformation { get; }
}

internal class Wrapper
{
    private readonly WrapRule[][] _rules;
    private readonly int _maxIndex;

    public Wrapper(WrapRule[][] rules, int maxIndex)
    {
        _rules = rules;
        _maxIndex = maxIndex;
    }

    public CubeCoordinate Transform(CubeCoordinate coordinate)
    {
        var faceRules = _rules[coordinate.Face];
        WrapRule rule;
        if (coordinate.Coordinate2D.X < 0 && coordinate.Direction == Direction.Up)
        {
            rule = faceRules[0];
        }
        else if (coordinate.Coordinate2D.Y > _maxIndex && coordinate.Direction == Direction.Right)
        {
            rule = faceRules[1];
        }
        else if (coordinate.Coordinate2D.X > _maxIndex && coordinate.Direction == Direction.Down)
        {
            rule = faceRules[2];
        }
        else if (coordinate.Coordinate2D.Y < 0 && coordinate.Direction == Direction.Left)
        {
            rule = faceRules[3];
        }
        else
        {
            return coordinate;
        }

        return new CubeCoordinate(rule.Face,
            rule.CoordinateTransformation(coordinate.Coordinate2D, _maxIndex),
            rule.DirectionTransformation);
    }
}