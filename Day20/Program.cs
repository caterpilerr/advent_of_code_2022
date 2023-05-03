// See https://aka.ms/new-console-template for more information

const string filePath = "input.txt";

var input = File.ReadAllLines(filePath).Select(long.Parse).ToArray();

var dataPartOne = input.Select((value, index) => (value, index)).ToArray();
var partOne = Solve(dataPartOne);

Console.WriteLine($"Part 1 answer: {partOne}");

const int multiplier = 811589153;
var dataPartTwo = input.Select((value, index) => (value * multiplier, index)).ToArray();
var partTwo = Solve(dataPartTwo, 10);

Console.WriteLine($"Part 2 answer: {partTwo}");

long Solve(IReadOnlyCollection<(long value, int index)> dataInput, int numberOfCycles = 1)
{
    var indexes = dataInput.ToDictionary(item => item, item => item.index);
    var decrypted = new List<(long Value, int InitialIndex)>(dataInput);
    for (var cycle = 0; cycle < numberOfCycles; cycle++)
    {
        foreach (var current in dataInput)
        {
            var currentIndex = indexes[current];
            var positionsToGo = (int)(current.value % (dataInput.Count - 1));
            positionsToGo = positionsToGo < 0 ? dataInput.Count + positionsToGo : positionsToGo;
            if (current.value < 0)
            {
                positionsToGo--;
            }

            var newIndex = (currentIndex + positionsToGo) % dataInput.Count;
            if (newIndex >= currentIndex)
            {
                for (var i = currentIndex + 1; i <= newIndex; i++)
                {
                    decrypted[i - 1] = decrypted[i];
                    indexes[decrypted[i]] = i - 1;
                }

                decrypted[newIndex] = current;
                indexes[current] = newIndex;
            }
            else
            {
                for (var i = currentIndex - 1; i >= newIndex + 1; i--)
                {
                    decrypted[i + 1] = decrypted[i];
                    indexes[decrypted[i]] = i + 1;
                }

                decrypted[newIndex + 1] = current;
                indexes[current] = newIndex + 1;
            }
        }
    }

    var zero = decrypted.Find(x => x.Value == 0);
    var zeroIndex = decrypted.IndexOf(zero);
    var oneThousandIndex = (zeroIndex + 1000) % decrypted.Count;
    var twoThousandIndex = (zeroIndex + 2000) % decrypted.Count;
    var threeThousandIndex = (zeroIndex + 3000) % decrypted.Count;
    
    return decrypted[oneThousandIndex].Value + decrypted[twoThousandIndex].Value +
           decrypted[threeThousandIndex].Value;
}