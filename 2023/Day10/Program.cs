public class Program
{
    public static void Main()
    {
        // Get the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();
        string relativePath = "Resources/PipeMazeInformation.txt";
        // Combine the current directory and relative path to get the full path
        string pipeMazeData = Path.Combine(currentDirectory, relativePath);

        // Read all lines from the text document
        string[] lines = File.ReadAllLines(pipeMazeData);

        var nodes = new LinkedList<PipeMazeInformation>(lines
            .Select((line, index) => new PipeMazeInformation(index, line))
        );

        var currentNode = nodes.First;
        while (currentNode != null && !currentNode.Value.LineInfo.Contains('S'))
        {
            currentNode = currentNode.Next;
        }

        var currentIndex = currentNode.Value.LineInfo.IndexOf('S');
        currentNode.Value.LoopIndexes.Add(currentIndex);
        Direction fromDirection = Direction.None;

        // First step
        if (currentIndex + 1 < currentNode.Value.LineInfo.Length && (new char[] { '-', 'J', '7' }).Contains(currentNode.Value.LineInfo[currentIndex + 1]))
        {
            currentIndex++;
            fromDirection = Direction.Left;
        }
        else if (currentIndex - 1 >= 0 && (new char[] { '-', 'L', 'F' }).Contains(currentNode.Value.LineInfo[currentIndex - 1]))
        {
            currentIndex--;
            fromDirection = Direction.Right;
        }
        else if (currentNode.Next != null && (new char[] { '|', 'L', 'J' }).Contains(currentNode.Next.Value.LineInfo[currentIndex]))
        {
            currentNode = currentNode.Next;
            fromDirection = Direction.Up;
        }
        else if (currentNode.Previous != null && (new char[] { '|', '7', 'F' }).Contains(currentNode.Previous.Value.LineInfo[currentIndex]))
        {
            currentNode = currentNode.Previous;
            fromDirection = Direction.Down;
        }

        while (currentNode != null
            && currentIndex < currentNode.Value.LineInfo.Length
            && currentIndex >= 0
            && currentNode.Value.LineInfo[currentIndex] != 'S')
        {
            currentNode.Value.LoopIndexes.Add(currentIndex);

            fromCharacterTo.TryGetValue((fromDirection, currentNode.Value.LineInfo[currentIndex]), out Direction toDirection);

            switch (toDirection)
            {
                case Direction.Left:
                    currentIndex--;
                    fromDirection = Direction.Right;
                    break;
                case Direction.Right:
                    currentIndex++;
                    fromDirection = Direction.Left;
                    break;
                case Direction.Up:
                    currentNode = currentNode.Previous;
                    fromDirection = Direction.Down;
                    break;
                case Direction.Down:
                    currentNode = currentNode.Next;
                    fromDirection = Direction.Up;
                    break;
            }
        }

        Console.WriteLine($"Part 1 result: {(int)(nodes.Sum(x => x.LoopIndexes.Count()) / 2)}");
    }

    public static Dictionary<(Direction Origin, char character), Direction> fromCharacterTo =
        new Dictionary<(Direction, char), Direction>()
        {
            { (Direction.Up,'|'), Direction.Down},
            { (Direction.Down,'|'), Direction.Up},
            { (Direction.Left,'-'), Direction.Right},
            { (Direction.Right,'-'), Direction.Left},
            { (Direction.Up,'L'), Direction.Right},
            { (Direction.Right,'L'), Direction.Up},
            { (Direction.Up,'J'), Direction.Left},
            { (Direction.Left,'J'), Direction.Up},
            { (Direction.Left,'7'), Direction.Down},
            { (Direction.Down,'7'), Direction.Left},
            { (Direction.Down,'F'), Direction.Right},
            { (Direction.Right,'F'), Direction.Down},
        };

    public class PipeMazeInformation
    {
        public PipeMazeInformation(int lineNumber, string lineInfo)
        {
            LineNumber = lineNumber;
            LineInfo = lineInfo;
        }

        public int LineNumber { get; set; }
        public string LineInfo { get; set; }

        public List<int> LoopIndexes { get; set; } = new List<int>();
        public int CountInnerTiles { get; set; }
    }

    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right
    }
}
