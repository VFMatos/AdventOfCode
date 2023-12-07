// Get the current working directory
using System.Text.RegularExpressions;

// Define a dictionary to map spelled-out digits to numeric values
Dictionary<string, int> SpelledDigitToNumericMapping = new Dictionary<string, int>
{
    { "zero",   0 },
    { "one",    1 },
    { "two",    2 },
    { "three",  3 },
    { "four",   4 },
    { "five",   5 },
    { "six",    6 },
    { "seven",  7 },
    { "eight",  8 },
    { "nine",   9 },
};

string currentDirectory = Directory.GetCurrentDirectory();
string relativePath = "Resources/CalibrationDocument.txt";
// Combine the current directory and relative path to get the full path
string calibrationDocument = Path.Combine(currentDirectory, relativePath);

// Read all lines from the text document
string[] lines = File.ReadAllLines(calibrationDocument);

var patterns = new List<string>()
{
    @"(?=(\d))",
    @"(?=(zero|one|two|three|four|five|six|seven|eight|nine|\d))"
};

int part = 1;
foreach (var pattern in patterns)
{
    // Get calibration values for each line
    var calibrationValues = lines
        .ToList()
        .Select(x => GetCalibrationValue(x, pattern));

    // Calculate sum of all calibration values
    var result = calibrationValues.Sum();

    Console.WriteLine($"Part {part} result: {result}");
    part++;
}

int GetCalibrationValue(string input, string pattern)
{
    // Use Regex.Matches to find all matches in the input string
    MatchCollection matches = Regex.Matches(input, pattern, RegexOptions.IgnoreCase);

    // If there are matches, extract the value of the last match
    if (matches.Any())
    {
        Match fistMatch = matches.First();
        Match lastMatch = matches.Last();

        return int.Parse($"{GetMatchedValue(fistMatch)}{GetMatchedValue(lastMatch)}");
    }

    // If no match is found, return 0 or handle accordingly
    return 0;
}

int GetMatchedValue(Match match)
{
    string matchedValue = match.Groups[1].Value.ToLower();

    if (int.TryParse(matchedValue, out int numericValue))
    {
        return numericValue;
    }
    else if (SpelledDigitToNumericMapping.TryGetValue(matchedValue, out int spelledDigitValue))
    {
        return spelledDigitValue;
    }
    return 0;
}