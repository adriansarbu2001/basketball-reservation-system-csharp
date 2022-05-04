using System;
using System.Collections.Generic;
using BasketballModel;

namespace BasketballServices
{
    public interface IBasketballService
    {
        Account login(Account user, IBasketballObserver client);

        void logout(Account user, IBasketballObserver client);

        IEnumerable<Match> findAll();

        void saveMatch(String name, float ticket_price, int no_available_seats);

        void deleteMatch(long match_id);

        void updateMatch(long match_id, String name, float ticket_price, int no_available_seats);

        void saveTicket(String client_name, String no_seats, String match_id);

        IEnumerable<Match> availableMatchesDescending();
    }
}
