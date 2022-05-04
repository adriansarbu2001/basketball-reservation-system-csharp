using System;

namespace BasketballModel.validator
{
    public class ValidatorException : ApplicationException
    {
        public ValidatorException() { }
        public ValidatorException(String mess) : base(mess) { }
        public ValidatorException(String mess, Exception e) : base(mess, e) { }
    }

    public interface IValidator<E>
    {
        void validate(E entity);
    }
}
