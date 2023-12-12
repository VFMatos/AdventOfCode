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
            var matches = Regex.Matches(line, @"\b[a-zA-Z0-9]+\b", options);

            var mapInfo = new MapInformation(matches[0].Value, matches[1].Value, matches[2].Value);

            mapInfos.Add(mapInfo);
        }

        var node = mapInfos.Where(x => x.Position == "AAA").First();
        long countToDestination = CountToDestination(node, mapInfos, instructions, x => x.Position != "ZZZ");

        Console.WriteLine($"Part 1 result: {countToDestination}");

        var nodes = mapInfos.Where(x => x.Position.EndsWith("A"));
        var countsToZ = new List<long>();

        Parallel.ForEach(nodes, node =>
        {
            var count = CountToDestination(node, mapInfos, instructions, x => !x.Position.EndsWith("Z"));

            countsToZ.Add(count);
        });

        var leastCommonMultiple = CalculateLCM(countsToZ);

        Console.WriteLine($"Part 2 result: {leastCommonMultiple}");
    }

    static long CountToDestination(MapInformation startNode, List<MapInformation> mapInfos, char[] instructions, Func<MapInformation, bool> condition)
    {
        long count = 0;

        while (condition(startNode))
        {
            foreach (var inst in instructions)
            {
                if (inst == 'L')
                {
                    startNode = mapInfos.FirstOrDefault(y => y.Position == startNode.Left);
                }
                else
                {
                    startNode = mapInfos.FirstOrDefault(y => y.Position == startNode.Right);
                }
                count++;
            }
        }

        return count;
    }

    static long CalculateLCM(List<long> numbers)
    {
        long lcm = 1;

        foreach (long number in numbers)
        {
            lcm = CalculateLCM(lcm, number);
        }

        return lcm;
    }

    static long CalculateLCM(long a, long b)
    {
        return Math.Abs(a * b) / CalculateGCD(a, b);
    }

    static long CalculateGCD(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }

        return Math.Abs(a);
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