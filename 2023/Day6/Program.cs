using System.Text.RegularExpressions;

public class Program
{
    public static void Main()
    {
        // Get the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();
        string relativePath = "Resources/RaceInformation.txt";
        // Combine the current directory and relative path to get the full path
        string gardenData = Path.Combine(currentDirectory, relativePath);

        // Read all lines from the text document
        string input = File.ReadAllText(gardenData);

        string timePattern = @"^Time:\s+([\d\s]+)$";
        string distancePattern = @"^Distance:\s+([\d\s]+)$";

        RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;

        var times = Regex.Match(input, timePattern, options).Groups[1].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var distances = Regex.Match(input, distancePattern, options).Groups[1].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var raceInfos = new List<RaceInformation>();

        for (int i = 0; i < times.Length; i++)
        {
            var raceInfo = new RaceInformation(i, int.Parse(times[i]), int.Parse(distances[i]));
            raceInfo.CountWaysToWin = CalculateWaysToWin(raceInfo.RaceTime, raceInfo.RecordDistance);

            raceInfos.Add(raceInfo);
        }

        Console.WriteLine($"Part 1 result: {raceInfos.Select(x => x.CountWaysToWin).Aggregate((x, y) => x * y)}");
    }

    static int CalculateWaysToWin(int raceTime, int recordDistance)
    {
        return Enumerable.Range(0, raceTime + 1).Count(x => x * (raceTime - x) > recordDistance);
    }

    public class RaceInformation
    {
        public RaceInformation(int id, int raceTime, int recordDistance)
        {
            Id = id;
            RaceTime = raceTime;
            RecordDistance = recordDistance;
        }

        public int Id { get; set; }
        public int RaceTime { get; set; }
        public int RecordDistance { get; set; }
        public int CountWaysToWin { get; set; }
    }
}