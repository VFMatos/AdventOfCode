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
        var copies = new List<Copy>();

        foreach (var line in lines)
        {
            var scratchInfo = new ScratchInformation(int.Parse(line.Split(": ").First().Split(' ').Last()));

            // Update total number of Card
            var copy = copies.FirstOrDefault(x => x.Id == scratchInfo.Id);
            if (copy != null)
            {
                scratchInfo.TotalNumberOfCards += copy.NumberOfCopies;
            }

            var numbers = line.Split(": ").Last().Split(" | ");

            var winningNumbers = numbers.First()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x));

            var chosenNumbers = numbers.Last()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x));

            scratchInfo.WinningNumbers = winningNumbers.Intersect(chosenNumbers);

            if (scratchInfo.WinningNumbers.Any())
            {
                scratchInfo.Worth = (int)Math.Pow(2, scratchInfo.WinningNumbers.Count() - 1);

                // Add copy to list of copies
                for (int i = 1; i <= scratchInfo.WinningNumbers.Count(); i++)
                {
                    copy = copies.FirstOrDefault(x => x.Id == scratchInfo.Id + i);
                    if (copy != null)
                    {
                        copy.NumberOfCopies += scratchInfo.TotalNumberOfCards;
                    }
                    else
                    {
                        copies.Add(new Copy(scratchInfo.Id + i, scratchInfo.TotalNumberOfCards));
                    }
                }
            }

            scratchInfos.Add(scratchInfo);
        }

        Console.WriteLine($"Part 1 result: {scratchInfos.Sum(x => x.Worth)}");
        Console.WriteLine($"Part 2 result: {scratchInfos.Sum(x => x.TotalNumberOfCards)}");
    }

    public class ScratchInformation
    {
        public ScratchInformation(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
        public IEnumerable<int> WinningNumbers { get; set; }
        public int Worth { get; set; } = 0;
        public int TotalNumberOfCards { get; set; } = 1;
    }

    public class Copy
    {
        public Copy(int id, int numberOfCopies)
        {
            Id = id;
            NumberOfCopies = numberOfCopies;
        }

        public int Id { get; set; }
        public int NumberOfCopies { get; set; }
    }
}