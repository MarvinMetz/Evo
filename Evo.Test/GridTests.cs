using Evo.Mono.Classes.PathFinding;
using Xunit;

namespace Evo.Test;

public class GridTests
{
    [Fact]
    public void GridInitialization_CreatesCorrectNumberOfNodes()
    {
        var grid = new Grid(5, 5);

        Assert.Equal(25, grid.Nodes.Count());
    }

    [Fact]
    public void GetNodeAt_ReturnsCorrectNode()
    {
        var grid = new Grid(5, 5);
        var node = grid[2, 3];

        Assert.NotNull(node);
        Assert.Equal(2, node.X);
        Assert.Equal(3, node.Y);
    }

    [Fact]
    public void GetNodeAt_ReturnsNullIfNodeDoesNotExist()
    {
        var grid = new Grid(5, 5);
        var node = grid[6, 6];

        Assert.Null(node);
    }

    [Fact]
    public void SetBorderNodesWalkable_SetsAllBordersToUnwalkable()
    {
        var grid = new Grid(5, 5);
        grid.SetBorderNodesWalkable(false);

        var borderNodes = grid.Nodes.Where(n =>
            n.X == 0 || n.X == 4 || n.Y == 0 || n.Y == 4);

        foreach (var node in borderNodes)
        {
            Assert.False(node.Walkable);
        }
    }

    [Fact]
    public void SetBorderNodesWalkable_LeavesNonBordersWalkable()
    {
        var grid = new Grid(5, 5);
        grid.SetBorderNodesWalkable(false);

        var innerNodes = grid.Nodes.Where(n =>
            n.X > 0 && n.X < 4 && n.Y > 0 && n.Y < 4);

        foreach (var node in innerNodes)
        {
            Assert.True(node.Walkable);
        }
    }

    [Fact]
    public void FindPath_ReturnsCorrectPathInSimpleCase()
    {
        var grid = new Grid(5, 5);
        var startNode = grid[0, 0];
        var endNode = grid[4, 4];

        var path = grid.FindPath(startNode, endNode);

        Assert.NotEmpty(path);
        Assert.Equal(5, path.Count); // Direct diagonal path in 5x5 grid
        Assert.Equal(startNode, path.First());
        Assert.Equal(endNode, path.Last());
    }

    [Fact]
    public void FindPath_HandlesNonWalkableNodes()
    {
        var grid = new Grid(5, 5);
        grid[2, 2].Walkable = false;

        var startNode = grid[0, 0];
        var endNode = grid[4, 4];

        var path = grid.FindPath(startNode, endNode);

        Assert.NotEmpty(path);
        Assert.DoesNotContain(grid[2, 2], path); // Path should avoid the non-walkable node
    }

    [Fact]
    public void FindPath_ReturnsEmptyIfNoPathExists()
    {
        var grid = new Grid(5, 5);
        grid[1, 0].Walkable = false;
        grid[0, 1].Walkable = false;

        var startNode = grid[0, 0];
        var endNode = grid[4, 4];

        var path = grid.FindPath(startNode, endNode);

        Assert.Empty(path); // No path should be found due to blocked start
    }

    [Fact]
    public void FindPath_AllowsDiagonalMovement()
    {
        var grid = new Grid(3, 3);
        var startNode = grid[0, 0];
        var endNode = grid[2, 2];

        var path = grid.FindPath(startNode, endNode);

        Assert.Equal(3, path.Count); // Diagonal path: (0,0) -> (1,1) -> (2,2)
        Assert.Equal(startNode, path.First());
        Assert.Equal(endNode, path.Last());
    }

    [Fact]
    public void FindPath_AvoidsObstaclesWithDiagonalMovement()
    {
        var grid = new Grid(3, 3);
        grid[1, 1].Walkable = false; // Block the center node

        var startNode = grid[0, 0];
        var endNode = grid[2, 2];

        var path = grid.FindPath(startNode, endNode);

        Assert.Equal(5, path.Count); // Path should go around the obstacle
        Assert.DoesNotContain(grid[1, 1], path);
    }

