using System;
using BasketballModel;

namespace BasketballNetworking
{
	public interface Request
	{

	}

	[Serializable]
	public class LoginRequest : Request
	{
		private Account user;

		public LoginRequest(Account user)
		{
			this.user = user;
		}

		public virtual Account User
		{
			get { return user; }
		}
	}

	[Serializable]
	public class LogoutRequest : Request
	{
		private Account user;

		public LogoutRequest(Account user)
		{
			this.user = user;
		}

		public virtual Account User
		{
			get { return user; }
		}
	}

	[Serializable]
	public class SaveTicketRequest : Request
	{
		private Ticket ticket;

		public SaveTicketRequest(Ticket ticket)
		{
			this.ticket = ticket;
		}

		public virtual Ticket Ticket
		{
			get { return ticket; }
		}
	}

	[Serializable]
	public class FindAllMatchesRequest : Request
	{

	}

	[Serializable]
	public class AvailableMatchesDescendingRequest : Request
	{

	}
}
