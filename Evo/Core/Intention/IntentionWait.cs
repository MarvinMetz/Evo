namespace Evo.Core.Intention;

public class IntentionWait(int waitTicks) : IIntention
{
    public int UpdateTicks { get; private set; }
    public int WaitTicks { get; set; } = waitTicks;

    public bool IsFulfilled()
    {
        return WaitTicks <= 0;
    }

    public void Execute()
    {
        WaitTicks--;
        UpdateTicks++;
    }
}