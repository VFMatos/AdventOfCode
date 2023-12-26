public class Program
{
    public static Dictionary<string, long> cache = new Dictionary<string, long>();

    public static void Main()
    {
        // Get the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();
        string relativePath = "Resources/MirrorInformation.txt";
        // Combine the current directory and relative path to get the full path
        string data = Path.Combine(currentDirectory, relativePath);

        string input = File.ReadAllText(data);

        var patterns = input.Split("\r\n\r\n");

        for (int i = 0; i <= 1; i++)
        {
            var total = 0;

            object lockObject = new object();

            Parallel.ForEach(patterns, pattern =>
            {
                int rowsAbove = 0;

                var columnsToLeft = IsMirroredVerticaly(pattern, i);

                if (columnsToLeft == 0)
                    rowsAbove = IsMirroredHorizontally(pattern, i);

                lock (lockObject)
                {
                    total += columnsToLeft + 100 * rowsAbove;
                }
            });

            Console.WriteLine($"Part {i + 1} result: {total}");
        }
    }

    public static int IsMirroredHorizontally(string pattern, int allowedDifference = 0)
    {
        var lines = pattern.Split("\r\n");

        var isMirrored = true;
        int rowsAbove = 0;

        for (int i = 1; i < lines.Count(); i++)
        {
            var differences = 0;
            isMirrored = true;
            var j = 0;

            while (isMirrored && (i - 1 - j) >= 0 && (i + j) < lines.Count())
            {
                var topLine = lines[i - 1 - j];
                var bottomLine = lines[i + j];

                differences += topLine.Zip(bottomLine, (c1, c2) => c1 != c2 ? 1 : 0).Sum();

                isMirrored = isMirrored && differences <= allowedDifference;

                if (isMirrored)
                {
                    j++;
                }
            }

            if (isMirrored && differences == allowedDifference)
            {
                //Console.WriteLine($"Is mirrored horizontally between rows {i} and {i+1} with {i} rows above");
                rowsAbove = i;
                break;
            }
        }

        if (!isMirrored)
        {
            //Console.WriteLine($"Is not mirrored horizontally");
            return 0;
        }
        return rowsAbove;
    }

    public static int IsMirroredVerticaly(string pattern, int allowedDifference = 0)
    {
        var lines = pattern.Split("\r\n");

        var isMirrored = true;
        int columnsToLeft = 0;

        for (int i = 1; i < lines[0].Length; i++)
        {
            object lockObject = new object();

            var range = Math.Min(i, lines[0].Length - i);
            var differences = 0;
            isMirrored = true;

            Parallel.ForEach(lines, (line, parallelLoopState) =>
            {
                lock (lockObject)
                {
                    var leftString = line.Substring(i - range, range);
                    var reversedRightString = new string(line.Substring(i, range).Reverse().ToArray());

                    differences += leftString.Zip(reversedRightString, (c1, c2) => c1 != c2 ? 1 : 0).Sum();

                    isMirrored = isMirrored && differences <= allowedDifference;
                }

                if (!isMirrored)
                    parallelLoopState.Break();
            });

            if (isMirrored && differences == allowedDifference)
            {
                //Console.WriteLine($"Is mirrored vertically between columns {i} and {i+1} with {i} columns on the left");
                columnsToLeft = i;
                break;
            }
        }

        if (!isMirrored)
        {
            //Console.WriteLine($"Is not mirrored vertically");
            return 0;
        }
        return columnsToLeft;
    }
}