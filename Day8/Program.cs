// See https://aka.ms/new-console-template for more information
const string FilePath = "input.txt";

var input = File.ReadAllLines(FilePath);
var columns = input.Length;
var rows = input[0].Length;

// Part 1
int[,][] localMax = new int[columns, rows][];

// Mark array [i, j, k, d]
// array[0] = max from left
// array[1] = max from top 
// array[2] = max from right
// array[3] = max from bot
for (var i = 0; i < columns; i++)
{
    for (var j = 0; j < rows; j++)
    {
        // Move forward and mark max values for left and top
        var leftIndex = j - 1;
        var topIndex = i - 1;
        var leftMax = leftIndex < 0 ? -1 : localMax[i, leftIndex][0];
        var topMax = topIndex < 0 ? -1 : localMax[topIndex, j][1];

        var value = input[i][j] - '0';
        localMax[i, j] = new int[4];
        localMax[i, j][0] = Math.Max(leftMax, value);
        localMax[i, j][1] = Math.Max(topMax, value);
    }
}

var invisiblePoints = 0;
for (var i = columns - 1; i >= 0; i--)
{
    for (var j = rows - 1; j >= 0; j--)
    {
        // Move backwards and mark max values for bottom and right 
        var rightIndex = j + 1;
        var botIndex = i + 1;
        var rightMax = rightIndex >= rows ? -1 : localMax[i, rightIndex][2];
        var botMax = botIndex >= columns ? -1 : localMax[botIndex, j][3];

        var value = input[i][j] - '0';
        localMax[i, j][2] = Math.Max(rightMax, value);
        localMax[i, j][3] = Math.Max(botMax, value);

        // Check visibility on backward movement
        if (i > 0 && i < columns - 1 && j > 0 && j < rows - 1)
        {
            if (value <= localMax[i - 1, j][1] &&
                value <= localMax[i, j - 1][0] &&
                value <= localMax[i + 1, j][3] &&
                value <= localMax[i, j + 1][2])
            {
                invisiblePoints++;
            }
        }
    }
}

var visiblePoints = rows * columns - invisiblePoints;
Console.WriteLine($"Number of visible points: {visiblePoints}");

// Part 2
int[,][] wallsIndexes = new int[columns, rows][];

// Wall array [i, j, k, d]
// array[0] = wall from left
// array[1] = wall from top 
// array[2] = wall from right
// array[3] = wall from bot
for (var i = 0; i < columns; i++)
{
    for (var j = 0; j < rows; j++)
    {
        // Move forward and find walls values for left and top
        var leftIndex = j - 1;
        var topIndex = i - 1;
        var value = input[i][j] - '0';
        wallsIndexes[i, j] = new int[4];
        if (leftIndex < 0)
        {
            wallsIndexes[i, j][0] = 0;
        }
        else
        {
            while (leftIndex > 0)
            {
                var leftValue = input[i][leftIndex] - '0';
                if (leftValue >= value)
                {
                    wallsIndexes[i, j][0] = leftIndex;
                    break;
                }
                else
                {
                    leftIndex = wallsIndexes[i, leftIndex][0];
                }
            }

            if (leftIndex == 0)
            {
                wallsIndexes[i, j][0] = 0;
            }
        }

        if (topIndex < 0)
        {
            wallsIndexes[i, j][1] = 0;
        }
        else
        {
            while (topIndex > 0)
            {
                var topValue = input[topIndex][j] - '0';
                if (topValue >= value)
                {
                    wallsIndexes[i, j][1] = topIndex;
                    break;
                }
                else
                {
                    topIndex = wallsIndexes[topIndex, j][1];
                }
            }

            if (topIndex == 0)
            {
                wallsIndexes[i, j][1] = 0;
            }
        }
    }
}

var maxScenic = 0;
for (var i = columns - 1; i >= 0; i--)
{
    for (var j = rows - 1; j >= 0; j--)
    {
        // Move forward and find walls values for left and top
        var rightIndex = j + 1;
        var botIndex = i + 1;
        var value = input[i][j] - '0';
        if (rightIndex >= rows)
        {
            wallsIndexes[i, j][2] = rows - 1;
        }
        else
        {
            while (rightIndex < rows - 1)
            {
                var rightValue = input[i][rightIndex] - '0';
                if (rightValue >= value)
                {
                    wallsIndexes[i, j][2] = rightIndex;
                    break;
                }
                else
                {
                    rightIndex = wallsIndexes[i, rightIndex][2];
                }
            }

            if (rightIndex == rows - 1)
            {
                wallsIndexes[i, j][2] = rows - 1;
            }
        }

        if (botIndex >= columns)
        {
            wallsIndexes[i, j][3] = columns - 1;
        }
        else
        {
            while (botIndex < columns - 1)
            {
                var botValue = input[botIndex][j] - '0';
                if (botValue >= value)
                {
                    wallsIndexes[i, j][3] = botIndex;
                    break;
                }
                else
                {
                    botIndex = wallsIndexes[botIndex, j][3];
                }
            }

            if (botIndex == rows - 1)
            {
                wallsIndexes[i, j][3] = rows - 1;
            }
        }
        
        // Computing scenic score
        var leftDistance = j - wallsIndexes[i, j][0];
        var rightDistance = wallsIndexes[i, j][2] - j;
        var topDistance = i - wallsIndexes[i, j][1];
        var botDistance = wallsIndexes[i, j][3] - i;
        var currentScenic = leftDistance * rightDistance * topDistance * botDistance;
        if (currentScenic > maxScenic)
        {
            maxScenic = currentScenic;
        }
    }
}

Console.WriteLine($"Max scenic score: {maxScenic}");