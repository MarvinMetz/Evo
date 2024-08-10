using System;
using Microsoft.Xna.Framework;

namespace Evo.Mono.Classes.PathFinding;

public class Node(int x, int y)
{
    public Point Position { get; } = new(x, y);
    public bool Walkable { get; set; } = true;
    public int X => Position.X;
    public int Y => Position.Y;
}

public class PathNode(Node baseNode)
{
    public Node BaseNode { get; } = baseNode;
    public int G { get; set; } = int.MaxValue;
    public int H { get; set; }
    public int F => G + H;
    public PathNode Parent { get; set; }

    public int X => BaseNode.X;
    public int Y => BaseNode.Y;
    public bool Walkable => BaseNode.Walkable;

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