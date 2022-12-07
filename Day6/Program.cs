// See https://aka.ms/new-console-template for more information
const string FilePath = "input.txt";
const int PacketSyncLength = 4;
const int MessageSyncLength = 14;

var input = File.ReadAllLines(FilePath);

foreach (var line in input)
{
    var packetStart = FindMarker(line, 0, PacketSyncLength);
    var messageStart = FindMarker(line, packetStart, MessageSyncLength);
    Console.WriteLine($"Number of packet index: {packetStart}");
    Console.WriteLine($"Number of message index: {messageStart}");
}

int FindMarker(string packet, int offset, int syncLength)
{
    var symbols = new int[26];
    var slow = offset;
    var fast = offset;
    while (fast < packet.Length)
    {
        var fastCharOffset = packet[fast] - 'a';
        symbols[fastCharOffset]++;
        if (fast - slow + 1 == syncLength)
        {
            if (IsWithoutDuplicates(symbols))
            {
                return fast + 1;
            }

            var slowCharOffset = packet[slow] - 'a';
            symbols[slowCharOffset]--;
            slow++;
        }

        fast++;
    }

    return -1;
}

bool IsWithoutDuplicates(int[] charData)
{
    foreach (var count in charData)
    {
        if (count > 1)
        {
            return false;
        }
    }

    return true;
}
