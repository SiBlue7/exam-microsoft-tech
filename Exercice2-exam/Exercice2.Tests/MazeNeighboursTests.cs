using Exercice2_exam.Worker;

namespace Exercice2.Tests;

public class MazeNeighboursTests
{
    private const string ExampleMaze = """
    D . . # .
    # # . . .
    . # . # .
    . . . # .
    # # # # S
    """;

    [Fact]
// 1) Cas nominal : les 4 voisins existent et sont visitables
// Création d'un nouveau labyrinthe simple pour ce test
    public void GetNeighbours_Should_ReturnFourNeighbours_WhenAllFourAreAvailable()
    {
        const string mazeText = """
                                D . .
                                . . .
                                . . S
                                """;

        var maze = new Maze(mazeText);
        var n = maze.GetNeighbours(1, 1);

        Assert.Contains((0, 1), n);
        Assert.Contains((2, 1), n);
        Assert.Contains((1, 0), n);
        Assert.Contains((1, 2), n);
        Assert.Equal(4, n.Count);
    }

    [Fact]
    // 2) Ne doit pas inclure une case hors limites (haut)
    public void GetNeighbours_Should_NotIncludeOutOfBounds_Up()
    {
        var maze = new Maze(ExampleMaze);
        var n = maze.GetNeighbours(0, 1);

        Assert.DoesNotContain((-1, 1), n);
    }

    [Fact]
    // 3) Ne doit pas inclure une case hors limites (gauche)
    public void GetNeighbours_Should_NotIncludeOutOfBounds_Left()
    {
        var maze = new Maze(ExampleMaze);
        var n = maze.GetNeighbours(1, 0);

        Assert.DoesNotContain((1, -1), n);
    }

    [Fact]
    // 4) Ne doit pas inclure une case hors limites (bas)
    public void GetNeighbours_Should_NotIncludeOutOfBounds_Down()
    {
        var maze = new Maze(ExampleMaze);
        var n = maze.GetNeighbours(4, 3);

        Assert.DoesNotContain((5, 3), n);
    }

    [Fact]
    // 5) Ne doit pas inclure une case hors limites (droite)
    public void GetNeighbours_Should_NotIncludeOutOfBounds_Right()
    {
        var maze = new Maze(ExampleMaze);
        var n = maze.GetNeighbours(3, 4);

        Assert.DoesNotContain((3, 5), n);
    }

    [Fact]
    // 6) Ne doit pas inclure un mur
    public void GetNeighbours_Should_NotIncludeWalls()
    {
        var maze = new Maze(ExampleMaze);
        
        var n = maze.GetNeighbours(0, 2);

        Assert.DoesNotContain((0, 3), n);
    }

    [Fact]
    // 7) Ne doit pas inclure le départ
    public void GetNeighbours_Should_NotIncludeStartCell()
    {
        var maze = new Maze(ExampleMaze);

        var n = maze.GetNeighbours(0, 1);

        Assert.DoesNotContain(maze.Start, n);
        Assert.DoesNotContain((0, 0), n);
    }
}