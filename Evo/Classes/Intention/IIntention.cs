namespace Evo.Classes.Intention;

public interface IIntention
{
    public int UpdateTicks { get; }
    public bool IsFulfilled();
    public void Execute();
}