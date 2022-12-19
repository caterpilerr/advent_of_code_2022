// See https://aka.ms/new-console-template for more information
const string filePath = "input.txt";
const int uppermostDirectoryLimit = 100000;
const int totalDiskSpace = 70000000;
const int unusedDiskSpaceLimit = 30000000;

var input = File.ReadAllLines(filePath);

// Part 1
var rootObject = ParseInputToDirectories(input);
var directorySizes = new List<int>();
GetDirectoriesSizes(rootObject!, directorySizes);
var result1 = directorySizes.Where(x => x <= uppermostDirectoryLimit).Sum();

Console.WriteLine($"The sum size of directories with size at most 100000: {result1}");

// Part 2
var totalUsedSpace = directorySizes[^1];
var totalUnusedSpace = totalDiskSpace - totalUsedSpace;
var spaceToFree = unusedDiskSpaceLimit - totalUnusedSpace;
directorySizes.Sort();
var result2 = directorySizes.FirstOrDefault(size => size >= spaceToFree);

Console.WriteLine($"The size of directory with minimal size to free enough space: {result2}");

int GetDirectoriesSizes(DirectoryObject current, List<int> sizes)
{
    var size = 0;
    foreach (var item in current.Content.Values)
    {
        switch (item)
        {
            case FileObject file:
                size += file.Size;
                break;
            case DirectoryObject dir:
                size += GetDirectoriesSizes(dir, sizes);
                break;
        }
    }

    sizes.Add(size);

    return size;
}

DirectoryObject? ParseInputToDirectories(IReadOnlyList<string> inputValue)
{
    DirectoryObject? root = null;
    DirectoryObject? current = null;
    var lineNumber = 0;
    var lsMode = false;
    while (lineNumber < inputValue.Count)
    {
        var data = inputValue[lineNumber].Split(' ');
        if (data[0] == "$")
        {
            switch (data[1])
            {
                case "cd":
                {
                    var arg = data[2];
                    switch (arg)
                    {
                        case "/":
                            root ??= new DirectoryObject(arg, null);
                            current = root;
                            break;
                        case "..":
                            current = current?.ParentDirectory ?? current;
                            break;
                        default:
                            current = current?.Content[arg] as DirectoryObject;
                            break;
                    }

                    lsMode = false;
                    break;
                }
                case "ls":
                    lsMode = true;
                    break;
            }
        }
        else if (lsMode)
        {
            var objName = data[1];
            if (!current!.Content.ContainsKey(objName))
            {
                if (data[0] == "dir")
                {
                    current.Content[objName] = new DirectoryObject(objName, current);
                }
                else
                {
                    current.Content[objName] = new FileObject(objName, int.Parse(data[0]));
                }
            }
        }

        lineNumber++;
    }

    return root;
}

internal class BaseObject
{
    protected BaseObject(string name)
    {
        Name = name;
    }

    private string Name { get; }
}

internal class FileObject : BaseObject
{
    public FileObject(string name, int size) : base(name)
    {
        Size = size;
    }
    
    public int Size { get; }
}

internal class DirectoryObject : BaseObject
{
    public DirectoryObject(string name, DirectoryObject? parent) : base(name)
    {
        ParentDirectory = parent;
    }

    public DirectoryObject? ParentDirectory { get; }
    public Dictionary<string, BaseObject> Content { get; } = new();
}