using System.Text.RegularExpressions;

public class Program
{
    public static void Main()
    {
        // Get the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();
        string relativePath = "Resources/UniverseInformation.txt";
        // Combine the current directory and relative path to get the full path
        string pipeMazeData = Path.Combine(currentDirectory, relativePath);

        // Read all lines from the text document
        string[] lines = File.ReadAllLines(pipeMazeData);

        var emptyLines = new List<int>();
        var galaxies = new List<GalaxyInformation>();

        int id = 1;
        for (int i = 1; i <= lines.Length; i++)
        {
            var matches = Regex.Matches(lines[i - 1], "#", RegexOptions.IgnoreCase);

            if (matches.Any())
            {
                foreach (Match match in matches)
                {
                    galaxies.Add(new GalaxyInformation(id, i, match.Index));
                    id++;
                }
            }
            else
            {
                emptyLines.Add(i);
            }
        }

        var emptyColumns = Enumerable.Range(0, lines[0].Length)
            .Except(galaxies.Select(x => x.Index).Distinct())
            .ToList();

        var pairs = GeneratePairs(galaxies);

        var universeExpansion = new List<int>() { 2, 1000000 };

        for (int i = 0; i < 2; i++)
        {
            foreach (var pair in pairs)
            {
                var indexDifference = Math.Abs(pair.Item1.Index - pair.Item2.Index);

                var lineNumberDifference = Math.Abs(pair.Item1.LineNumber - pair.Item2.LineNumber);

                var doubleColumns = indexDifference != 0 ? Enumerable.Range(Math.Min(pair.Item1.Index, pair.Item2.Index) + 1, indexDifference - 1)
                    .Intersect(emptyColumns).ToList() : new List<int>();

                var doubleLines = lineNumberDifference != 0 ? Enumerable.Range(Math.Min(pair.Item1.LineNumber, pair.Item2.LineNumber) + 1, lineNumberDifference - 1)
                    .Intersect(emptyLines).ToList() : new List<int>();

                pair.Distance = lineNumberDifference + indexDifference
                    + (universeExpansion[i] - 1) * doubleColumns.Count()
                    + (universeExpansion[i] - 1) * doubleLines.Count();
            }

            Console.WriteLine($"Part {i + 1} result: {pairs.Sum(x => x.Distance)}");
        }
    }

    private static List<GalaxyPair> GeneratePairs(List<GalaxyInformation> galaxies)
    {
        return galaxies.SelectMany((x, i) => galaxies.Skip(i + 1), (x, y) => new GalaxyPair(x, y)).ToList();
    }

    public class GalaxyPair
    {
        public GalaxyPair(GalaxyInformation item1, GalaxyInformation item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public GalaxyInformation Item1 { get; set; }
        public GalaxyInformation Item2 { get; set; }
        public long Distance { get; set; }
    }

    public class GalaxyInformation
    {
        public GalaxyInformation(int id, int lineNumber, int index)
        {
            Id = id;
            LineNumber = lineNumber;
            Index = index;
        }

        public int Id { get; set; }
        public int LineNumber { get; set; }
        public int Index { get; set; }
    }
}