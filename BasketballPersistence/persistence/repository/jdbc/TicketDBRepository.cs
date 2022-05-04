using System;
using System.Collections.Generic;
using System.Data;
using BasketballModel;
using BasketballModel.validator;
using log4net;

namespace BasketballPersistence
{
    public class TicketDBRepository : ITicketRepository
    {
        private static readonly ILog log = LogManager.GetLogger("MatchDBRepository");

        IDictionary<string, string> props;
        IValidator<Ticket> validator;

        public TicketDBRepository(IDictionary<string, string> props, IValidator<Ticket> validator)
        {
            log.Info("Creating TicketDBRepository ");
            this.props = props;
            this.validator = validator;
        }

        public void delete(long id)
        {
            log.InfoFormat("Entering TicketDBRepository delete with value {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "delete from Tickets where ticket_id=@ticket_id";
                IDbDataParameter paramId = comm.CreateParameter();
                paramId.ParameterName = "@ticket_id";
                paramId.Value = id;
                comm.Parameters.Add(paramId);
                var dataR = comm.ExecuteNonQuery();
                if (dataR == 0)
                    throw new RepositoryException("No ticket deleted!");
            }
            log.InfoFormat("Exiting TicketDBRepository delete with value {0}", null);
        }

        public IEnumerable<Ticket> findAll()
        {
            log.InfoFormat("Entering TicketDBRepository findAll with value {0}", null);
            IDbConnection con = DBUtils.getConnection(props);
            IList<Ticket> ticketsR = new List<Ticket>();
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "select * from Tickets";

                using (var dataR = comm.ExecuteReader())
                {
                    while (dataR.Read())
                    {
                        long ticket_id = dataR.GetInt64(0);
                        String client_name = dataR.GetString(1);
                        int no_seats = dataR.GetInt32(2);
                        long match_id = dataR.GetInt64(3);
                        Ticket ticket = new Ticket(ticket_id, client_name, no_seats, match_id);
                        ticketsR.Add(ticket);
                    }
                }
            }
            log.InfoFormat("Exiting TicketDBRepository findAll with value {0}", null);
            return ticketsR;
        }

        public Ticket findOne(long id)
        {
            log.InfoFormat("Entering TicketDBRepository findOne with value {0}", id);
            IDbConnection con = DBUtils.getConnection(props);

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "select * from Tickets where ticket_id=@ticket_id";
                IDbDataParameter paramId = comm.CreateParameter();
                paramId.ParameterName = "@ticket_id";
                paramId.Value = id;
                comm.Parameters.Add(paramId);

                using (var dataR = comm.ExecuteReader())
                {
                    if (dataR.Read())
                    {
                        long ticket_id = dataR.GetInt64(0);
                        String client_name = dataR.GetString(1);
                        int no_seats = dataR.GetInt32(2);
                        long match_id = dataR.GetInt64(3);
                        Ticket ticket = new Ticket(ticket_id, client_name, no_seats, match_id);
                        return ticket;
                    }
                }
            }
            log.InfoFormat("Exiting TicketDBRepository findOne with value {0}", null);
            throw new RepositoryException("User not found!");
        }

        public void save(Ticket entity)
        {
            log.InfoFormat("Entering TicketDBRepository save with entity {0}", entity.ToString());
            var con = DBUtils.getConnection(props);

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "insert into Tickets(client_name, no_seats, match_id) values (@client_name, @no_seats, @match_id)";

                var paramClient = comm.CreateParameter();
                paramClient.ParameterName = "@client_name";
                paramClient.Value = entity.ClientName;
                comm.Parameters.Add(paramClient);

                var paramSeats = comm.CreateParameter();
                paramSeats.ParameterName = "@no_seats";
                paramSeats.Value = entity.NoSeats;
                comm.Parameters.Add(paramSeats);

                var paramMatch = comm.CreateParameter();
                paramMatch.ParameterName = "@match_id";
                paramMatch.Value = entity.MatchId;
                comm.Parameters.Add(paramMatch);

                var result = comm.ExecuteNonQuery();
                if (result == 0)
                    throw new RepositoryException("No ticket added!");
                log.InfoFormat("Exiting TicketDBRepository save with value {0}", result);
            }
        }

        public void update(Ticket entity)
        {
            log.InfoFormat("Entering TicketDBRepository update with entity {0}", entity.ToString());
            var con = DBUtils.getConnection(props);

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "update Tickets set client_name = @client_name, no_seats = @no_seats, match_id = @match_id where ticket_id = @ticket_id";

                var paramClientName = comm.CreateParameter();
                paramClientName.ParameterName = "@client_name";
                paramClientName.Value = entity.ClientName;
                comm.Parameters.Add(paramClientName);

                var paramNoSeats = comm.CreateParameter();
                paramNoSeats.ParameterName = "@no_seats";
                paramNoSeats.Value = entity.NoSeats;
                comm.Parameters.Add(paramNoSeats);

                var paramMatchId = comm.CreateParameter();
                paramMatchId.ParameterName = "@match_id";
                paramMatchId.Value = entity.MatchId;
                comm.Parameters.Add(paramMatchId);

                var paramid = comm.CreateParameter();
                paramid.ParameterName = "@ticket_id";
                paramid.Value = entity.TicketId;
                comm.Parameters.Add(paramid);

                var result = comm.ExecuteNonQuery();
                if (result == 0)
                    throw new RepositoryException("No ticket updated!");
                log.InfoFormat("Exiting TicketDBRepository update with value {0}", result);
            }
        }
    }
}
