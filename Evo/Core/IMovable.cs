namespace Evo.Core;

public interface IMovable
{
    public void MoveTo(ITargetable target);
    public bool IsAt(ITargetable target, float distanceThreshold = 0.0f);
}