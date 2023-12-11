public class Program
{
    public static void Main()
    {
        // Get the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();
        string relativePath = "Resources/CamelCardInformation.txt";
        // Combine the current directory and relative path to get the full path
        string scratchCardData = Path.Combine(currentDirectory, relativePath);

        // Read all lines from the text document
        string[] lines = File.ReadAllLines(scratchCardData);

        string customOrder = "AKQJT98765432";

        var camelCardInfos = new List<CamelCardInformation>();

        foreach (var line in lines)
        {
            var lineInfo = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var camelCardInfo = new CamelCardInformation(lineInfo.First(), int.Parse(lineInfo.Last()));
            camelCardInfo.Type = GetHandType(camelCardInfo.Hand);

            camelCardInfos.Add(camelCardInfo);
        }

        /*
		var orderedCamelCards = camelCardInfos
            .OrderBy(x => x.Type)
            .ThenByDescending(x => x.Hand, new CustomOrderComparer(customOrder))
			.ToList();
        */

        var orderedCamelCards = camelCardInfos
            .OrderBy(x => x.Type)
            .ThenBy(x => x.Hand, new CustomOrderComparer2())
            .ToList();

        int rankCounter = 1;
        orderedCamelCards.ForEach(hand =>
        {
            hand.Rank = rankCounter;
            rankCounter++;
        });

        Console.WriteLine($"Part 1 result: {orderedCamelCards.Sum(x => x.GetWin())}");
        //Console.WriteLine($"Part 2 result: {newRace.CountWaysToWin}");

        foreach (var camelCardInfo in orderedCamelCards)
        {
            Console.WriteLine($"Hand: {camelCardInfo.Hand}" +
                             $", Bid: {camelCardInfo.Bid}" +
                             $", HandType: {camelCardInfo.Type}" +
                             $", Rank: {camelCardInfo.Rank}");
        }
    }

    static HandType GetHandType(string hand)
    {
        var countChars = CountCharacters(hand);
        var numberOfDifferentChars = countChars.Count();

        switch (numberOfDifferentChars)
        {
            case 5:
                return HandType.Highcard;
            case 4:
                return HandType.OnePair;
            case 3:
                if (countChars.Any(x => x.Value == 3))
                {
                    return HandType.ThreeOfAkind;
                }
                return HandType.TwoPair;
            case 2:
                if (countChars.Any(x => x.Value == 4))
                {
                    return HandType.FourOfAKind;
                }
                return HandType.FullHouse;
            case 1:
                return HandType.FiveOfAKind;
        }

        return HandType.FullHouse;
    }

    static Dictionary<char, int> CountCharacters(string input)
    {
        var charCount = input
           .GroupBy(c => c)
           .ToDictionary(g => g.Key, g => g.Count());

        return charCount;
    }

    public enum HandType
    {
        Highcard,
        OnePair,
        TwoPair,
        ThreeOfAkind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind
    }

    public class CamelCardInformation
    {
        public CamelCardInformation(string hand, int bid)
        {
            Hand = hand;
            Bid = bid;
        }

        public string Hand { get; set; }
        public int Bid { get; set; }
        public HandType Type { get; set; }
        public int Rank { get; set; }

        public long GetWin()
        {
            return (long)Bid * (long)Rank;
        }
    }

    class CustomOrderComparer : IComparer<string>
    {
        private readonly string customOrder;

        public CustomOrderComparer(string order)
        {
            customOrder = order;
        }

        public int Compare(string x, string y)
        {
            // Compare strings based on custom order
            for (int i = 0; i < Math.Min(x.Length, y.Length); i++)
            {
                int compareResult = customOrder.IndexOf(x[i]).CompareTo(customOrder.IndexOf(y[i]));
                if (compareResult != 0)
                {
                    return compareResult;
                }
            }

            return x.Length.CompareTo(y.Length);
        }
    }

    class CustomOrderComparer2 : IComparer<string>
    {
        private readonly Dictionary<char, int> charValues;

        public CustomOrderComparer2()
        {
            // Initialize character values
            charValues = new Dictionary<char, int>
            {
              {'2', 2},
              {'3', 3},
              {'4', 4},
              {'5', 5},
              {'6', 6},
              {'7', 7},
              {'8', 8},
              {'9', 9},
              {'T', 10},
              {'J', 11},
              {'Q', 12},
              {'K', 13},
              {'A', 14}
            };
        }

        public int Compare(string x, string y)
        {
            for (int i = 0; i < Math.Min(x.Length, y.Length); i++)
            {
                int compareResult = charValues[x[i]].CompareTo(charValues[y[i]]);
                if (compareResult != 0)
                {
                    return compareResult;
                }
            }

            return x.Length.CompareTo(y.Length);
        }
    }
}
