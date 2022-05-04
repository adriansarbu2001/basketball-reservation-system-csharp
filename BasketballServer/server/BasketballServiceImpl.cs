using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BasketballModel;
using BasketballPersistence;
using BasketballServices;

namespace BasketballServer
{
    public class BasketballServiceImpl : IBasketballService
    {
        private IAccountRepository accountRepository;
        private IMatchRepository matchRepository;
        private ITicketRepository ticketRepository;
        private readonly IDictionary <long, IBasketballObserver> loggedClients;

        public BasketballServiceImpl(IAccountRepository accountRepository, IMatchRepository matchRepository,
            ITicketRepository ticketRepository)
        {
            this.accountRepository = accountRepository;
            this.matchRepository = matchRepository;
            this.ticketRepository = ticketRepository;
            loggedClients=new Dictionary<long, IBasketballObserver>();
        }

        public Account login(Account user, IBasketballObserver client)
        {
            Account userR;
            try
            {
                userR = accountRepository.findBy(user.Username, user.Password);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(ex.Message);
            }

            if (userR != null) {
                if(loggedClients.ContainsKey(userR.AccountId))
                    throw new ServiceException("User already logged in.");
                loggedClients[userR.AccountId]= client;
                Console.WriteLine(loggedClients.ToString());
                return userR;
            } else {
                throw new ServiceException("Authentication failed.");
            }
        }

        public void logout(Account user, IBasketballObserver client)
        {
            IBasketballObserver localClient = loggedClients[user.AccountId];
            if (localClient == null)
                throw new ServiceException("User " + user.AccountId + " is not logged in.");
            loggedClients.Remove(user.AccountId);
        }

        public void register(String username, String password)
        {
            Account account = new Account(username, password);
            try
            {
                accountRepository.save(account);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(ex.Message);
            }
        }

        public IEnumerable<Match> findAll()
        {
            return matchRepository.findAll();
        }

        public void saveMatch(String name, float ticket_price, int no_available_seats)
        {
            Match match = new Match(name, ticket_price, no_available_seats);
            try
            {
                matchRepository.save(match);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(ex.Message);
            }
        }

        public void deleteMatch(long match_id)
        {
            try
            {
                matchRepository.delete(match_id);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(ex.Message);
            }
        }

        public void updateMatch(long match_id, String name, float ticket_price, int no_available_seats)
        {
            Match match = new Match(match_id, name, ticket_price, no_available_seats);
            try
            {
                matchRepository.update(match);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(ex.Message);
            }
        }

        public void saveTicket(string client_name, string no_seats, string match_id)
        {
            int no_seats_int = 0;
            long match_id_long = -1L;
            try {
                no_seats_int = Int32.Parse(no_seats);
                match_id_long = Int64.Parse(match_id);
            } catch (FormatException e) {
                throw new ServiceException("empty text fields!");
            }

            Match match;
            try
            {
                match = matchRepository.findOne(match_id_long);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(ex.Message);
            }
            if (match.NoAvailableSeats - no_seats_int < 0) {
                throw new ServiceException("Not enough seats available!");
            }
            Ticket ticket = new Ticket(client_name, no_seats_int, match_id_long);
            ticketRepository.save(ticket);
            match.NoAvailableSeats -= no_seats_int;
            matchRepository.update(match);
            notifyTicketSold();
        }

        public IEnumerable<Match> availableMatchesDescending()
        {
            try
            {
                return matchRepository.availableMatchesDescending();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(ex.Message);
            }
        }
        
        private void notifyTicketSold() {
            foreach (long id in this.loggedClients.Keys)
            {
                IBasketballObserver chatClient=loggedClients[id];
                if (chatClient!=null)
                {
                    Task.Run(() =>
                    {
                        Console.WriteLine("Notifying [" + id + "] ticket sold.");
                        chatClient.ticketSold();
                    });
                }
            }
        }
    }
}
