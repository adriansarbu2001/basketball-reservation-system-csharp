using System;

namespace BasketballModel
{
    [Serializable]
    public class Ticket : IEntity<long>
    {
        public long TicketId { get; set; }
        public String ClientName { get; set; }
        public int NoSeats { get; set; }
        public long MatchId { get; set; }

        public Ticket()
        {
            
        }

        public Ticket(long Idt, String ClientName, int NoSeats, long matchId)
        {
            this.TicketId = Idt;
            this.ClientName = ClientName;
            this.NoSeats = NoSeats;
            this.MatchId = matchId;
        }

        public Ticket(String ClientName, int NoSeats, long matchId)
        {
            this.ClientName = ClientName;
            this.NoSeats = NoSeats;
            this.MatchId = matchId;
        }

        public override string ToString()
        {
            return TicketId.ToString() + " " + ClientName.ToString() + " " + NoSeats.ToString() + " " + MatchId.ToString();
        }
    }
}
