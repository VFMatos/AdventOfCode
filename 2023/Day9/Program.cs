public class Program
{
    public static void Main()
    {
        // Get the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();
        string relativePath = "Resources/HistoryInformation.txt";
        // Combine the current directory and relative path to get the full path
        string scratchCardData = Path.Combine(currentDirectory, relativePath);

        // Read all lines from the text document
        string[] lines = File.ReadAllLines(scratchCardData);

        var extrapolatedValues = new List<ExtrapolatedValue>();

        foreach (var line in lines)
        {
            var values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => int.Parse(x))
            .ToList();

            var extrapolatedValue = CalculateExtrapolatedValue(values);
            extrapolatedValues.Add(extrapolatedValue);
        }

        Console.WriteLine($"Part 1 result: {extrapolatedValues.Sum(x => x.End)}");
        Console.WriteLine($"Part 2 result: {extrapolatedValues.Sum(x => x.Begin)}");
    }

    static ExtrapolatedValue CalculateExtrapolatedValue(List<int> values)
    {
        var finalValue = new ExtrapolatedValue(values.First(), values.Last());

        if (values.Any(x => x != 0))
        {
            List<int> differences = values
                        .Skip(1)
                        .Select((current, i) => current - values[i])
                        .ToList();

            var newValue = CalculateExtrapolatedValue(differences);

            finalValue.Begin -= newValue.Begin;
            finalValue.End += newValue.End;
        }
        return finalValue;
    }

    public class ExtrapolatedValue
    {
        public ExtrapolatedValue(int begin, int end)
        {
            Begin = begin;
            End = end;
        }

        public int Begin { get; set; }
        public int End { get; set; }
    }
}
