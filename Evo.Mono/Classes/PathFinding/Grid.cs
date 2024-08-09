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
            node.X == 0 || node.X == Nodes.Max(n => n.X) - 1 || node.Y == 0 ||
            node.Y == Nodes.Max(n => n.Y) - 1).ToList();

        foreach (var node in borderNodes)
        {
            node.Walkable = walkable;
        }
    }

    public List<Node> FindPath(Node startNode, Node endNode)
    {
        var openList = new List<PathNode> { new PathNode(startNode) };
        var closedList = new HashSet<PathNode>();

        var pathfindingNodes = Nodes.Select(node => new PathNode(node)).ToList();

        var start = pathfindingNodes.First(n => n.X == startNode.X && n.Y == startNode.Y);
        var end = pathfindingNodes.First(n => n.X == endNode.X && n.Y == endNode.Y);

        start.G = 0;
        start.H = CalculateHeuristic(start, end);

        while (openList.Count > 0)
        {
            var currentNode = openList.OrderBy(node => node.F).First();

            if (currentNode.X == end.X && currentNode.Y == end.Y)
            {
                return ReconstructPath(currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbor in GetNeighbors(currentNode, pathfindingNodes))
            {
                if (!neighbor.Walkable || closedList.Contains(neighbor))
                {
                    continue;
                }

                // 1.414 for diagonal movement
                int tentativeG = currentNode.G + (currentNode.X != neighbor.X && currentNode.Y != neighbor.Y ? 14 : 10);

                if (tentativeG < neighbor.G)
                {
                    neighbor.Parent = currentNode;
                    neighbor.G = tentativeG;
                    neighbor.H = CalculateHeuristic(neighbor, end);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return [];
    }

    private List<PathNode> GetNeighbors(PathNode node, List<PathNode> pathfindingNodes)
    {
        var neighbors = new List<PathNode>();

        var directions = new (int x, int y)[]
        {
            (-1, 0), (1, 0), (0, -1), (0, 1), // Up, Down, Left, Right
            (-1, -1), (1, -1), (-1, 1), (1, 1) // Diagonals
        };

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

    private int CalculateHeuristic(PathNode node, PathNode endNode)
    {
        return Math.Abs(node.X - endNode.X) + Math.Abs(node.Y - endNode.Y);
    }

    private List<Node> ReconstructPath(PathNode endNode)
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
}