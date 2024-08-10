using System;
using Microsoft.Xna.Framework;

namespace Evo.Mono.Classes;

public readonly struct Degrees
{
    private readonly float _value;

    public Degrees(float value)
    {
        _value = Normalize(value);
    }

    private static float Normalize(float value)
    {
        value %= 360;
        switch (value)
        {
            case > 180:
                value -= 360;
                break;
            case <= -180:
                value += 360;
                break;
        }
        return value;
    }

    public static Degrees FromVector2(Vector2 vector)
    {
        return new Degrees(MathHelper.ToDegrees((float)Math.Atan2(vector.Y, vector.X)));
    }

    public override string ToString()
        => _value + "d";

    public float ToRadians()
    {
        return (float)Math.PI / 180f * _value;
    }

    public Vector2 ToVector2()
    {
        var vector = new Vector2((float)Math.Cos(ToRadians()), (float)Math.Sin(ToRadians()));
        vector.Normalize();
        return vector;
    }

    public static implicit operator Degrees(float f)
        => new (f);

    public static implicit operator float(Degrees d)
        => d._value;

    public static implicit operator double(Degrees d)
        => d._value;

    public static Degrees operator +(Degrees a, Degrees b)
        => new (a._value + b._value);

    public static Degrees operator -(Degrees a, Degrees b)
        => new (a._value - b._value);
}