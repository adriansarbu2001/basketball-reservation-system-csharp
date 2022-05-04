using System;
using System.Collections.Generic;
using System.Data;
using BasketballModel;
using BasketballModel.validator;
using log4net;

namespace BasketballPersistence
{
    public class AccountDBRepository : IAccountRepository
    {
        private static readonly ILog log = LogManager.GetLogger("MatchDBRepository");

        IDictionary<string, string> props;
        IValidator<Account> validator;

        public AccountDBRepository(IDictionary<string, string> props, IValidator<Account> validator)
        {
            log.Info("Creating AccountDBRepository ");
            this.props = props;
            this.validator = validator;
        }

        public void delete(long id)
        {
            log.InfoFormat("Entering AccountDBRepository delete with value {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "delete from Accounts where account_id=@account_id";
                IDbDataParameter paramId = comm.CreateParameter();
                paramId.ParameterName = "@account_id";
                paramId.Value = id;
                comm.Parameters.Add(paramId);
                var dataR = comm.ExecuteNonQuery();
                if (dataR == 0)
                    throw new RepositoryException("No account deleted!");
            }
            log.InfoFormat("Exiting AccountDBRepository delete with value {0}", null);
        }

        public IEnumerable<Account> findAll()
        {
            log.InfoFormat("Entering AccountDBRepository findAll with value {0}", null);
            IDbConnection con = DBUtils.getConnection(props);
            IList<Account> accountsR = new List<Account>();
            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "select * from Accounts";

                using (var dataR = comm.ExecuteReader())
                {
                    while (dataR.Read())
                    {
                        long account_id = dataR.GetInt64(0);
                        String username = dataR.GetString(1);
                        String password = dataR.GetString(2);
                        Account account = new Account(account_id, username, password);
                        accountsR.Add(account);
                    }
                }
            }
            log.InfoFormat("Exiting AccountDBRepository findAll with value {0}", null);
            return accountsR;
        }

        public Account findOne(long id)
        {
            log.InfoFormat("Entering AccountDBRepository findOne with value {0}", id);
            IDbConnection con = DBUtils.getConnection(props);

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "select * from Accounts where account_id=@account_id";
                IDbDataParameter paramId = comm.CreateParameter();
                paramId.ParameterName = "@account_id";
                paramId.Value = id;
                comm.Parameters.Add(paramId);

                using (var dataR = comm.ExecuteReader())
                {
                    if (dataR.Read())
                    {
                        long account_id = dataR.GetInt64(0);
                        String username = dataR.GetString(1);
                        String password = dataR.GetString(2);
                        Account account = new Account(account_id, username, password);
                        return account;
                    }
                }
            }
            log.InfoFormat("Exiting AccountDBRepository findOne with value {0}", null);
            throw new RepositoryException("User not found!");
        }

        public void save(Account entity)
        {
            log.InfoFormat("Entering AccountDBRepository save with entity {0}", entity.ToString());
            var con = DBUtils.getConnection(props);

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "insert into Accounts(username, password) values (@username, @password)";

                var paramUsername = comm.CreateParameter();
                paramUsername.ParameterName = "@username";
                paramUsername.Value = entity.Username;
                comm.Parameters.Add(paramUsername);

                var paramPassword = comm.CreateParameter();
                paramPassword.ParameterName = "@password";
                paramPassword.Value = entity.Password;
                comm.Parameters.Add(paramPassword);

                var result = comm.ExecuteNonQuery();
                if (result == 0)
                    throw new RepositoryException("No account added!");
                log.InfoFormat("Exiting AccountDBRepository save with value {0}", result);
            }
        }

        public void update(Account entity)
        {
            log.InfoFormat("Entering AccountDBRepository update with entity {0}", entity.ToString());
            var con = DBUtils.getConnection(props);

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "update Account set username = @username, password = @password where account_id = @account_id";

                var paramUsername = comm.CreateParameter();
                paramUsername.ParameterName = "@username";
                paramUsername.Value = entity.Username;
                comm.Parameters.Add(paramUsername);

                var paramPassword = comm.CreateParameter();
                paramPassword.ParameterName = "@password";
                paramPassword.Value = entity.Password;
                comm.Parameters.Add(paramPassword);

                var paramid = comm.CreateParameter();
                paramid.ParameterName = "@account_id";
                paramid.Value = entity.AccountId;
                comm.Parameters.Add(paramid);

                var result = comm.ExecuteNonQuery();
                if (result == 0)
                    throw new RepositoryException("No account updated!");
                log.InfoFormat("Exiting AccountDBRepository update with value {0}", result);
            }
        }

        public Account findBy(String username, String password)
        {
            log.InfoFormat("Entering AccountDBRepository findByUsername with value {0}", username);
            IDbConnection con = DBUtils.getConnection(props);

            using (var comm = con.CreateCommand())
            {
                comm.CommandText = "select * from Accounts where username=@username and password=@password";
                IDbDataParameter paramUsername = comm.CreateParameter();
                paramUsername.ParameterName = "@username";
                paramUsername.Value = username;
                comm.Parameters.Add(paramUsername);
                
                IDbDataParameter paramPassword = comm.CreateParameter();
                paramPassword.ParameterName = "@password";
                paramPassword.Value = password;
                comm.Parameters.Add(paramPassword);

                using (var dataR = comm.ExecuteReader())
                {
                    if (dataR.Read())
                    {
                        long account_id = dataR.GetInt64(0);
                        Account account = new Account(account_id, username, password);
                        return account;
                    }
                }
            }
            log.InfoFormat("Exiting AccountDBRepository findBy with value {0}", null);
            throw new RepositoryException("User not found!");
        }
    }
}
