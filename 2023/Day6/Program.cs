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

        var joinTimes = string.Join(string.Empty, times);
        var joinDistances = string.Join(string.Empty, distances);

        var newRace = new RaceInformation(raceInfos.Count() + 1, int.Parse(joinTimes), long.Parse(joinDistances));
        newRace.CountWaysToWin = CalculateWaysToWin(newRace.RaceTime, newRace.RecordDistance);


        Console.WriteLine($"Part 1 result: {raceInfos.Select(x => x.CountWaysToWin).Aggregate((x, y) => x * y)}");
        Console.WriteLine($"Part 2 result: {newRace.CountWaysToWin}");
    }

    static long CalculateWaysToWin(int raceTime, long recordDistance)
    {
        return Enumerable.Range(0, raceTime + 1).LongCount(x => (long)x * (long)(raceTime - x) > recordDistance);
    }

    public class RaceInformation
    {
        public RaceInformation(int id, int raceTime, long recordDistance)
        {
            Id = id;
            RaceTime = raceTime;
            RecordDistance = recordDistance;
        }

        public int Id { get; set; }
        public int RaceTime { get; set; }
        public long RecordDistance { get; set; }
        public long CountWaysToWin { get; set; }
    }
}