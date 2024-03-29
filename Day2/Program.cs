﻿// See https://aka.ms/new-console-template for more information
const string filePath = "input.txt";

var input = File.ReadAllLines(filePath);
var mapScore = new Dictionary<char, int>
{
    ['A'] = 1,
    ['B'] = 2,
    ['C'] = 3,
    ['W'] = 6,
    ['D'] = 3,
    ['L'] = 0,
};

var shapeToWin = new Dictionary<char, char>
{
    ['A'] = 'B',
    ['B'] = 'C',
    ['C'] = 'A',
};

var shapeToLose = new Dictionary<char, char>
{
    ['A'] = 'C',
    ['B'] = 'A',
    ['C'] = 'B',
};

var totalScorePart1 = 0;
var totalScorePart2 = 0;
foreach (var game in input)
{
    var data = game.Split(' ');
    var opponent = data[0][0];
    var player = data[1][0];
    totalScorePart1 += CountScorePart1(opponent, player, mapScore);
    totalScorePart2 += CountScorePart2(opponent, player, mapScore, shapeToWin, shapeToLose);
}

Console.WriteLine($"Total score according to first strategy: {totalScorePart1}");
Console.WriteLine($"Total score according to second strategy: {totalScorePart2}");

int CountScorePart1(char opponent, char player, Dictionary<char, int> scoreMap)
{
    // X -> A, Y -> B, Z -> C
    player = (char)(player - 23);
    var score = scoreMap[player];
    if (opponent == player)
    {
        score += scoreMap['D'];
    }
    else if
    (
        player == 'A' && opponent == 'C' ||
        player == 'B' && opponent == 'A' ||
        player == 'C' && opponent == 'B'
    )
    {
        score += scoreMap['W'];
    }
    else
    {
        score += scoreMap['L'];
    }

    return score;
}

int CountScorePart2(
    char opponent,
    char player,
    IReadOnlyDictionary<char, int> scoreMap,
    IReadOnlyDictionary<char, char> shapeToWinMap,
    IReadOnlyDictionary<char, char> shapeToLoseMap)
{
    var score = 0;
    char playerShape;
    switch (player)
    {
        case 'X':
            playerShape = shapeToLoseMap[opponent];
            score += scoreMap['L'];
            break;
        case 'Y':
            playerShape = opponent;
            score += scoreMap['D'];
            break;
        case 'Z':
            playerShape = shapeToWinMap[opponent];
            score += scoreMap['W'];
            break;
        default:
            throw new ArgumentException(player.ToString());
    }

    score += scoreMap[playerShape];

    return score;
}