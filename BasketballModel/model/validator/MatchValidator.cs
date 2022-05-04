using System;

namespace BasketballModel.validator
{
    public class MatchValidator : IValidator<Match>
    {
        public void validate(Match match)
        {
            String err = "";
            if (match.Name.Length <= 0)
            {
                err = err + "name can not be empty!\n";
            }
            if (match.TicketPrice < 0)
            {
                err = err + "ticket_price must be >= 0!\n";
            }
            if (match.NoAvailableSeats < 0)
            {
                err = err + "no_available_seats must be >= 0!\n";
            }
            if (err.Length > 0)
            {
                throw new ValidatorException(err);
            }
        }
    }
}
