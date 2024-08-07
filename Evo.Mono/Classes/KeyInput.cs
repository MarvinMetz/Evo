using Microsoft.Xna.Framework.Input;

namespace Evo.Mono.Classes;

public class KeyInput
{
    private static KeyboardState currentState;
    private static KeyboardState previousState;

    public static KeyboardState GetState()
    {
        previousState = currentState;
        currentState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        return currentState;
    }
    
    public static bool IsPressed(Keys key)
    {
        return currentState.IsKeyDown(key);
    }

    public static bool HasBeenPressed(Keys key)
    {
        return currentState.IsKeyDown(key) && !previousState.IsKeyDown(key);
    }
}