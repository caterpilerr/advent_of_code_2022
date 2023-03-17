// See https://aka.ms/new-console-template for more information

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

const string filePath = "input.txt";
var input = File.ReadAllLines(filePath);
var commands = input.First().Select(command =>
{
    return command switch
    {
        '>' => Command.MoveRight,
        '<' => Command.MoveLeft,
        _ => throw new SwitchExpressionException()
    };
}).ToArray();

const int figureLimit = 5;
const long numberOfTurns = 1000000000000;

var board = new Board();
var figureId = 0;
var commandId = 0;
var addCycleHeight = 0L;
var signatures = new Dictionary<int, (long Turn, int Height)>();
for (var turnNumber = 1L; turnNumber <= numberOfTurns; turnNumber++)
{
    Block figure = figureId switch
    {
        0 => new HorizontalBlock(board),
        1 => new CrossBlock(board),
        2 => new LBlock(board),
        3 => new VerticalBlock(board),
        4 => new SquareBlock(board),
        _ => throw new InvalidEnumArgumentException()
    };

    while (!figure.IsFixed)
    {
        figure.Move(commands[commandId]);
        figure.Drop();
        commandId = (commandId + 1) % commands.Length;
    }

    board.NewFixed(figure);
    var boardSignature = board.GetSignature();
    if (addCycleHeight == 0 && boardSignature > 0)
    {
        var signature = HashCode.Combine(figureId, boardSignature);
        if (signatures.ContainsKey(signature))
        {
            var cycleHeight = board.TowerHeight - signatures[signature].Height;
            var cycleTurns = turnNumber - signatures[signature].Turn;
            var skippedCycles = (numberOfTurns - turnNumber) / cycleTurns;
            addCycleHeight = cycleHeight * skippedCycles;
            turnNumber += cycleTurns * skippedCycles;
        }
        else
        {
            signatures[signature] = (turnNumber, board.TowerHeight);
        }
    }

    figureId = (figureId + 1) % figureLimit;
}

Console.WriteLine($"Height of tower of rocks equal to {board.TowerHeight + addCycleHeight}");

internal enum Command
{
    MoveLeft,
    MoveRight
}

internal record struct Coordinate(int X, int Y);

internal abstract class Block
{
    private readonly Board _board;

    protected Block(Board board)
    {
        _board = board;
        Init();
    }

    public bool IsFixed { get; private set; }

    public abstract IEnumerable<Coordinate> Points { get; protected set; }

    public void Move(Command command)
    {
        var yDelta = command switch
        {
            Command.MoveRight => 1,
            Command.MoveLeft => -1,
            _ => 0
        };

        var newPoints = Points.Select(point => point with { Y = point.Y + yDelta }).ToArray();
        if (newPoints.Any(nextPointPosition =>
                _board[nextPointPosition.X, nextPointPosition.Y] == Board.FixedFigure ||
                _board[nextPointPosition.X, nextPointPosition.Y] == Board.Wall))
        {
            return;
        }

        MovePoints(newPoints);
    }

    public void Drop()
    {
        var newPoints = Points.Select(point => point with { X = point.X - 1 }).ToArray();
        if (newPoints.Any(nextPointPosition =>
                _board[nextPointPosition.X, nextPointPosition.Y] == Board.FixedFigure ||
                _board[nextPointPosition.X, nextPointPosition.Y] == Board.Floor))
        {
            Fix();
            return;
        }

        MovePoints(newPoints);
    }

    private void Init()
    {
        var startCoordinate = _board.SpawnCoordinate;
        Points = Points.Select(coordinate =>
            new Coordinate(coordinate.X + startCoordinate.X, coordinate.Y + startCoordinate.Y)).ToArray();
        Spawn();
    }

    private void Spawn()
    {
        foreach (var point in Points)
        {
            _board[point.X, point.Y] = Board.MovingFigure;
        }
    }

    private void Fix()
    {
        foreach (var point in Points)
        {
            _board[point.X, point.Y] = Board.FixedFigure;
        }

        IsFixed = true;
    }

