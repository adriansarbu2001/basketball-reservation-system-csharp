using System;

namespace BasketballModel.validator
{
     public class TicketValidator : IValidator<Ticket>
    {
        public void validate(Ticket ticket)
        {
            String err = "";
            if (ticket.ClientName.Length <= 0)
            {
                err = err + "client_name can not be empty!\n";
            }
            if (ticket.NoSeats <= 0)
            {
                err = err + "no_seats must be > 0!\n";
            }
            if (err.Length > 0)
            {
                throw new ValidatorException(err);
            }
        }
    }
}
