using Exercice2_exam.Worker;

namespace Exercice2.Tests;

public class MazeParsingTests
{
    private const string ExampleMaze = """
                                       D . . # .
                                       # # . . .
                                       . # . # .
                                       . . . # .
                                       # # # # S
                                       """;

    [Fact]
    // A - Start/Exit + interprétation murs/vides
    public void Constructor_Should_SetStartAndExit_And_ParseWallsCorrectly()
    {
        var maze = new Maze(ExampleMaze);

        Assert.Equal((0, 0), maze.Start);
        Assert.Equal((4, 4), maze.Exit);

        Assert.False(maze.Grid[0, 0]);
        Assert.False(maze.Grid[0, 1]);
        Assert.True(maze.Grid[0, 3]);
        Assert.True(maze.Grid[1, 0]);
        Assert.False(maze.Grid[1, 2]);
        Assert.True(maze.Grid[2, 1]);
        Assert.False(maze.Grid[4, 4]);
    }

    [Fact]
    // B - Distances même taille que Grid et initialisées à 0
    public void Constructor_Should_InitDistances_WithSameSizeAsGrid_And_AllZeros()
    {
        var maze = new Maze(ExampleMaze);

        Assert.Equal(maze.Grid.GetLength(0), maze.Distances.GetLength(0));
        Assert.Equal(maze.Grid.GetLength(1), maze.Distances.GetLength(1));

        for (int r = 0; r < maze.Distances.GetLength(0); r++)
        {
            for (int c = 0; c < maze.Distances.GetLength(1); c++)
            {
                Assert.Equal(0, maze.Distances[r, c]);
            }
        }
    }
}