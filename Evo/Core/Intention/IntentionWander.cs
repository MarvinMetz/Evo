namespace Evo.Core.Intention;

public class IntentionWander(IMovable wanderer, ITargetable target) : IIntention
{
    public int UpdateTicks { get; private set; }
    public IMovable Wanderer { get; } = wanderer;
    public ITargetable Target { get; } = target;
    public float FulfillmentThreshold { get; set; }

    public bool IsFulfilled()
    {
        return Wanderer.IsAt(Target);
    }

    public void Execute()
    {
        Wanderer.MoveTo(Target);
        UpdateTicks++;
    }
}