using System;
using System.Collections;
using System.Collections.Generic;
using BasketballModel;
using BasketballServices;

namespace BasketballClient
{
    public class BasketballClientCtrl : IBasketballObserver
    {
        public event EventHandler<BasketballUserEventArgs> updateEvent;
        private readonly IBasketballService server;
        private Account currentUser;

        public BasketballClientCtrl(IBasketballService server)
        {
            this.server = server;
            currentUser = null;
        }

        public void login(String username, String password)
        {
            Account account = new Account(username, password);
            currentUser = server.login(account, this);
            Console.WriteLine("Ctrl Login succeeded ...");
            Console.WriteLine("Current user {0}", currentUser);
        }

        public void logout()
        {
            Console.WriteLine("Ctrl logout");
            server.logout(currentUser, this);
            currentUser = null;
        }

        public IEnumerable<Match> findAll()
        {
            return server.findAll();
        }

        public void saveTicket(string client_name, string no_seats, string match_id)
        {
            server.saveTicket(client_name, no_seats, match_id);
        }

        public IEnumerable<Match> availableMatchesDescending()
        {
            return server.availableMatchesDescending();
        }

        public void ticketSold()
        {
            Console.WriteLine("Ticket sold");
            BasketballUserEventArgs userEventArgs = new BasketballUserEventArgs(BasketballUserEvent.TicketSold, null);
            OnUserEvent(userEventArgs);
        }

        protected virtual void OnUserEvent(BasketballUserEventArgs e)
        {
            if (updateEvent == null) return;
            updateEvent(this, e);
            Console.WriteLine("Update Event called");
        }
    }
}
