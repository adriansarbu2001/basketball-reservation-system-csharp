using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Sockets;
using System.Threading;
using BasketballModel.validator;
using BasketballNetworking;
using BasketballPersistence;
using BasketballServices;
using log4net.Config;
using ServerTemplate;

namespace BasketballServer
{
    public class StartServer
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("App.config"));
            IDictionary<string, string> props = new SortedList<string, string>();
            props.Add("ConnectionString", GetConnectionStringByName("brsDB"));

            IAccountRepository accountRepository = new AccountDBRepository(props, new AccountValidator());
            IMatchRepository matchRepository = new MatchDBRepository(props, new MatchValidator());
            ITicketRepository ticketRepository = new TicketDBRepository(props, new TicketValidator());
            IBasketballService serviceImpl =
                new BasketballServiceImpl(accountRepository, matchRepository, ticketRepository);

            // IChatServer serviceImpl = new ChatServerImpl();
            SerialChatServer server = new SerialChatServer("127.0.0.1", 55556, serviceImpl);
            server.Start();
            Console.WriteLine("Server started ...");
            //Console.WriteLine("Press <enter> to exit...");
            Console.ReadLine();
        }

        static string GetConnectionStringByName(string name)
        {
            // Assume failure.
            string returnValue = null;

            // Look for the name in the connectionStrings section.
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];

            // If found, return the connection string.
            if (settings != null)
                returnValue = settings.ConnectionString;

            return returnValue;
        }
    }

    public class SerialChatServer : ConcurrentServer
    {
        private IBasketballService server;
        private BasketballClientObjectWorker worker;

        public SerialChatServer(string host, int port, IBasketballService server) : base(host, port)
        {
            this.server = server;
            Console.WriteLine("SerialChatServer...");
        }

        protected override Thread createWorker(TcpClient client)
        {
            worker = new BasketballClientObjectWorker(server, client);
            return new Thread(new ThreadStart(worker.run));
        }
    }
}