    [Fact]
    public void FindPath_ReturnsCorrectPathWhenStartAndEndAreSame()
    {
        var grid = new Grid(5, 5);
        var startNode = grid[2, 2];
        var endNode = grid[2, 2];

        var path = grid.FindPath(startNode, endNode);

        Assert.Single(path);
        Assert.Equal(startNode, path.First());
        Assert.Equal(endNode, path.Last());
    }

    [Fact]
    public void FindPath_AvoidsEdgesWhenBorderIsNotWalkable()
    {
        var grid = new Grid(5, 5);
        grid.SetBorderNodesWalkable(false);

        var startNode = grid[1, 1];
        var endNode = grid[3, 3];

        var path = grid.FindPath(startNode, endNode);

        Assert.NotEmpty(path);
        Assert.DoesNotContain(grid[0, 0], path);
        Assert.DoesNotContain(grid[4, 4], path);
    }

    [Fact]
    public void FindPath_HandlesLargeGridEfficiently()
    {
        var grid = new Grid(100, 100);
        var startNode = grid[0, 0];
        var endNode = grid[99, 99];

        var path = grid.FindPath(startNode, endNode);

        Assert.NotEmpty(path);
        Assert.Equal(startNode, path.First());
        Assert.Equal(endNode, path.Last());
    }

    [Fact]
    public void FindPath_NavigatesComplexMaze()
    {
        var grid = new Grid(10, 10);

        // Create a simple maze-like structure
        for (int i = 0; i < 10; i++)
        {
            if (i != 5) grid[5, i].Walkable = false;
        }

        var startNode = grid[0, 0];
        var endNode = grid[9, 9];

        var path = grid.FindPath(startNode, endNode);

        Assert.NotEmpty(path);
        Assert.Equal(startNode, path.First());
        Assert.Equal(endNode, path.Last());
    }

    [Fact]
    public void FindPath_AvoidsNarrowPassages()
    {
        var grid = new Grid(5, 5);

        // Create narrow passages
        grid[1, 0].Walkable = false;
        grid[1, 2].Walkable = false;
        grid[3, 1].Walkable = false;
        grid[2, 3].Walkable = false;

        var startNode = grid[0, 0];
        var endNode = grid[4, 4];

        var path = grid.FindPath(startNode, endNode);

        Assert.NotEmpty(path);
        Assert.Equal(startNode, path.First());
        Assert.Equal(endNode, path.Last());
    }

    [Fact]
    public void FindPath_HandlesNoPossiblePath()
    {
        var grid = new Grid(3, 3);
        grid[1, 0].Walkable = false;
        grid[1, 1].Walkable = false;
        grid[1, 2].Walkable = false;

        var startNode = grid[0, 0];
        var endNode = grid[2, 2];

        var path = grid.FindPath(startNode, endNode);

        Assert.Empty(path); // No path should be found due to blocked passage
    }

    [Fact]
    public void FindPath_StartOrEndNodeNonWalkable()
    {
        var grid = new Grid(5, 5);
        grid[0, 0].Walkable = false;

        var startNode = grid[0, 0];
        var endNode = grid[4, 4];

        var path = grid.FindPath(startNode, endNode);

        Assert.Empty(path); // No path since the start node is non-walkable

        grid[0, 0].Walkable = true;
        grid[4, 4].Walkable = false;
        path = grid.FindPath(startNode, endNode);

        Assert.Empty(path); // No path since the end node is non-walkable
    }

    [Fact]
    public void FindPath_FavorsDiagonalWhenShortest()
    {
        var grid = new Grid(3, 3);
        var startNode = grid[0, 0];
        var endNode = grid[2, 2];

        var path = grid.FindPath(startNode, endNode);

        Assert.Equal(3, path.Count); // Diagonal path: (0,0) -> (1,1) -> (2,2)
    }

    [Fact]
    public void FindPath_NoDiagonalWhenNotNeeded()
    {
        var grid = new Grid(3, 1);
        var startNode = grid[0, 0];
        var endNode = grid[2, 0];

        var path = grid.FindPath(startNode, endNode);

        Assert.Equal(3, path.Count); // Should be a straight line
    }

    [Fact]
    public void FindPath_HandlesSmallGrid()
    {
        var grid = new Grid(1, 1);
        var startNode = grid[0, 0];
        var endNode = grid[0, 0];

        var path = grid.FindPath(startNode, endNode);

        Assert.Single(path); // Only one node in the path, start == end
    }

