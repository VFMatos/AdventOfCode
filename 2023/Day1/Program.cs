// Get the current working directory
using System.Text.RegularExpressions;

string currentDirectory = Directory.GetCurrentDirectory();

#region Part 1
string relativePath_Part1 = "Resources/CalibrationDocument_Part1.txt";

// Combine the current directory and relative path to get the full path
string calibrationDocumentPart1 = Path.Combine(currentDirectory, relativePath_Part1);

// Read all lines from the text document
string[] linesPart1 = File.ReadAllLines(calibrationDocumentPart1);

// Get calibration values for each line
var calibrationValuesPart1 = linesPart1
    .ToList()
    .Select(x => new { FirstDigit = GetFirstDigit(x), LastDigit = GetLastDigit(x) })
    .Select(y => int.Parse($"{y.FirstDigit}{y.LastDigit}"));

// Calculate sum of all calibration values
var resultPart1 = calibrationValuesPart1.Sum();

Console.WriteLine($"Part 1 result: {resultPart1}");

static char GetFirstDigit(string input)
{
    char firstDigit = input.FirstOrDefault(char.IsDigit);
    return firstDigit;
}

static char GetLastDigit(string input)
{
    char lastDigit = input.LastOrDefault(char.IsDigit);
    return lastDigit;
}
#endregion


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

string relativePath_Part2 = "Resources/CalibrationDocument_Part1.txt";

// Combine the current directory and relative path to get the full path
string calibrationDocumentPart2 = Path.Combine(currentDirectory, relativePath_Part2);

// Read all lines from the text document
string[] linesPart2 = File.ReadAllLines(calibrationDocumentPart2);

// Get calibration values for each line
var calibrationValuesPart2 = linesPart2
    .ToList()
    .Select(x => GetCalibrationValue(x));

// Calculate sum of all calibration values
var resultPart2 = calibrationValuesPart2.Sum();

Console.WriteLine($"Part 2 result: {resultPart2}");

int GetCalibrationValue(string input)
{
    // Define a regular expression pattern to match spelled-out digits or numeric digits
    string pattern = @"(?=(zero|one|two|three|four|five|six|seven|eight|nine|\d))";

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
    else if(SpelledDigitToNumericMapping.TryGetValue(matchedValue, out int spelledDigitValue))
    {
        return spelledDigitValue;
    }
    return 0;
}