    private void MovePoints(Coordinate[] newPoints)
    {
        foreach (var oldPoint in Points)
        {
            _board[oldPoint.X, oldPoint.Y] = Board.EmptySpace;
        }

        foreach (var newPoint in newPoints)
        {
            _board[newPoint.X, newPoint.Y] = Board.MovingFigure;
        }

        Points = newPoints;
    }
}

internal class HorizontalBlock : Block
{
    public HorizontalBlock(Board board) : base(board)
    {
    }

    public override IEnumerable<Coordinate> Points { get; protected set; } = new Coordinate[]
    {
        new(0, 0),
        new(0, 1),
        new(0, 2),
        new(0, 3)
    };
}

internal class CrossBlock : Block
{
    public CrossBlock(Board board) : base(board)
    {
    }

    public override IEnumerable<Coordinate> Points { get; protected set; } = new Coordinate[]
    {
        new(0, 1),
        new(1, 0),
        new(1, 1),
        new(1, 2),
        new(2, 1)
    };
}

internal class LBlock : Block
{
    public LBlock(Board board) : base(board)
    {
    }

    public override IEnumerable<Coordinate> Points { get; protected set; } = new Coordinate[]
    {
        new(0, 0),
        new(0, 1),
        new(0, 2),
        new(1, 2),
        new(2, 2)
    };
}

internal class VerticalBlock : Block
{
    public VerticalBlock(Board board) : base(board)
    {
    }

    public override IEnumerable<Coordinate> Points { get; protected set; } = new Coordinate[]
    {
        new(0, 0),
        new(1, 0),
        new(2, 0),
        new(3, 0)
    };
}

internal class SquareBlock : Block
{
    public SquareBlock(Board board) : base(board)
    {
    }

    public override IEnumerable<Coordinate> Points { get; protected set; } = new Coordinate[]
    {
        new(0, 0),
        new(0, 1),
        new(1, 0),
        new(1, 1)
    };
}

internal class Board
{
    public const char FixedFigure = '#';
    public const char MovingFigure = '@';
    public const char Wall = '|';
    public const char Floor = '_';
    public const char EmptySpace = '.';

    private const int BoardWidth = 9;
    private const int SpawnLeftCoordinate = 3;
    private const int SpawnHeightOffset = 4;
    private const int BoardHeightLimit = 1000000;
    private readonly char[][] _board;

    public Board()
    {
        _board = new char[BoardHeightLimit][];
        _board[0] = Enumerable.Repeat(Floor, BoardWidth).ToArray();
        for (var row = 1; row < _board.Length; row++)
        {
            _board[row] = Enumerable.Repeat(EmptySpace, BoardWidth).ToArray();
            _board[row][0] = Wall;
            _board[row][BoardWidth - 1] = Wall;
        }
    }

    public int TowerHeight { get; private set; }

    public Coordinate SpawnCoordinate => new(TowerHeight + SpawnHeightOffset, SpawnLeftCoordinate);

    public void NewFixed(Block block)
    {
        foreach (var point in block.Points)
        {
            TowerHeight = Math.Max(TowerHeight, point.X);
        }
    }

    public char this[int i, int j]
    {
        get => _board[i][j];
        set => _board[i][j] = value;
    }

    public int GetSignature()
    {
        const int signatureLength = 50;
        var signature = 0;
        if (TowerHeight < signatureLength)
        {
            return signature;
        }

        for (var row = 0; row < signatureLength; row++)
        {
            for (var j = 1; j < BoardWidth - 1; j++)
            {
                if (_board[TowerHeight - row][j] != FixedFigure)
                {
                    signature = HashCode.Combine(signature, (row, j).GetHashCode());
                }
            }
        }

        return signature;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("---------");
        for (var i = TowerHeight; i >= 0; i--)
        {
            sb.Append(string.Join(string.Empty, _board[i]));
            sb.Append('\n');
        }

        sb.Append("---------");
        return sb.ToString();
    }
}