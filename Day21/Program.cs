// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text.RegularExpressions;

const string inputPath = "input.txt";

var input = File.ReadAllLines(inputPath).ToList();
var monkeys = new Dictionary<string, Monkey>();

input.ForEach(line => AddMonkey(monkeys, line));

var rootMonkey = monkeys["root"];

var timer = new Stopwatch();
timer.Start();
var partOne = rootMonkey.YellResult();
timer.Stop();

Console.WriteLine($"Part One answer: {partOne}");
Console.WriteLine($"Elapsed time for part one is {timer.ElapsedMilliseconds} ms");

timer.Restart();
var partTwo = rootMonkey.PartTwo();
timer.Stop();

Console.WriteLine($"Part Two answer: {partTwo}");
Console.WriteLine($"Elapsed time for part two is {timer.ElapsedMilliseconds} ms");

void AddMonkey(Dictionary<string, Monkey> monkeyPack, string description)
{
    const string mathMonkeyPattern =
        @"^(?<name>[a-z]+): (?<leftMonkey>[a-z]+) (?<operator>\W{1}) (?<rightMonkey>[a-z]+)$";
    const string numberMonkeyPattern = @"^(?<name>[a-z]+): (?<number>\d+)$";
    Monkey newMonkey;
    string monkeyName;
    if (Regex.IsMatch(description, mathMonkeyPattern))
    {
        var match = Regex.Match(description, mathMonkeyPattern);
        var leftMonkey = match.Groups["leftMonkey"].Value;
        var rightMonkey = match.Groups["rightMonkey"].Value;
        var @operator = match.Groups["operator"].Value;
        monkeyName = match.Groups["name"].Value;
        newMonkey = new MathMonkey(monkeyName, monkeyPack, leftMonkey, rightMonkey, @operator);
    }
    else
    {
        var match = Regex.Match(description, numberMonkeyPattern);
        monkeyName = match.Groups["name"].Value;
        var monkeyNumber = int.Parse(match.Groups["number"].Value);
        newMonkey = new NumberMonkey(monkeyName, monkeyNumber);
    }

    monkeyPack[monkeyName] = newMonkey;
}

internal abstract class Monkey
{
    protected string Name { get; }

    protected Monkey(string name)
    {
        Name = name;
    }

    public abstract long YellResult();

    public abstract long? PartTwo();

    public abstract long FindUnknown(long equal);
}

internal class NumberMonkey : Monkey
{
    private readonly long _number;

    public NumberMonkey(string name, long number) : base(name)
    {
        _number = number;
    }

    public override long YellResult()
    {
        return _number;
    }

    public override long? PartTwo()
    {
        return Name == "humn" ? null : _number;
    }


    public override string ToString()
    {
        return $"{Name}: {_number}";
    }

    public override long FindUnknown(long equal)
    {
        if (Name == "humn")
        {
            return equal;
        }

        throw new InvalidOperationException();
    }
}

internal class MathMonkey : Monkey
{
    private readonly Dictionary<string, Monkey> _monkeyPack;
    private readonly string _leftOperandMonkey;
    private readonly string _rightOperandMonkey;
    private readonly string _operatorSymbol;
    private long? _cached;
    private long? _partTwoCached;

    public MathMonkey(
        string name,
        Dictionary<string, Monkey> monkeyPack,
        string leftOperandMonkey,
        string rightOperandMonkey,
        string operatorSymbol) : base(name)
    {
        _monkeyPack = monkeyPack;
        _leftOperandMonkey = leftOperandMonkey;
        _rightOperandMonkey = rightOperandMonkey;
        _operatorSymbol = operatorSymbol;
    }

    public override long YellResult()
    {
        if (_cached.HasValue)
        {
            return _cached.Value;
        }

        var leftMonkey = _monkeyPack[_leftOperandMonkey];
        var rightMonkey = _monkeyPack[_rightOperandMonkey];
        var leftValue = leftMonkey.YellResult();
        var rightValue = rightMonkey.YellResult();
        var result = _operatorSymbol switch
        {
            "+" => leftValue + rightValue,
            "-" => leftValue - rightValue,
            "*" => leftValue * rightValue,
            "/" => leftValue / rightValue,
            _ => throw new InvalidOperationException()
        };

        _cached = result;

        return result;
    }

    public override long? PartTwo()
    {
        if (_partTwoCached.HasValue)
        {
            return _partTwoCached.Value;
        }

        if (Name == "root")
        {
            return EqualOperation();
        }

        var leftMonkey = _monkeyPack[_leftOperandMonkey];
        var rightMonkey = _monkeyPack[_rightOperandMonkey];
        var leftValue = leftMonkey.PartTwo();
        var rightValue = rightMonkey.PartTwo();
        var result = _operatorSymbol switch
        {
            "+" => leftValue + rightValue,
            "-" => leftValue - rightValue,
            "*" => leftValue * rightValue,
            "/" => leftValue / rightValue,
            _ => throw new InvalidOperationException()
        };

        _partTwoCached = result;

        return result;
    }

    private long? EqualOperation()
    {
        var left = _monkeyPack[_leftOperandMonkey];
        var right = _monkeyPack[_rightOperandMonkey];
        var leftValue = left.PartTwo();
        var rightValue = right.PartTwo();
        if (leftValue is null && rightValue is not null)
        {
            return left.FindUnknown(rightValue.Value);
        }

        if (rightValue is null && leftValue is not null)
        {
            return right.FindUnknown(leftValue.Value);
        }

        throw new InvalidOperationException();
    }

    public override long FindUnknown(long result)
    {
        var leftMonkey = _monkeyPack[_leftOperandMonkey];
        var rightMonkey = _monkeyPack[_rightOperandMonkey];
        var leftValue = leftMonkey.PartTwo();
        var rightValue = rightMonkey.PartTwo();

        if (leftValue is null && rightValue is not null)
        {
            var equal = _operatorSymbol switch
            {
                "+" => result - rightValue,
                "-" => result + rightValue,
                "*" => result / rightValue,
                "/" => result * rightValue,
                _ => throw new InvalidOperationException()
            };

            return leftMonkey.FindUnknown(equal!.Value);
        }

        if (rightValue is null && leftValue is not null)
        {
            var equal = _operatorSymbol switch
            {
                "+" => result - leftValue,
                "-" => leftValue - result,
                "*" => result / leftValue,
                "/" => leftValue / rightValue,
                _ => throw new InvalidOperationException()
            };

            return rightMonkey.FindUnknown(equal!.Value);
        }

        throw new InvalidOperationException();
    }

    public override string ToString()
    {
        return $"{Name}: {_leftOperandMonkey} {_operatorSymbol} {_rightOperandMonkey}";
    }
}