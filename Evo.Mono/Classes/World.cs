using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Evo.Mono.Classes;

public class World
{
    public int Size { get; set; }

    public ICollection<Entity> Entities { get; set; } = [];

    public bool IsPositionOnWorld(float offset, params Vector2[] positions)
    {
        foreach (var position in positions)
            if (position.X < 0 + offset || position.X > Size - offset || position.Y < 0 + offset ||
                position.Y > Size - offset)
            {
                return false;
            }

        return true;
    }
}