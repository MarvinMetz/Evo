using Microsoft.Xna.Framework.Input;

namespace Evo.Mono.Classes;

public static class KeyInput
{
    private static KeyboardState _currentState;
    private static KeyboardState _previousState;

    public static KeyboardState GetState()
    {
        _previousState = _currentState;
        _currentState = Keyboard.GetState();
        return _currentState;
    }
    
    public static bool IsPressed(Keys key)
    {
        return _currentState.IsKeyDown(key);
    }

    public static bool HasBeenPressed(Keys key)
    {
        return _currentState.IsKeyDown(key) && !_previousState.IsKeyDown(key);
    }
}