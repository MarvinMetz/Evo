using System;
using Microsoft.Xna.Framework;

namespace Evo.Mono.Classes;

public class Entity
{
    public Guid Guid { get; } = Guid.NewGuid();
    public Vector2 Position { get; set; }
    public int Size { get; set; }

    public bool Debug = false;

    public Vector2 TexturePosition => new(Position.X - Size / 2, Position.Y - Size / 2);
}