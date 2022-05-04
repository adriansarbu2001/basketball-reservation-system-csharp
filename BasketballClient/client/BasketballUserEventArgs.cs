using System;

namespace BasketballClient
{
    public enum BasketballUserEvent
    {
        TicketSold
    };

    public class BasketballUserEventArgs : EventArgs
    {
        private readonly BasketballUserEvent userEvent;
        private readonly Object data;

        public BasketballUserEventArgs(BasketballUserEvent userEvent, object data)
        {
            this.userEvent = userEvent;
            this.data = data;
        }

        public BasketballUserEvent UserEventType
        {
            get { return userEvent; }
        }

        public object Data
        {
            get { return data; }
        }
    }
}
