// See https://aka.ms/new-console-template for more information
using System.Text;

const int xSandSource = 500;
const int ySandSource = 0;
const string filePath = "input.txt";
const char emptyBlock = '.';
const char wallBlock = '#';
const char sandBlock = 'o';

var input = File.ReadAllLines(filePath);
var maxX = 0;
var maxY = 0;
var minX = int.MaxValue;
var minY = int.MaxValue;
var wallsData = new List<int[][]>();
foreach (var line in input)
{
    var wallCorners = line
        .Split("->")
        .Select(x => x.Split(',')
            .Select(int.Parse).ToArray())
        .ToArray();

    var localMaxX = wallCorners.Max(x => x[0]);
    var localMaxY = wallCorners.Max(x => x[1]);
    maxX = localMaxX > maxX ? localMaxX : maxX;
    maxY = localMaxY > maxY ? localMaxY : maxY;

    var localMinX = wallCorners.Min(x => x[0]);
    var localMinY = wallCorners.Min(x => x[1]);
    minX = localMinX < minX ? localMinX : minX;
    minY = localMinY < minY ? localMinY : minY;

    wallsData.Add(wallCorners);
}

Console.WriteLine($"Size of board X: {minX} - {maxX}, Y: {minY} - {maxY}");

var gameBoard = new char[maxX + 1000, maxY + 10];
InitBoard(gameBoard, wallsData);

// Part 1
var sandsCount = 0;
while (DropSand(gameBoard, xSandSource, ySandSource))
{
    sandsCount++;
}

DisplayBoard(gameBoard, minX - 1, maxX + 1, 0, maxY);
Console.WriteLine($"Number of accumulated sand grains: {sandsCount}");

// Part 2
AddFloor(gameBoard, maxY + 2);
while (DropSand(gameBoard, xSandSource, ySandSource))
{
    sandsCount++;
}

DisplayBoard(gameBoard, minX - 1, maxX + 9, 0, maxY + 2);
Console.WriteLine($"Number of accumulated sand grains with floor: {sandsCount}");

void AddFloor(char[,] board, int floorLevel)
{
    for (var x = 0; x < board.GetLength(0); x++)
    {
        board[x, floorLevel] = wallBlock;
    }
}

bool DropSand(char[,] board, int x, int y)
{
    if (board[x, y] != emptyBlock)
    {
        return false;
    }

    var currentX = x;
    var currentY = y;
    var offsets = new (int X, int Y)[] { (0, 1), (-1, 1), (1, 1) };
    var isNotRest = true;
    while (isNotRest)
    {
        foreach (var offset in offsets)
        {
            var nextX = currentX + offset.X;
            var nextY = currentY + offset.Y;
            if (nextX < 0 || nextX >= board.GetLength(0))
            {
                throw new Exception("Board is too short");
            }

            if (nextY < 0 || nextY >= board.GetLength(1))
            {
                return false;
            }

            if (board[nextX, nextY] == emptyBlock)
            {
                currentX = nextX;
                currentY = nextY;
                isNotRest = true;
                break;
            }

            isNotRest = false;
        }
    }

    board[currentX, currentY] = sandBlock;
    return true;
}

void InitBoard(char[,] board, IEnumerable<int[][]> wallsInfo)
{
    for (var i = 0; i < board.GetLength(0); i++)
    {
        for (var j = 0; j < board.GetLength(1); j++)
        {
            board[i, j] = emptyBlock;
        }
    }

    foreach (var data in wallsInfo)
    {
        for (var i = 0; i < data.Length - 1; i++)
        {
            var currentX = data[i][0];
            var currentY = data[i][1];
            board[currentX, currentY] = wallBlock;

            var xDelta = currentX - data[i + 1][0];
            var yDelta = currentY - data[i + 1][1];

            while (xDelta != 0 || yDelta != 0)
            {
                switch (xDelta)
                {
                    case > 0:
                        currentX--;
                        xDelta--;
                        break;
                    case < 0:
                        currentX++;
                        xDelta++;
                        break;
                }

                switch (yDelta)
                {
                    case > 0:
                        currentY--;
                        yDelta--;
                        break;
                    case < 0:
                        currentY++;
                        yDelta++;
                        break;
                }

                board[currentX, currentY] = wallBlock;
            }
        }
    }
}

void DisplayBoard(char[,] board, int minX, int maxX, int minY, int maxY)
{
    var xDisplayLength = maxX - minX + 1;
    var minXCharNumber = minX.ToString().Length;
    Console.WriteLine($"{{0, -3}}   {{1, -{minXCharNumber}}}{{2, {xDisplayLength - minXCharNumber}}}", string.Empty, minX, maxX);
    
    for (var y = minY; y <= maxY; y++)
    {
        var row = new StringBuilder();
        for (var x = minX; x <= maxX; x++)
        {
            row.Append(board[x, y]);
        }

        Console.WriteLine($"{{0, -3}} | {{1, -{xDisplayLength} }}", y, row);
    }
}