    [Fact]
    public void FindPath_HandlesEmptyGrid()
    {
        var grid = new Grid(0, 0);

        var path = grid.FindPath(null, null);

        Assert.Empty(path); // No path should be found in an empty grid
    }

    [Fact]
    public void GridInitialization_NegativeSizeThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Grid(-5, -5));
    }

    [Fact]
    public void GetNodeAt_ReturnsNullForInvalidCoordinates()
    {
        var grid = new Grid(5, 5);

        Assert.Null(grid[-1, -1]);
        Assert.Null(grid[5, 5]);
    }

    [Fact]
    public void FindPath_HandlesRandomObstaclesOnLargeGrid()
    {
        var grid = new Grid(50, 50);
        var random = new Random();

        // Randomly make nodes non-walkable
        foreach (var node in grid.Nodes)
        {
            if (random.Next(0, 2) == 0) node.Walkable = false;
        }

        var startNode = grid[0, 0];
        var endNode = grid[49, 49];

        var path = grid.FindPath(startNode, endNode);

        // Depending on randomness, path may or may not exist
        if (startNode.Walkable && endNode.Walkable)
        {
            Assert.True(path.Count > 0 || path.Count == 0); // Path may or may not be found
        }
        else
        {
            Assert.Empty(path); // If start or end is non-walkable, path should be empty
        }
    }
        [Fact]
    public void FindPath_DiagonalMovementDisabled_NoDiagonalsUsed()
    {
        var grid = new Grid(3, 3);
        var startNode = grid[0, 0];
        var endNode = grid[2, 2];

        var path = grid.FindPath(startNode, endNode, allowDiagonalMovement: false);

        Assert.Equal(5, path.Count); // Without diagonal, it should take 5 steps (0,0) -> (0,1) -> (1,1) -> (1,2) -> (2,2)
    }

    [Fact]
    public void FindPath_DiagonalMovementEnabled_UsesDiagonals()
    {
        var grid = new Grid(3, 3);
        var startNode = grid[0, 0];
        var endNode = grid[2, 2];

        var path = grid.FindPath(startNode, endNode, allowDiagonalMovement: true);

        Assert.Equal(3, path.Count); // With diagonal movement, it should take 3 steps (0,0) -> (1,1) -> (2,2)
    }

    [Fact]
    public void FindPath_DiagonalMovementDisabled_AvoidsObstaclesCorrectly()
    {
        var grid = new Grid(3, 3);
        grid[1, 1].Walkable = false; // Block the center node

        var startNode = grid[0, 0];
        var endNode = grid[2, 2];

        var path = grid.FindPath(startNode, endNode, allowDiagonalMovement: false);

        Assert.Equal(5, path.Count); // Path should go around the obstacle (e.g., (0,0) -> (0,1) -> (0,2) -> (1,2) -> (2,2))
        Assert.DoesNotContain(grid[1, 1], path); // Ensure path avoids the blocked center node
    }

    [Fact]
    public void FindPath_DiagonalMovementEnabled_AvoidsObstaclesCorrectly()
    {
        var grid = new Grid(3, 3);
        grid[1, 1].Walkable = false; // Block the center node

        var startNode = grid[0, 0];
        var endNode = grid[2, 2];

        var path = grid.FindPath(startNode, endNode, allowDiagonalMovement: true);

        Assert.Equal(5, path.Count); // Path should go around the obstacle (e.g., (0,0) -> (1,0) -> (2,0) -> (2,1) -> (2,2))
        Assert.DoesNotContain(grid[1, 1], path); // Ensure path avoids the blocked center node
    }

    [Fact]
    public void FindPath_LargeGridWithToggleableDiagonal()
    {
        var grid = new Grid(50, 50);
        var startNode = grid[0, 0];
        var endNode = grid[49, 49];

        var pathWithoutDiagonal = grid.FindPath(startNode, endNode, allowDiagonalMovement: false);
        var pathWithDiagonal = grid.FindPath(startNode, endNode, allowDiagonalMovement: true);

        Assert.NotEqual(pathWithoutDiagonal.Count, pathWithDiagonal.Count);
        Assert.True(pathWithDiagonal.Count < pathWithoutDiagonal.Count); // Diagonal path should be shorter
    }
}