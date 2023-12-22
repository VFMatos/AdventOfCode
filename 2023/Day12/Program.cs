// https://youtu.be/g3Ms5e7Jdqo?t=694
// https://dotnetfiddle.net/420y5d
using System.Collections.Immutable;

public class Program
{
    public static Dictionary<(string, ImmutableList<int>), long> cache = new Dictionary<(string, ImmutableList<int>), long>();

    public static void Main()
    {
        // Get the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();
        string relativePath = "Resources/HotSpringInformation.txt";
        // Combine the current directory and relative path to get the full path
        string data = Path.Combine(currentDirectory, relativePath);

        // Read all lines from the text document
        string[] lines = File.ReadAllLines(data);

        long total = 0;

        foreach (var line in lines)
        {
            var split = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();

            var symbols = split.First();
            var numbers = split.Last()
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse).ToImmutableList();

            var springInfo = new SpringInformation(symbols, numbers);

            total += Count(springInfo.Symbols, springInfo.Numbers);
        }

        Console.WriteLine($"Part 1 result: {total}");
    }

    public static long Count(string symbols, ImmutableList<int> numbers)
    {
        // When there are no more symbols, there can be no more numbers to be valid
        if (string.IsNullOrEmpty(symbols))
            return numbers.IsEmpty ? 1 : 0;

        // When there are no more numbers, there can be no more # in symbols to be valid
        if (numbers.IsEmpty)
            return symbols.Contains("#") ? 0 : 1;

        var key = (symbols, numbers);

        if (cache.ContainsKey(key))
            return cache[key];

        long result = 0;

        // If the first character is '?' or '.' we assume it is '.' and skip it
        if ((".?").Contains(symbols[0]))
        {
            result += Count(symbols.Substring(1), numbers);
        }

        // If the first character is '?' or '#' we assume it is '#'
        if (("#?").Contains(symbols[0]))
        {
            // Validate if i have enough symbols for the number I need and contains no '.'
            if (
            //Validate if i have enough symbols for the number N
            numbers[0] <= symbols.Length
            // Validate if I have a block with N symbols != '.'
            && !symbols.Substring(0, numbers[0]).Contains(".")
            // Validate if is there are no more symbols or the next one is not '#', because there are no adjacent blocks
            && (numbers[0] == symbols.Length || symbols[numbers[0]] != '#'))
            {
                if (numbers[0] == symbols.Length)
                    result += Count(symbols.Substring(numbers[0]), numbers.Skip(1).ToImmutableList());
                else
                    result += Count(symbols.Substring(numbers[0] + 1), numbers.Skip(1).ToImmutableList());
            }
        }

        cache[key] = result;

        return result;
    }

    public class SpringInformation
    {
        public SpringInformation(string symbols, ImmutableList<int> numbers)
        {
            Symbols = symbols;
            Numbers = numbers;
        }

        public string Symbols { get; set; }
        public ImmutableList<int> Numbers { get; set; }
    }
}