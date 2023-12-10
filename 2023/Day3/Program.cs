using System.Text.RegularExpressions;

public class Program
{
    public static void Main()
    {
        // Get the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();
        string relativePath = "Resources/EngineInformation.txt";
        // Combine the current directory and relative path to get the full path
        string engineData = Path.Combine(currentDirectory, relativePath);

        // Read all lines from the text document
        string[] lines = File.ReadAllLines(engineData);

        var engineLineInfo = new LinkedList<EngineLineInformation>(
       lines.Select((x, i) => new EngineLineInformation(i + 1)
       {
           Numbers = Regex.Matches(x, @"\d+"),
           Symbols = Regex.Matches(x, @"[^a-zA-Z0-9.]")
       }));

        var currentNode = engineLineInfo.First;

        var partNumbers = new List<int>();
        var gearRatios = new List<int>();

        while (currentNode != null)
        {
            if (currentNode.Value.Symbols.Any())
            {
                foreach (Match symbol in currentNode.Value.Symbols)
                {
                    var gearNumbers = new List<int>();

                    var currentPartNumbers = GetPartNumbersFromSymbol(currentNode.Value, symbol.Index);
                    var previousPartNumbers = GetPartNumbersFromSymbol(currentNode.Previous?.Value, symbol.Index);
                    var nextPartNumbers = GetPartNumbersFromSymbol(currentNode.Next?.Value, symbol.Index);

                    partNumbers.AddRange(currentPartNumbers);
                    partNumbers.AddRange(previousPartNumbers);
                    partNumbers.AddRange(nextPartNumbers);

                    if (symbol.Value == "*")
                    {
                        gearNumbers.AddRange(currentPartNumbers);
                        gearNumbers.AddRange(previousPartNumbers);
                        gearNumbers.AddRange(nextPartNumbers);

                        if (gearNumbers.Count() == 2)
                        {
                            gearRatios.Add(gearNumbers.Aggregate((x, y) => x * y));
                        }
                    }
                }
            }

            // Move to the next node
            currentNode = currentNode.Next;
        }

        Console.WriteLine($"Part 1 result: {partNumbers.Sum()}");
        Console.WriteLine($"Part 2 result: {gearRatios.Sum()}");
    }

    static bool IsBetween(int index, int minIndex, int maxIndex)
    {
        return index >= minIndex && index <= maxIndex;
    }

    static bool HasAdjacentSymbols(EngineLineInformation lineInfo, int minIndex, int maxIndex)
    {
        return lineInfo.Symbols.Any(x => IsBetween(x.Index, minIndex, maxIndex));
    }

    static List<int> GetPartNumbersFromSymbol(EngineLineInformation lineInfo, int symbolIndex)
    {
        return lineInfo != null ?
            lineInfo.Numbers
            .Where(x => IsBetween(symbolIndex, x.Index - 1, x.Index + x.Length))
            .Select(x => int.Parse(x.Value))
            .ToList()
            : new List<int>();
    }

    public class EngineLineInformation
    {
        public EngineLineInformation(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
        public MatchCollection Numbers { get; set; }
        public MatchCollection Symbols { get; set; }
    }
}
