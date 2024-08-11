namespace Evo.Mono.Classes.Intention;

public class IntentionNone : IIntention
{
    public int UpdateTicks { get; private set; }
    
    public IntentionNone(){}

    public bool IsFulfilled()
    {
        return true;
    }

    public void Execute()
    {
        UpdateTicks++;
    }
}