public class Program
{
    public static void Main()
    {
        // Get the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();
        string relativePath = "Resources/GameInformation.txt";
        // Combine the current directory and relative path to get the full path
        string gameData = Path.Combine(currentDirectory, relativePath);

        // Read all lines from the text document
        string[] lines = File.ReadAllLines(gameData);

        // Dictionary of color and max quantity pairs
        Dictionary<string, int> validQuantities = new Dictionary<string, int>
        {
          { "red", 12 },
          { "green", 13 },
          { "blue", 14 },
        };

        var gameInfos = new List<GameInformation>();

        foreach (var line in lines)
        {
            var gameInfo = new GameInformation(int.Parse(line.Split(": ").First().Split(' ').Last()));

            var gameSets = line.Split(": ").Last().Split("; ")
                .SelectMany(x => x.Split(", ")
                    .Select(x => new { Quantity = int.Parse(x.Split(' ').First()), Color = x.Split(' ').Last() })
              );

            var maxQuantityByColor = gameSets
              .GroupBy(x => x.Color)
              .Select(x => new ColorMaxQuantity() { Color = x.Key, MaxQuantity = x.Max(y => y.Quantity) })
              .ToList();

            // Check all conditions in a single query
            gameInfo.IsValid = ValidateQuantities(maxQuantityByColor, validQuantities);
			
			gameInfo.Power = maxQuantityByColor.Select(x => x.MaxQuantity).Aggregate((x, y) => x * y);

            gameInfos.Add(gameInfo);
        }

        Console.WriteLine($"Part 1 result: {gameInfos.Where(x => x.IsValid).Sum(x => x.Id)}");
        Console.WriteLine($"Part 2 result: {gameInfos.Sum(x => x.Power)}");
    }

    static bool ValidateQuantities(List<ColorMaxQuantity> maxQuantityByColor, Dictionary<string, int> validQuantities)
    {
        return maxQuantityByColor.All(x => validQuantities.TryGetValue(x.Color, out int maxQuantity)
            && x.MaxQuantity <= maxQuantity);
    }
    public class GameInformation
    {
        public GameInformation(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
        public bool IsValid { get; set; }
		public int Power {get; set; }
    }

    public class ColorMaxQuantity
    {
        public string Color { get; set; }
        public int MaxQuantity { get; set; }
    }
}
