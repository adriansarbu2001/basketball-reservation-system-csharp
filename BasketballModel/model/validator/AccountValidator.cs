using System;

namespace BasketballModel.validator
{
    public class AccountValidator : IValidator<Account>
    {
        public void validate(Account account)
        {
            String err = "";
            if (account.Username.Length <= 0)
            {
                err = err + "username can not be empty!\n";
            }
            if (account.Password.Length < 5)
            {
                err = err + "password is too short!\n";
            }
            if (err.Length > 0)
            {
                throw new ValidatorException(err);
            }
        }
    }
}
