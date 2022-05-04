using System;
using System.Collections.Generic;
using System.Data;
using BasketballModel;
using BasketballModel.validator;
using log4net;

namespace BasketballPersistence
{
    public class MatchDBRepository : IMatchRepository
    {
        private static readonly ILog log = LogManager.GetLogger("MatchDBRepository");

        IDictionary<string, string> props;
        IValidator<Match> validator;

        public MatchDBRepository(IDictionary<string, string> props, IValidator<Match> validator)
        {
            log.Info("Creating MatchDBRepository");
            this.props = props;
            this.validator = validator;
        }

        public void delete(long id)
        {
            log.InfoFormat("Entering MatchDBRepository delete with value {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "delete from Matches where match_id=@match_id";
                IDbDataParameter paramId = comm.CreateParameter();
                paramId.ParameterName = "@match_id";
                paramId.Value = id;
                comm.Parameters.Add(paramId);
                var dataR = comm.ExecuteNonQuery();
                if (dataR == 0)
                    throw new RepositoryException("No match deleted!");
            }
            log.InfoFormat("Exiting MatchDBRepository delete with value {0}", null);
        }

        public IEnumerable<Match> findAll()
        {
            log.InfoFormat("Entering MatchDBRepository findAll with value {0}", null);
            IDbConnection con = DBUtils.getConnection(props);
            IList<Match> matchesR = new List<Match>();
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "select * from Matches";

                using (var dataR = comm.ExecuteReader())
                {
                    while (dataR.Read())
                    {
                        long match_id = dataR.GetInt64(0);
                        String name = dataR.GetString(1);
                        float ticket_price = dataR.GetFloat(2);
                        int no_available_seats = dataR.GetInt32(3);
                        Match match = new Match(match_id, name, ticket_price, no_available_seats);
                        matchesR.Add(match);
                    }
                }
            }
            log.InfoFormat("Exiting MatchDBRepository findAll with value {0}", null);
            return matchesR;
        }

        public Match findOne(long id)
        {
            log.InfoFormat("Entering MatchDBRepository findOne with value {0}", id);
            IDbConnection con = DBUtils.getConnection(props);

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "select * from Matches where match_id=@match_id";
                IDbDataParameter paramId = comm.CreateParameter();
                paramId.ParameterName = "@match_id";
                paramId.Value = id;
                comm.Parameters.Add(paramId);

                using (var dataR = comm.ExecuteReader())
                {
                    if (dataR.Read())
                    {
                        long match_id = dataR.GetInt64(0);
                        String name = dataR.GetString(1);
                        float ticket_price = dataR.GetFloat(2);
                        int no_available_seats = dataR.GetInt32(3);
                        Match match = new Match(match_id, name, ticket_price, no_available_seats);
                        return match;
                    }
                }
            }
            log.InfoFormat("Exiting MatchDBRepository findOne with value {0}", null);
            throw new RepositoryException("User not found!");
        }

        public void save(Match entity)
        {
            log.InfoFormat("Entering MatchDBRepository save with entity {0}", entity.ToString());
            var con = DBUtils.getConnection(props);

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "insert into Matches(name, ticket_price, no_available_seats) values (@name, @ticket_price, @no_available_seats)";

                var paramName = comm.CreateParameter();
                paramName.ParameterName = "@name";
                paramName.Value = entity.Name;
                comm.Parameters.Add(paramName);

                var paramPrice = comm.CreateParameter();
                paramPrice.ParameterName = "@ticket_price";
                paramPrice.Value = entity.TicketPrice;
                comm.Parameters.Add(paramPrice);

                var paramSeats = comm.CreateParameter();
                paramSeats.ParameterName = "@no_available_seats";
                paramSeats.Value = entity.NoAvailableSeats;
                comm.Parameters.Add(paramSeats);

                var result = comm.ExecuteNonQuery();
                if (result == 0)
                    throw new RepositoryException("No match added!");
                log.InfoFormat("Exiting MatchDBRepository save with value {0}", result);
            }
        }

        public void update(Match entity)
        {
            log.InfoFormat("Entering MatchDBRepository update with entity {0}", entity.ToString());
            var con = DBUtils.getConnection(props);

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "update Matches set name = @name, ticket_price = @ticket_price, no_available_seats = @no_available_seats where match_id = @match_id";

                var paramName = comm.CreateParameter();
                paramName.ParameterName = "@name";
                paramName.Value = entity.Name;
                comm.Parameters.Add(paramName);

                var paramPrice = comm.CreateParameter();
                paramPrice.ParameterName = "@ticket_price";
                paramPrice.Value = entity.TicketPrice;
                comm.Parameters.Add(paramPrice);

                var paramSeats = comm.CreateParameter();
                paramSeats.ParameterName = "@no_available_seats";
                paramSeats.Value = entity.NoAvailableSeats;
                comm.Parameters.Add(paramSeats);

                var paramId = comm.CreateParameter();
                paramId.ParameterName = "@match_id";
                paramId.Value = entity.MatchId;
                comm.Parameters.Add(paramId);

                var result = comm.ExecuteNonQuery();
                if (result == 0)
                    throw new RepositoryException("No match updated!");
                log.InfoFormat("Exiting MatchDBRepository update with value {0}", result);
            }
        }

        public IEnumerable<Match> availableMatchesDescending()
        {
            log.InfoFormat("Entering MatchDBRepository AvailableMatchesDescending with value {0}", null);
            IDbConnection con = DBUtils.getConnection(props);
            IList<Match> matchesR = new List<Match>();
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "select * from Matches where no_available_seats > 0 order by no_available_seats desc";

                using (var dataR = comm.ExecuteReader())
                {
                    while (dataR.Read())
                    {
                        long match_id = dataR.GetInt64(0);
                        String name = dataR.GetString(1);
                        float ticket_price = dataR.GetFloat(2);
                        int no_available_seats = dataR.GetInt32(3);
                        Match match = new Match(match_id, name, ticket_price, no_available_seats);
                        matchesR.Add(match);
                    }
                }
            }
            log.InfoFormat("Exiting MatchDBRepository availableMatchesDescending with value {0}", null);
            return matchesR;
        }
    }
}
