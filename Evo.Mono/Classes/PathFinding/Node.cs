using System;
using Microsoft.Xna.Framework;

namespace Evo.Mono.Classes.PathFinding;

public class Node
{
    public Point Position { get; init; }
    public bool Walkable { get; set; }
    public int X => Position.X;
    public int Y => Position.Y;

    public Node(int x, int y)
    {
        Position = new Point(x, y);
        Walkable = true;
    }
}

public class PathNode
{
    public Node BaseNode { get; private set; }
    public int G { get; set; }
    public int H { get; set; }
    public int F => G + H;
    public PathNode Parent { get; set; }

    public int X => BaseNode.X;
    public int Y => BaseNode.Y;
    public bool Walkable => BaseNode.Walkable;

    public PathNode(Node baseNode)
    {
        BaseNode = baseNode;
        G = int.MaxValue;
        H = 0;
        Parent = null;
    }

    public override bool Equals(object obj)
    {
        if (obj is PathNode other)
        {
            return X == other.X && Y == other.Y;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}