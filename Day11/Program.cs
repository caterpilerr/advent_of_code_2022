// See https://aka.ms/new-console-template for more information
using System.Collections;

var monkeys = new MonkeyPack
{
    new(
        new long[]{ 74, 73, 57, 77, 74 },
        worries => worries * 11,
        worries => worries % 19 == 0,
        6,
        7
    ),
    new(
        new long[]{ 99, 77, 79 },
        worries => worries + 8,
        worries => worries % 2 == 0,
        6,
        0
    ),
    new(
        new long[]{ 64, 67, 50, 96, 89, 82, 82 },
        worries => worries + 1,
        worries => worries % 3 == 0,
        5,
        3
    ),
    new(
        new long[]{ 88 },
        worries => worries * 7,
        worries => worries % 17 == 0,
        5,
        4
    ),
    new(
        new long[]{ 80, 66, 98, 83, 70, 63, 57, 66 },
        worries => worries + 4,
        worries => worries % 13 == 0,
        0,
        1
    ),
    new(
        new long[]{ 81, 93, 90, 61, 62, 64 },
        worries => worries + 7,
        worries => worries % 7 == 0,
        1,
        4
    ),
    new(
        new long[]{ 69, 97, 88, 93 },
        worries => worries * worries,
        worries => worries % 5 == 0,
        7,
        2
    ),
    new(
        new long[]{ 59, 80 },
        worries => worries + 6,
        worries => worries % 11 == 0,
        2,
        3
    )
};

// var monkeys = new MonkeyPack()
// {
//     new Monkey(
//         new[]{ 79, 98 },
//         worrines => worrines * 19,
//         worrines => worrines % 23 == 0,
//         2,
//         3
//     ),
//     new Monkey(
//         new[]{ 54, 65, 75, 74 },
//         worrines => worrines + 6,
//         worrines => worrines % 19 == 0,
//         2,
//         0
//     ),
//     new Monkey(
//         new[]{ 79, 60, 97 },
//         worrines => worrines * worrines,
//         worrines => worrines % 13 == 0,
//         1,
//         3
//     ),
//     new Monkey(
//         new[]{ 74 },
//         worrines => worrines + 3,
//         worrines => worrines % 17 == 0,
//         0,
//         1
//     )
// };


// Part 1 & 2
// For part 2 Monkey.MakeTurn(nervous: true)
const int numberOfRounds = 10000;
var itemsPerRound = new long[8];
for (var round = 0; round < numberOfRounds; round++)
{
    for (var i = 0; i < monkeys.Count; i++)
    {
        var inspectedItems = monkeys[i].MakeTurn(nervous: true);
        itemsPerRound[i] += inspectedItems;
    }
}

for (var i = 0; i < itemsPerRound.Length; i++)
{
    Console.WriteLine($"Monkey {i} inspected {itemsPerRound[i]}");
}

var monkeyBusiness = itemsPerRound.OrderByDescending(x => x).Take(2).Aggregate(1L, (multiplied, current) => multiplied * current);
Console.WriteLine($"The Monkey Business index equal to {monkeyBusiness}");

internal class MonkeyPack : IEnumerable<Monkey>
{
    private List<Monkey> Monkeys { get; } = new();

    public int Count => Monkeys.Count;

    public IEnumerator<Monkey> GetEnumerator()
    {
        return Monkeys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(Monkey monkey)
    {
        Monkeys.Add(monkey);
        monkey.Monkeys = this;
    }

    public Monkey this[int i] => Monkeys[i];
}

internal class Monkey
{
    public Monkey(
        IEnumerable<long> items,
        Func<long, long> operation,
        Predicate<long> test,
        int monkeyIfTest,
        int monkeyIfNotTest)
    {
        Items = new Queue<long>(items);
        Operation = operation;
        Test = test;
        IfTestMonkey = monkeyIfTest;
        IfNotTestMonkey = monkeyIfNotTest;
    }

    private Func<long, long> Operation { get; }
    private Queue<long> Items { get; }
    private Predicate<long> Test { get; }
    private int IfTestMonkey { get; }
    private int IfNotTestMonkey { get; }

    private const int primeBase = 2 * 3 * 5 * 7 * 11 * 13 * 17 * 19;

    public MonkeyPack Monkeys { get; set; } = null!;

    private long Operate(long worries, bool nervous)
    {
        worries = Operation(worries);

        if (nervous)
        {
            worries = worries % primeBase;
        }
        else
        {
            worries = worries / 3;
        }


        return worries;
    }

    public int MakeTurn(bool nervous = false)
    {
        var proceededItems = 0;
        while (Items.TryDequeue(out var itemWorries))
        {
            itemWorries = Operate(itemWorries, nervous);

            if (Test(itemWorries))
            {
                Monkeys[IfTestMonkey].Take(itemWorries);
            }
            else
            {
                Monkeys[IfNotTestMonkey].Take(itemWorries);
            }

            proceededItems++;
        }

        return proceededItems;
    }

    public void Take(long itemWorries)
    {
        Items.Enqueue(itemWorries);
    }
}
