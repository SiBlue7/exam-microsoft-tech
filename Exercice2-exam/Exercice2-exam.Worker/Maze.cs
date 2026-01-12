namespace Exercice2_exam.Worker;

public class Maze
{
    public bool[,] Grid { get; }
    public (int row, int col) Start { get; }
    public (int row, int col) Exit { get; }
    public int[,] Distances { get; }

    public Maze(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Maze input cannot be empty.", nameof(input));

        var lines = input
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var tokensByLine = lines
            .Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .ToArray();

        var rows = tokensByLine.Length;
        var cols = tokensByLine[0].Length;

        for (int r = 0; r < rows; r++)
        {
            if (tokensByLine[r].Length != cols)
                throw new ArgumentException("Maze must be rectangular (same number of columns on each row).");
        }

        Grid = new bool[rows, cols];
        Distances = new int[rows, cols];

        (int row, int col)? start = null;
        (int row, int col)? exit = null;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var cell = tokensByLine[r][c];

                switch (cell)
                {
                    case "#":
                        Grid[r, c] = true;
                        break;

                    case ".":
                        Grid[r, c] = false;
                        break;

                    case "D":
                        Grid[r, c] = false;
                        start = (r, c);
                        break;

                    case "S":
                        Grid[r, c] = false;
                        exit = (r, c);
                        break;

                    default:
                        throw new ArgumentException($"Unknown cell token '{cell}' at ({r},{c}).");
                }
            }
        }

        if (start is null) throw new ArgumentException("Maze must contain a start 'D'.");
        if (exit is null) throw new ArgumentException("Maze must contain an exit 'S'.");

        Start = start.Value;
        Exit = exit.Value;
    }
}