using System;
using System.Collections.Generic;
using System.Linq;

namespace Evo.Mono.Classes.PathFinding;

public class Grid
{
    public IEnumerable<Node> Nodes { get; private set; }

    public Node this[int x, int y] => GetNodeAt(x, y);


    public Grid(int width, int height)
    {
        Nodes = Enumerable.Range(0, width)
            .SelectMany(x => Enumerable.Range(0, height)
                .Select(y => new Node(x, y)))
            .ToList();
    }

    private Node GetNodeAt(int x, int y)
    {
        return Nodes.FirstOrDefault(node => node.X == x && node.Y == y);
    }

    public void SetBorderNodesWalkable(bool walkable)
    {
        var borderNodes = Nodes.Where(node =>
                node.X == Nodes.Min(n => n.X) ||
                node.X == Nodes.Max(n => n.X) ||
                node.Y == Nodes.Min(n => n.Y) ||
                node.Y == Nodes.Max(n => n.Y))
            .ToList();

        foreach (var node in borderNodes)
        {
            node.Walkable = walkable;
        }
    }

    public List<Node> FindPath(Node startNode, Node endNode, bool allowDiagonalMovement = true)
    {
        var openList = new List<PathNode> { new PathNode(startNode) };
        var closedList = new HashSet<PathNode>();

        var pathfindingNodes = Nodes.Select(node => new PathNode(node)).ToList();

        var start = GetPathNode(startNode, pathfindingNodes);
        var end = GetPathNode(endNode, pathfindingNodes);

        start.G = 0;
        start.H = CalculateHeuristic(start, end);

        while (openList.Count > 0)
        {
            var currentNode = GetNodeWithLowestF(openList);

            if (currentNode.X == end.X && currentNode.Y == end.Y)
            {
                return ReconstructPath(currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            UpdateNeighbors(currentNode, end, pathfindingNodes, openList, closedList, allowDiagonalMovement);
        }

        return [];
    }

    #region FindPath_HelperMethods

    private void UpdateNeighbors(PathNode currentNode, PathNode endNode, List<PathNode> pathfindingNodes,
        List<PathNode> openList,
        HashSet<PathNode> closedList, bool allowDiagonalMovement)
    {
        foreach (var neighbor in GetNeighbors(currentNode, pathfindingNodes, allowDiagonalMovement))
        {
            if (!neighbor.Walkable || closedList.Contains(neighbor))
            {
                continue;
            }

            var tentativeG = GetMovementCost(currentNode, neighbor);

            if (tentativeG >= neighbor.G) continue;
            neighbor.Parent = currentNode;
            neighbor.G = tentativeG;
            neighbor.H = CalculateHeuristic(neighbor, endNode);

            if (!openList.Contains(neighbor))
            {
                openList.Add(neighbor);
            }
        }
    }

    private static PathNode GetPathNode(Node nodeToFind, List<PathNode> pathfindingNodes)
    {
        var start = pathfindingNodes.First(n => n.X == nodeToFind.X && n.Y == nodeToFind.Y);
        return start;
    }

    private static PathNode GetNodeWithLowestF(List<PathNode> openList)
    {
        var currentNode = openList.OrderBy(node => node.F).First();
        return currentNode;
    }

    private static int GetMovementCost(PathNode currentNode, PathNode neighborNode)
    {
        // 1.414 for diagonal movement
        var tentativeG = currentNode.G + (currentNode.X != neighborNode.X && currentNode.Y != neighborNode.Y ? 14 : 10);
        return tentativeG;
    }

    private static int CalculateHeuristic(PathNode node, PathNode endNode)
    {
        return Math.Abs(node.X - endNode.X) + Math.Abs(node.Y - endNode.Y);
    }

    private List<PathNode> GetNeighbors(PathNode node, List<PathNode> pathfindingNodes, bool allowDiagonalMovement)
    {
        var neighbors = new List<PathNode>();

        var directions = new List<(int x, int y)>
        {
            (-1, 0), (1, 0), (0, -1), (0, 1)
        };

        if (allowDiagonalMovement)
        {
            directions.AddRange(new (int x, int y)[]
            {
                (-1, -1), (1, -1), (-1, 1), (1, 1)
            });
        }

        foreach (var (dx, dy) in directions)
        {
            int neighborX = node.X + dx;
            int neighborY = node.Y + dy;

            var neighbor = pathfindingNodes.Find(n => n.X == neighborX && n.Y == neighborY);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private static List<Node> ReconstructPath(PathNode endNode)
    {
        var path = new List<Node>();
        var currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.BaseNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    #endregion
}