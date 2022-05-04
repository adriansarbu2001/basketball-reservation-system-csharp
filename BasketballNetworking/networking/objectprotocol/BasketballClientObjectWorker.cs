using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using BasketballModel;
using BasketballServices;

namespace BasketballNetworking
{
	///
	/// <summary> * Created by IntelliJ IDEA.
	/// * User: grigo
	/// * Date: Mar 18, 2009
	/// * Time: 4:04:43 PM </summary>
	/// 
	public class BasketballClientObjectWorker : IBasketballObserver //, Runnable
	{
		private IBasketballService server;
		private TcpClient connection;

		private NetworkStream stream;
		private IFormatter formatter;
		private volatile bool connected;

		public BasketballClientObjectWorker(IBasketballService server, TcpClient connection)
		{
			this.server = server;
			this.connection = connection;
			try
			{
				stream = connection.GetStream();
				formatter = new BinaryFormatter();
				connected = true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}
		}

		public virtual void run()
		{
			while (connected)
			{
				//try
				//{
					object request = formatter.Deserialize(stream);
					object response = handleRequest((Request) request);
					if (response != null)
					{
						sendResponse((Response) response);
					}/*
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(e.StackTrace);
				}*/

				try
				{
					Thread.Sleep(1000);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.StackTrace);
				}
			}

			try
			{
				stream.Close();
				connection.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine("Error " + e);
			}
		}

		public virtual void ticketSold()
		{
			Console.WriteLine("Ticket sold");
			try
			{
				sendResponse(new TicketSoldResponse());
			}
			catch (Exception e)
			{
				throw new ServiceException("Sending error: " + e);
			}
		}

		private Response handleRequest(Request request)
		{
			Response response = null;
			if (request is LoginRequest)
			{
				Console.WriteLine("Login request ...");
				LoginRequest logReq = (LoginRequest) request;
				Account user = logReq.User;
				try
				{
					Account userR;
					lock (server)
					{
						userR = server.login(user, this);
					}

					return new OkResponse(userR);
				}
				catch (ServiceException e)
				{
					connected = false;
					return new ErrorResponse(e.Message);
				}
			}

			if (request is LogoutRequest)
			{
				Console.WriteLine("Logout request ...");
				LogoutRequest logReq = (LogoutRequest) request;
				Account user = logReq.User;
				try
				{
					lock (server)
					{

						server.logout(user, this);
					}

					connected = false;
					return new OkResponse();

				}
				catch (ServiceException e)
				{
					return new ErrorResponse(e.Message);
				}
			}

			if (request is SaveTicketRequest)
			{
				Console.WriteLine("SaveTicketRequest ...");
				SaveTicketRequest senReq = (SaveTicketRequest) request;
				Ticket ticket = senReq.Ticket;
				try
				{
					lock (server)
					{
						server.saveTicket(ticket.ClientName, ticket.NoSeats.ToString(), ticket.MatchId.ToString());
					}

					return new OkResponse();
				}
				catch (ServiceException e)
				{
					return new ErrorResponse(e.Message);
				}
			}

			if (request is FindAllMatchesRequest)
			{
				Console.WriteLine("FindAllMatchesRequest ...");
				try
				{
					IEnumerable<Match> matches;
					
					lock (server)
					{
						matches = server.findAll();
					}

					return new OkResponse(matches);
				}
				catch (ServiceException e)
				{
					return new ErrorResponse(e.Message);
				}
			}

			if (request is AvailableMatchesDescendingRequest)
			{
				Console.WriteLine("AvailableMatchesDescendingRequest ...");
				try
				{
					IEnumerable<Match> matches;
					
					lock (server)
					{
						matches = server.findAll();
					}

					return new OkResponse(matches);
				}
				catch (ServiceException e)
				{
					return new ErrorResponse(e.Message);
				}
			}
			return response;
		}

		private void sendResponse(Response response)
		{
			Console.WriteLine("sending response " + response);
			lock (stream)
			{
				formatter.Serialize(stream, response);
				stream.Flush();
			}
		}
	}
}