using Exercice2_exam.Worker;

namespace Exercice2.Tests;

public class MazeFillTests
{
    [Fact]
    // 1) Initialisation : la queue contient le départ avec distance 0
    public void Constructor_Should_EnqueueStart_WithDistance0()
    {
        const string mazeText = """
        D . .
        . . .
        . . S
        """;

        var maze = new Maze(mazeText);

        Assert.Single(maze.Frontier);
        var item = maze.Frontier.Peek();

        Assert.Equal(0, item.distance);
        Assert.Equal(maze.Start, (item.x, item.y));
    }

    
}