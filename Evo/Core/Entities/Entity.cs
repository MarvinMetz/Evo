using System;
using Microsoft.Xna.Framework;

namespace Evo.Core.Entities;

public class Entity
{
    public Guid Guid { get; } = Guid.NewGuid();
    public Vector2 Position { get; set; }
    public int Size { get; set; }

    public bool Debug { get; set; }

    public Vector2 TexturePosition => new(Position.X - Size / 2f, Position.Y - Size / 2f);
}