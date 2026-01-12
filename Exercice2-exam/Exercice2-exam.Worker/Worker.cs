namespace Exercice2_exam.Worker;

public class Worker : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string mazeText = """
                                D . . # .
                                # # . . .
                                . # . # .
                                . . . # .
                                # # # # S
                                """;

        var maze = new Maze(mazeText);

        PrintMaze(maze);

        Console.WriteLine();
        Console.WriteLine("Voisins de (0,1) :");
        PrintNeighbours(maze, 0, 1);

        Console.WriteLine();
        Console.WriteLine("Voisins de (2,2) :");
        PrintNeighbours(maze, 2, 2);

        return Task.CompletedTask;
    }

    private static void PrintMaze(Maze maze)
    {
        for (int r = 0; r < maze.Grid.GetLength(0); r++)
        {
            for (int c = 0; c < maze.Grid.GetLength(1); c++)
            {
                if ((r, c) == maze.Start) Console.Write("D ");
                else if ((r, c) == maze.Exit) Console.Write("S ");
                else if (maze.Grid[r, c]) Console.Write("# ");
                else Console.Write(". ");
            }

            Console.WriteLine();
        }
    }

    private static void PrintNeighbours(Maze maze, int x, int y)
    {
        var neighbours = maze.GetNeighbours(x, y);

        foreach (var (nx, ny) in neighbours)
        {
            Console.WriteLine($"({nx},{ny})");
        }
    }
}