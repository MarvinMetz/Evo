using System.Collections.Generic;

namespace Evo.Classes.Intention;

public interface IIntentionManager
{
    public void FindNextIntention();
    public IEnumerable<IIntention> GetAvailableIntentions();
    public IIntention GetCurrentIntention();
}