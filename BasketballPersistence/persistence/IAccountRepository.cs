using System;
using BasketballModel;

namespace BasketballPersistence
{
    public interface IAccountRepository : IRepository<long, Account>
    {
        Account findBy(String username, String password);
    }
}
