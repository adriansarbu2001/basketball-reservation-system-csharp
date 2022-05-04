using System;

namespace BasketballModel
{
    [Serializable]
    public class Match : IEntity<long>
    {
        public long MatchId { get; set; }
        public String Name { get; set; }
        public float TicketPrice { get; set; }
        public int NoAvailableSeats { get; set; }

        public Match()
        {
            
        }

        public Match(long Idm, String Name, float TicketPrice, int NoAvailableSeats)
        {
            this.MatchId = Idm;
            this.Name = Name;
            this.TicketPrice = TicketPrice;
            this.NoAvailableSeats = NoAvailableSeats;
        }

        public Match(String Name, float TicketPrice, int NoAvailableSeats)
        {
            this.Name = Name;
            this.TicketPrice = TicketPrice;
            this.NoAvailableSeats = NoAvailableSeats;
        }

        public override string ToString()
        {
            return MatchId.ToString() + " " + Name.ToString() + " " + TicketPrice.ToString() + " " + NoAvailableSeats.ToString();
        }
    }
}
