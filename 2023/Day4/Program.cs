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
        string relativePath = "Resources/ScratchCardInformation.txt";
        // Combine the current directory and relative path to get the full path
        string scratchCardData = Path.Combine(currentDirectory, relativePath);

        // Read all lines from the text document
        string[] lines = File.ReadAllLines(scratchCardData);

        var scratchInfos = new List<ScratchInformation>();

        foreach (var line in lines)
        {
            var scratchInfo = new ScratchInformation(int.Parse(line.Split(": ").First().Split(' ').Last()));

            var numbers = line.Split(": ").Last().Split(" | ");

            var winningNumbers = numbers.First()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => int.Parse(x));

            var chosenNumbers = numbers.Last()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => int.Parse(x));

            scratchInfo.WinningNumbers = winningNumbers.Intersect(chosenNumbers);
            scratchInfo.Worth = (int)Math.Pow(2, scratchInfo.WinningNumbers.Count() - 1);

            scratchInfos.Add(scratchInfo);
        }

        Console.WriteLine($"Part 1 result: {scratchInfos.Sum(x => x.Worth)}");
    }

    public class ScratchInformation
    {
        public ScratchInformation(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
        public IEnumerable<int> WinningNumbers { get; set; }
        public int Worth { get; set; }
    }
}