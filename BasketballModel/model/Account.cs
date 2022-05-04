using System;

namespace BasketballModel
{
    [Serializable]
    public class Account : IEntity<long>
    {
        public long AccountId { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }

        public Account()
        {
            
        }

        public Account(long Ida, String Username, String Password)
        {
            this.AccountId = Ida;
            this.Username = Username;
            this.Password = Password;
        }

        public Account(String Username, String Password)
        {
            this.Username = Username;
            this.Password = Password;
        }

        public override string ToString()
        {
            return AccountId.ToString() + " " + Username.ToString() + " " + Password.ToString();
        }
    }
}
