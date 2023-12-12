using System.Text.RegularExpressions;

public class Program
{
    public static void Main()
    {
        // Get the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();
        string relativePath = "Resources/MapInformation.txt";
        // Combine the current directory and relative path to get the full path
        string scratchCardData = Path.Combine(currentDirectory, relativePath);

        // Read all lines from the text document
        string[] lines = File.ReadAllLines(scratchCardData);

        RegexOptions options = RegexOptions.IgnoreCase;

        var mapInfos = new List<MapInformation>();

        var instructions = lines.First().ToCharArray();

        foreach (var line in lines.Skip(2))
        {
            var matches = Regex.Matches(line, @"\b[A-Z]+\b", options);

            var mapInfo = new MapInformation(matches[0].Value, matches[1].Value, matches[2].Value);

            mapInfos.Add(mapInfo);
        }

        var currentPosition = mapInfos.Where(x => x.Position == "AAA").First();
        var destination = mapInfos.Where(x => x.Position == "ZZZ").First().Position;

        int i = 0;
        int rounds = 0;

        while (currentPosition.Position != destination)
        {
            if (instructions[i] == 'L')
            {
                currentPosition = mapInfos.Where(x => x.Position == currentPosition.Left).First();
            }
            else
            {
                currentPosition = mapInfos.Where(x => x.Position == currentPosition.Right).First();
            }

            if (i < instructions.Length - 1)
            {
                i++;
            }
            else
            {
                i = 0;
                rounds++;
            }
        }

        Console.WriteLine($"Part 1 result: {rounds * instructions.Length + i}");
    }

    public class MapInformation
    {
        public MapInformation(string position, string left, string right)
        {
            Position = position;
            Left = left;
            Right = right;
        }

        public string Position { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
    }
}