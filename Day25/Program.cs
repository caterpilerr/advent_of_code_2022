const string inputPath = "input.txt";
var input = File.ReadAllLines(inputPath);
var solution = new Solution(input);
Console.WriteLine(solution.SolvePartOne());

internal class Solution
{
    private readonly List<string> _input;

    private readonly Dictionary<char, int> _snafuDigits = new()
    {
        ['2'] = 2,
        ['1'] = 1,
        ['0'] = 0,
        ['-'] = -1,
        ['='] = -2
    };

    private readonly Dictionary<int, string> _digitSnafu = new()
    {
        [5] = "01",
        [4] = "-1",
        [3] = "=1",
        [2] = "2",
        [1] = "1",
        [0] = "0",
    };

    public Solution(IEnumerable<string> input)
    {
        _input = input.ToList();
    }

    public string SolvePartOne()
    {
        var sum = _input.Select(FromSnafu).Sum();
        return ToSnafu(sum);
    }

    private string ToSnafuFromBaseFive(IEnumerable<int> reversedBaseFive)
    {
        var result = new List<char>();
        var carry = 0;
        foreach (var digit in reversedBaseFive)
        {
            var snafu = _digitSnafu[digit + carry];
            result.Add(snafu[0]);
            carry = snafu.Length > 1 ? _snafuDigits[snafu[1]] : 0;
        }

        if (carry > 0)
            result.Add(_digitSnafu[carry][0]);

        result.Reverse();
        return string.Join("", result);
    }

    private string ToSnafu(long value)
    {
        var reversedBaseFive = new List<int>();
        while (value > 0)
        {
            var digit = (int)(value % 5);
            reversedBaseFive.Add(digit);
            value /= 5;
        }

        return ToSnafuFromBaseFive(reversedBaseFive);
    }

    private long FromSnafu(string snafu)
    {
        var currentDigit = (long)Math.Pow(5, snafu.Length - 1);
        var result = 0L;
        foreach (var symbol in snafu)
        {
            result += currentDigit * _snafuDigits[symbol];
            currentDigit /= 5;
        }

        return result;
    }
}