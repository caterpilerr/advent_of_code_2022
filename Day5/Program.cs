// See https://aka.ms/new-console-template for more information
const string filePath = "input.txt";

var input = File.ReadAllLines(filePath);

var stacks = new Stack<char>[]
{
    new(new[] { 'W', 'B', 'D', 'N', 'C', 'F', 'J' }),
    new(new[] { 'P', 'Z', 'V', 'Q', 'L', 'S', 'T' }),
    new(new[] { 'P', 'Z', 'B', 'G', 'J', 'T' }),
    new(new[] { 'D', 'T', 'L', 'J', 'Z', 'B', 'H', 'C' }),
    new(new[] { 'G', 'V', 'B', 'J', 'S' }),
    new(new[] { 'P', 'S', 'Q' }),
    new(new[] { 'B', 'V', 'D', 'F', 'L', 'M', 'P', 'N' }),
    new(new[] { 'P', 'S', 'M', 'F', 'B', 'D', 'L', 'R' }),
    new(new[] { 'V', 'D', 'T', 'R' }),
};

var data = input.Select(
    line => line.Split(' ')
        .Where((_, index) => index % 2 != 0)
        .Select(int.Parse)
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