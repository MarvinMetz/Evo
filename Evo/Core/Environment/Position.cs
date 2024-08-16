using Evo.Core.Environment.Path;
using Evo.Core.Utils;
using Microsoft.Xna.Framework;

namespace Evo.Core.Environment;

public struct Position(Vector2 location, Degrees rotation) : ITargetable
{
    public Vector2 Location { get; } = location;
    public Degrees Rotation { get; } = rotation;

    public Position(Vector2 location) : this(location, 0f)
    {
    }
    
    public Vector2 GetVelocity(Position previousPosition) => Location - previousPosition.Location;
    public Degrees GetRotationVelocity(Position previousPosition) => Rotation - previousPosition.Rotation;
    public Node GetNodeIn(World world) => world.GetNodeAt(this);

    public static Position operator +(Position a, Position b)
        => new(a.Location + b.Location, a.Rotation + b.Rotation);

    public static Position operator -(Position a, Position b)
        => new(a.Location - b.Location, a.Rotation - b.Rotation);

    public static Position operator +(Position a, Vector2 b)
        => new(a.Location + b, a.Rotation);

    public static Position operator -(Position a, Vector2 b)
        => new(a.Location - b, a.Rotation);

    public static Position operator +(Vector2 b, Position a)
        => new(a.Location + b, a.Rotation);

    public static Position operator -(Vector2 b, Position a)
        => new(a.Location - b, a.Rotation);

    public static Position operator +(Position a, Degrees b)
        => new(a.Location, a.Rotation + b);

    public static Position operator -(Position a, Degrees b)
        => new(a.Location, a.Rotation - b);

    public static Position operator +(Degrees b, Position a)
        => new(a.Location, a.Rotation + b);

    public static Position operator -(Degrees b, Position a)
        => new(a.Location, a.Rotation - b);
}