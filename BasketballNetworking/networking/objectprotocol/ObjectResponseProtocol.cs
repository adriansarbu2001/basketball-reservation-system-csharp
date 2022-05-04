using System;

namespace BasketballNetworking
{
	public interface Response
	{

	}

	[Serializable]
	public class OkResponse : Response
	{
		private object data;

		public OkResponse(object data)
		{
			this.data = data;
		}

		public OkResponse()
		{
			this.data = null;
		}

		public virtual object Data
		{
			get { return data; }
		}
	}

	[Serializable]
	public class ErrorResponse : Response
	{
		private string message;

		public ErrorResponse(string message)
		{
			this.message = message;
		}

		public virtual string Message
		{
			get { return message; }
		}
	}

	public interface UpdateResponse : Response
	{

	}

	[Serializable]
	public class TicketSoldResponse : UpdateResponse
	{
		
	}
}
