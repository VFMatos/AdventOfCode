using System.Diagnostics;
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

        var maps = new List<Map>();
        RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;

        foreach (Match mapMatch in Regex.Matches(input, mapPattern, options))
        {
            string source = mapMatch.Groups[1].Value;
            string destination = mapMatch.Groups[2].Value;
            string ranges = mapMatch.Groups[3].Value;

            var map = new Map(source, destination);

            foreach (var range in ranges.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var aux = range.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                map.Ranges.Add(new Range(long.Parse(aux[0]), long.Parse(aux[1]), long.Parse(aux[2])));
            }

            maps.Add(map);
        }

        Stopwatch stopwatch = new Stopwatch();

        #region Part 1
        stopwatch.Start();
        var seeds = GetSeeds(input);

        long overallMinimumLocation = long.MaxValue;
        foreach (var seed in seeds)
        {
            var location = GetLocationFromSeed(maps, seed);
            overallMinimumLocation = Math.Min(overallMinimumLocation, location);
        }
        stopwatch.Stop();

        Console.WriteLine($"Part 1 result: {overallMinimumLocation} | Elapsed ms: {stopwatch.ElapsedMilliseconds}");
        #endregion

        #region Part 2
        var seedRanges = GetSeedRanges(input);

        stopwatch.Start();
        for (long j = 1; j <= long.MaxValue; j++)
        {
            var seed = GetSeedFromLocation(maps, j);

            if (seedRanges.Any(x => IsBetween(seed, x.Start, x.Length)))
            {
                stopwatch.Stop();
                Console.WriteLine($"Part 2 result: {j} | Elapsed ms: {stopwatch.ElapsedMilliseconds}");
                break;
            }
        }
        #endregion
    }

    static long GetLocationFromSeed(List<Map> maps, long seed)
    {
        var gardenInfo = new GardenInformation();
        gardenInfo.Seed = seed;
        gardenInfo.Soil = GetDestinationMapping(maps, nameof(GardenInformation.Seed), nameof(GardenInformation.Soil), gardenInfo.Seed);
        gardenInfo.Fertilizer = GetDestinationMapping(maps, nameof(GardenInformation.Soil), nameof(GardenInformation.Fertilizer), gardenInfo.Soil);
        gardenInfo.Water = GetDestinationMapping(maps, nameof(GardenInformation.Fertilizer), nameof(GardenInformation.Water), gardenInfo.Fertilizer);
        gardenInfo.Light = GetDestinationMapping(maps, nameof(GardenInformation.Water), nameof(GardenInformation.Light), gardenInfo.Water);
        gardenInfo.Temperature = GetDestinationMapping(maps, nameof(GardenInformation.Light), nameof(GardenInformation.Temperature), gardenInfo.Light);
        gardenInfo.Humidity = GetDestinationMapping(maps, nameof(GardenInformation.Temperature), nameof(GardenInformation.Humidity), gardenInfo.Temperature);
        gardenInfo.Location = GetDestinationMapping(maps, nameof(GardenInformation.Humidity), nameof(GardenInformation.Location), gardenInfo.Humidity);

        return gardenInfo.Location;
    }

    static long GetSeedFromLocation(List<Map> maps, long location)
    {
        var gardenInfo = new GardenInformation();

        gardenInfo.Location = location;
        gardenInfo.Humidity = GetSourceMapping(maps, nameof(GardenInformation.Humidity), nameof(GardenInformation.Location), gardenInfo.Location);
        gardenInfo.Temperature = GetSourceMapping(maps, nameof(GardenInformation.Temperature), nameof(GardenInformation.Humidity), gardenInfo.Humidity);
        gardenInfo.Light = GetSourceMapping(maps, nameof(GardenInformation.Light), nameof(GardenInformation.Temperature), gardenInfo.Temperature);
        gardenInfo.Water = GetSourceMapping(maps, nameof(GardenInformation.Water), nameof(GardenInformation.Light), gardenInfo.Light);
        gardenInfo.Fertilizer = GetSourceMapping(maps, nameof(GardenInformation.Fertilizer), nameof(GardenInformation.Water), gardenInfo.Water);
        gardenInfo.Soil = GetSourceMapping(maps, nameof(GardenInformation.Soil), nameof(GardenInformation.Fertilizer), gardenInfo.Fertilizer);
        gardenInfo.Seed = GetSourceMapping(maps, nameof(GardenInformation.Seed), nameof(GardenInformation.Soil), gardenInfo.Soil);

        return gardenInfo.Seed;
    }

    static List<long> GetSeeds(string input)
    {
        string seedsPattern = @"^seeds:\s+(\d+(\s+\d+)*\s)$";
        RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;

        var seeds = Regex.Match(input, seedsPattern, options).Groups[1].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => long.Parse(x))
            .ToList();

        return seeds;
    }

    static List<SeedRange> GetSeedRanges(string input)
    {
        string seedsPattern = @"^seeds:\s+(\d+(\s+\d+)*\s)$";
        RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;

        var seeds = Regex.Match(input, seedsPattern, options).Groups[1].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => long.Parse(x))
            .ToList();

        List<SeedRange> seedRange = new List<SeedRange>();

        for (int i = 0; i < seeds.Count() - 1; i++)
        {
            if (i % 2 == 0)
            {
                seedRange.Add(new SeedRange(seeds[i], seeds[i + 1]));
            }
        }

        return seedRange;
    }

    static bool IsBetween(long value, long start, long length)
    {
        return value >= start && value < start + length;
    }

    static long GetDestinationMapping(List<Map> maps, string source, string destination, long sourceValue)
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

    static long GetSourceMapping(List<Map> maps, string source, string destination, long destValue)
    {
        var aux = maps.Where(x =>
                             string.Equals(x.Source, source, StringComparison.OrdinalIgnoreCase)
                             && string.Equals(x.Destination, destination, StringComparison.OrdinalIgnoreCase))
            .First()
            .Ranges
            .Select(x => x.GetSourceFromDestination(destValue))
            .Where(x => x != null);

        return aux.Any() ? (long)aux.First() : destValue;
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

    public class SeedRange
    {
        public SeedRange(long start, long length)
        {
            Start = start;
            Length = length;
        }
        public long Start { get; set; }
        public long Length { get; set; }
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
        public long? GetSourceFromDestination(long destValue)
        {
            if (IsBetween(destValue, DestinationRangeStart, RangeLength))
            {
                var aux = destValue - DestinationRangeStart;
                return SourceRangeStart + aux;
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
