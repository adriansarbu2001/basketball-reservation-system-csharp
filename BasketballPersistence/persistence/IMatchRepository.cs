using System.Collections.Generic;
using BasketballModel;

namespace BasketballPersistence
{
    public interface IMatchRepository : IRepository<long, Match>
    {
        IEnumerable<Match> availableMatchesDescending();
    }
}
