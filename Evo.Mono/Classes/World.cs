using System;
using System.Collections.Generic;
using Evo.Mono.Classes.PathFinding;
using Microsoft.Xna.Framework;

namespace Evo.Mono.Classes;

public class World
{
    public int WorldSize { get; }

    private readonly Grid _grid;
    private readonly int _gridResolution;

    public ICollection<Entity> Entities { get; set; } = [];


    public World(int worldSize, int gridResolution)
    {
        if (worldSize % gridResolution != 0)
            throw new ArgumentException("Grid resolution must divide world size without remainder.",
                nameof(gridResolution));

        WorldSize = worldSize;
        _gridResolution = gridResolution;

        var nodeAmount = WorldSize / _gridResolution;
        nodeAmount += 2; // Border

        _grid = new Grid(nodeAmount, nodeAmount);
        _grid.SetBorderNodesWalkable(false);
    }

    public bool IsPositionOnWorld(float offset, params Vector2[] positions)
    {
        foreach (var position in positions)
            if (position.X < 0 + offset || position.X > WorldSize || position.Y < 0 + offset ||
                position.Y > WorldSize)
            {
                return false;
            }

        return true;
    }
}