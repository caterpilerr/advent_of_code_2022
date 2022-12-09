// See https://aka.ms/new-console-template for more information
const string FilePath = "input.txt";
const int UppermostDirectoryLimit = 100000;
const int TotalDiskSpace = 70000000;
const int UnusedDiskSpaceLimit = 30000000;

var input = File.ReadAllLines(FilePath);

// Part 1
var root = ParseInputToDirectories(input);
var directorySizes = new List<int>();
GetDirectoriesSizes(root!, directorySizes);
var result1 = directorySizes.Where(x => x <= UppermostDirectoryLimit).Sum();

Console.WriteLine($"The sum size of directories with size at most 100000: {result1}");

// Part 2
var totalUsedSpace = directorySizes[directorySizes.Count - 1];
var totalUnusedSpace = TotalDiskSpace - totalUsedSpace; 
var spaceToFree = UnusedDiskSpaceLimit - totalUnusedSpace;
directorySizes.Sort();
var result2 = 0; 
foreach(var size in directorySizes)
{
    if (size >= spaceToFree)
    {
        result2 = size;
        break;
    }
}

Console.WriteLine($"The size of directory with minimal size to free enough space: {result2}");

int GetDirectoriesSizes(DirectoryObject current, List<int> sizes)
{
    var size = 0;
    foreach (var item in current.Content.Values)
    {
        if (item is FileObject file)
        {
            size += file.Size;
        }
        else if (item is DirectoryObject dir)
        {
            size += GetDirectoriesSizes(dir, sizes);
        }
    }

    sizes.Add(size);

    return size;
}

DirectoryObject? ParseInputToDirectories(string[] input)
{
    var stack = new Stack<DirectoryObject>();
    DirectoryObject? root = null;
    DirectoryObject? current = null;
    var lineNumber = 0;
    var lsMode = false;
    while (lineNumber < input.Length)
    {
        var data = input[lineNumber].Split(' ');
        if (data[0] == "$")
        {
            if (data[1] == "cd")
            {
                var arg = data[2];
                if (arg == "/")
                {
                    if (root is null)
                    {
                        root = new DirectoryObject(arg, null);
                    }

                    current = root;
                }
                else if (arg == "..")
                {
                    current = current?.ParentDirectory is not null ? current.ParentDirectory : current;
                }
                else
                {
                    current = current?.Content[arg] as DirectoryObject;
                }

                lsMode = false;
            }
            else if (data[1] == "ls")
            {
                lsMode = true;
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

class BaseObject
{
    public BaseObject(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}

class FileObject : BaseObject
{
    public FileObject(string name, int size) : base(name)
    {
        Size = size;
    }
    public int Size { get; protected set; }
}

class DirectoryObject : BaseObject
{
    public DirectoryObject(string name, DirectoryObject? parent) : base(name)
    {
        ParentDirectory = parent;
    }

    public DirectoryObject? ParentDirectory { get; set; }
    public Dictionary<string, BaseObject> Content { get; set; } = new Dictionary<string, BaseObject>();
}