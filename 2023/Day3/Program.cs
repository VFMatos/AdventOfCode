using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        while (currentNode != null)
        {
            var currentLineInfo = currentNode.Value;

            if (currentLineInfo.Numbers.Any())
            {
                foreach (Match number in currentLineInfo.Numbers)
                {
                    var minIndex = number.Index - 1;
                    var maxIndex = number.Index + number.Length + 1;

                    var isPartNumber = HasAdjacentSymbols(currentLineInfo, minIndex, maxIndex)
                    || (currentNode.Previous != null && HasAdjacentSymbols(currentNode.Previous.Value, minIndex, maxIndex))
                    || (currentNode.Next != null && HasAdjacentSymbols(currentNode.Next.Value, minIndex, maxIndex));

                    if (isPartNumber)
                    {
                        partNumbers.Add(int.Parse(number.Value));
                    }
                }
            }

            // Move to the next node
            currentNode = currentNode.Next;
        }

        Console.WriteLine($"Part 1 result: {partNumbers.Sum()}");
    }

    static bool IsBetween(int index, int minIndex, int maxIndex)
    {
        return index >= minIndex && index <= maxIndex;
    }

    static bool HasAdjacentSymbols(EngineLineInformation lineInfo, int minIndex, int maxIndex)
    {
        return lineInfo.Symbols.Any(x => IsBetween(x.Index, minIndex, maxIndex));
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