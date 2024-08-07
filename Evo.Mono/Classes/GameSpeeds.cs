using System;

namespace Evo.Mono.Classes;

public enum GameSpeeds
{
    Pause = 0,
    Slow = 1,
    Normal = 2,
    Fast = 3,
    UltraFast = 4
}

internal static class GameSpeedsMethods
{
    private const double Pause = 0.0;
    private const double Slow = 2.0;
    private const double Normal = 1.0;
    private const double Fast = 0.5;
    private const double UltraFast = 0.25;
    private const string PauseText = "Pause";
    private const string SlowText = "Slow";
    private const string NormalText = "Normal";
    private const string FastText = "Fast";
    private const string UltraFastText = "Fastest";
    private const string PauseTextAlt = "0x";
    private const string SlowTextAlt = "0.5x";
    private const string NormalTextAlt = "1x";
    private const string FastTextAlt = "2x";
    private const string UltraFastTextAlt = "4x";

    public static double GetSpeedValue(this GameSpeeds gameSpeed)
    {
        return gameSpeed switch
        {
            GameSpeeds.Pause => Pause,
            GameSpeeds.Slow => Slow,
            GameSpeeds.Normal => Normal,
            GameSpeeds.Fast => Fast,
            GameSpeeds.UltraFast => UltraFast,
            _ => throw new ArgumentOutOfRangeException(nameof(gameSpeed), gameSpeed, null)
        };
    }

    public static string ToString(this GameSpeeds gameSpeed)
    {
        return gameSpeed switch
        {
            GameSpeeds.Pause => PauseText,
            GameSpeeds.Slow => SlowText,
            GameSpeeds.Normal => NormalText,
            GameSpeeds.Fast => FastText,
            GameSpeeds.UltraFast => UltraFastText,
            _ => throw new ArgumentOutOfRangeException(nameof(gameSpeed), gameSpeed, null)
        };
    }

    public static string ToAlternativeString(this GameSpeeds gameSpeed)
    {
        return gameSpeed switch
        {
            GameSpeeds.Pause => PauseTextAlt,
            GameSpeeds.Slow => SlowTextAlt,
            GameSpeeds.Normal => NormalTextAlt,
            GameSpeeds.Fast => FastTextAlt,
            GameSpeeds.UltraFast => UltraFastTextAlt,
            _ => throw new ArgumentOutOfRangeException(nameof(gameSpeed), gameSpeed, null)
        };
    }

    public static GameSpeeds GetNextGameSpeed(this GameSpeeds gameSpeed)
    {
        return gameSpeed switch
        {
            GameSpeeds.Pause => GameSpeeds.Slow,
            GameSpeeds.Slow => GameSpeeds.Normal,
            GameSpeeds.Normal => GameSpeeds.Fast,
            GameSpeeds.Fast => GameSpeeds.UltraFast,
            GameSpeeds.UltraFast => GameSpeeds.UltraFast,
            _ => throw new ArgumentOutOfRangeException(nameof(gameSpeed), gameSpeed, null)
        };
    }

    public static GameSpeeds GetPreviousGameSpeed(this GameSpeeds gameSpeed)
    {
        return gameSpeed switch
        {
            GameSpeeds.Pause => GameSpeeds.Pause,
            GameSpeeds.Slow => GameSpeeds.Pause,
            GameSpeeds.Normal => GameSpeeds.Slow,
            GameSpeeds.Fast => GameSpeeds.Normal,
            GameSpeeds.UltraFast => GameSpeeds.Fast,
            _ => throw new ArgumentOutOfRangeException(nameof(gameSpeed), gameSpeed, null)
        };
    }
}