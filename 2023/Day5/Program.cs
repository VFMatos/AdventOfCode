using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public class Program
{
    public static void Main()
    {
        // Get the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();
        string relativePath = "Resources/GardenInformation.txt";
        // Combine the current directory and relative path to get the full path
        string gardenData = Path.Combine(currentDirectory, relativePath);

        // Read all lines from the text document
        string input = File.ReadAllText(gardenData);

        string mapPattern = @"(\S+)-to-(\S+) map:\s*((?:\d+\s*)+)?$";
        string seedsPattern = @"^seeds:\s+(\d+(\s+\d+)*\s)$";

        var maps = new List<Map>();
        RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;

        foreach (Match mapMatch in Regex.Matches(input, mapPattern, options))
        {
            string source = mapMatch.Groups[1].Value;
            string destination = mapMatch.Groups[2].Value;
            string ranges = mapMatch.Groups[3].Value;

            var map = new Map(source, destination);

            //Console.WriteLine($"Found map from {source} to {destination} wit values:\n{ranges}");

            foreach (var range in ranges.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var aux = range.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                map.Ranges.Add(new Range(long.Parse(aux[0]), long.Parse(aux[1]), long.Parse(aux[2])));
            }

            maps.Add(map);
        }

        var gardenInfos = new List<GardenInformation>();
        var seeds = Regex.Match(input, seedsPattern, options).Groups[1].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var seed in seeds)
        {
            var gardenInfo = new GardenInformation();
            gardenInfo.Seed = long.Parse(seed);
            gardenInfo.Soil = GetMapping(maps, nameof(GardenInformation.Seed), nameof(GardenInformation.Soil), gardenInfo.Seed);
            gardenInfo.Fertilizer = GetMapping(maps, nameof(GardenInformation.Soil), nameof(GardenInformation.Fertilizer), gardenInfo.Soil);
            gardenInfo.Water = GetMapping(maps, nameof(GardenInformation.Fertilizer), nameof(GardenInformation.Water), gardenInfo.Fertilizer);
            gardenInfo.Light = GetMapping(maps, nameof(GardenInformation.Water), nameof(GardenInformation.Light), gardenInfo.Water);
            gardenInfo.Temperature = GetMapping(maps, nameof(GardenInformation.Light), nameof(GardenInformation.Temperature), gardenInfo.Light);
            gardenInfo.Humidity = GetMapping(maps, nameof(GardenInformation.Temperature), nameof(GardenInformation.Humidity), gardenInfo.Temperature);
            gardenInfo.Location = GetMapping(maps, nameof(GardenInformation.Humidity), nameof(GardenInformation.Location), gardenInfo.Humidity);

            //Console.WriteLine($"Seed: {gardenInfo.Seed}, " +
            //				  $"Soil: {gardenInfo.Soil}, " +
            //				  $"Fertilizer: {gardenInfo.Fertilizer}, " +
            //				  $"Water: {gardenInfo.Water}, " +
            //				  $"Light: {gardenInfo.Light}, " +
            //				  $"Temperature: {gardenInfo.Temperature}, " +
            //				  $"Humidity: {gardenInfo.Humidity}, " +
            //				  $"Location: {gardenInfo.Location}");

            gardenInfos.Add(gardenInfo);
        }
        Console.WriteLine($"Part 1 result: {gardenInfos.Min(x => x.Location)}");
    }

    static bool IsBetween(long value, long start, long length)
    {
        return value >= start && value < start + length;
    }

    static long GetMapping(List<Map> maps, string source, string destination, long sourceValue)
    {
        var aux = maps.Where(x =>
                             string.Equals(x.Source, source, StringComparison.OrdinalIgnoreCase)
                             && string.Equals(x.Destination, destination, StringComparison.OrdinalIgnoreCase))
            .First()
            .Ranges
            .Select(x => x.GetDestinationFromSource(sourceValue))
            .Where(x => x != null);

        return aux.Any() ? (long)aux.First() : sourceValue;
    }

    public class Map
    {
        public Map(string source, string destination)
        {
            Source = source;
            Destination = destination;
        }

        public string Source { get; set; }
        public string Destination { get; set; }

        public List<Range> Ranges { get; set; } = new List<Range>();
    }

    public class Range
    {
        public Range(long destRangeStart, long srcRangeStart, long rangeLength)
        {
            DestinationRangeStart = destRangeStart;
            SourceRangeStart = srcRangeStart;
            RangeLength = rangeLength;
        }

        public long DestinationRangeStart { get; set; }
        public long SourceRangeStart { get; set; }
        public long RangeLength { get; set; }

        public long? GetDestinationFromSource(long sourceValue)
        {
            if (IsBetween(sourceValue, SourceRangeStart, RangeLength))
            {
                var aux = sourceValue - SourceRangeStart;
                return DestinationRangeStart + aux;
            }
            return null;
        }
    }

    public class GardenInformation
    {
        public long Seed { get; set; }
        public long Soil { get; set; }
        public long Fertilizer { get; set; }
        public long Water { get; set; }
        public long Light { get; set; }
        public long Temperature { get; set; }
        public long Humidity { get; set; }
        public long Location { get; set; }
    }
}
