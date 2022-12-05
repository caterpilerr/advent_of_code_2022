// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;

const string FilePath = "input.txt";

var input = File.ReadAllLines(FilePath);

var stacks = new Stack<char>[]
{
    new Stack<char> (new [] {'W', 'B', 'D', 'N', 'C', 'F', 'J'}),
    new Stack<char> (new [] {'P', 'Z', 'V', 'Q', 'L', 'S', 'T'}),
    new Stack<char> (new [] {'P', 'Z', 'B', 'G', 'J', 'T'}),
    new Stack<char> (new [] {'D', 'T', 'L', 'J', 'Z', 'B', 'H', 'C'}),
    new Stack<char> (new [] {'G', 'V', 'B', 'J', 'S'}),
    new Stack<char> (new [] {'P', 'S', 'Q'}),
    new Stack<char> (new [] {'B', 'V', 'D', 'F', 'L', 'M', 'P', 'N'}),
    new Stack<char> (new [] {'P', 'S', 'M', 'F', 'B', 'D', 'L', 'R'}),
    new Stack<char> (new [] {'V', 'D', 'T', 'R'}),
};

var data = input.Select(
    line => line.Split(' ')
        .Where((val, index) => index % 2 != 0)
        .Select(x => int.Parse(x))
        .ToArray());

var tempStack = new Stack<char>();
foreach (var command in data)
{
    var amount = command[0];
    var from = command[1] - 1;
    var to = command[2] - 1;
    for (var i = 0; i < amount; i++)
    {
        tempStack.Push(stacks[from].Pop());
    }

    for (var i = 0; i < amount; i++)
    {
        stacks[to].Push(tempStack.Pop());
    }
}

Array.ForEach(stacks, stack => Console.Write(stack.Peek()));