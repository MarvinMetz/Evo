using System.Collections.Generic;

namespace Evo.Mono.Classes.Intention;

public interface IIntentionManager
{
    public void FindNextIntention();
    public IEnumerable<IIntention> GetAvailableIntentions();
    public IIntention GetCurrentIntention();
}