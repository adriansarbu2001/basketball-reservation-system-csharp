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
	public class BasketballServerObjectProxy : IBasketballService
	{
		private string host;
		private int port;

		private IBasketballObserver client;

		private NetworkStream stream;

		private IFormatter formatter;
		private TcpClient connection;

		private Queue<Response> responses;
		private volatile bool finished;
		private EventWaitHandle _waitHandle;

		public BasketballServerObjectProxy(string host, int port)
		{
			this.host = host;
			this.port = port;
			responses = new Queue<Response>();
		}

		public virtual Account login(Account user, IBasketballObserver client)
		{
			initializeConnection();
			sendRequest(new LoginRequest(user));
			Response response = readResponse();
			if (response is OkResponse)
			{
				this.client = client;
				return (Account) ((OkResponse) response).Data;
			}

			if (response is ErrorResponse)
			{
				ErrorResponse err = (ErrorResponse) response;
				closeConnection();
				throw new ServiceException(err.Message);
			}
			return null;
		}

		public virtual void logout(Account user, IBasketballObserver client)
		{
			sendRequest(new LogoutRequest(user));
			Response response = readResponse();
			closeConnection();
			if (response is ErrorResponse)
			{
				ErrorResponse err = (ErrorResponse) response;
				throw new ServiceException(err.Message);
			}
		}

		public IEnumerable<Match> findAll()
		{
			sendRequest(new FindAllMatchesRequest());
			Response response = readResponse();
			if (response is OkResponse)
			{
				return (IEnumerable<Match>) ((OkResponse) response).Data;
			}
			if (response is ErrorResponse)
			{
				ErrorResponse err = (ErrorResponse) response;
				throw new ServiceException(err.Message);
			}
			return null;
		}

		public void saveMatch(string name, float ticket_price, int no_available_seats)
		{
			throw new NotImplementedException();
		}

		public void deleteMatch(long match_id)
		{
			throw new NotImplementedException();
		}

		public void updateMatch(long match_id, string name, float ticket_price, int no_available_seats)
		{
			throw new NotImplementedException();
		}

		public void saveTicket(string client_name, string no_seats, string match_id)
		{
			sendRequest(new SaveTicketRequest(new Ticket(client_name, Int32.Parse(no_seats), Int32.Parse(match_id))));
			Response response = readResponse();
			if (response is ErrorResponse)
			{
				ErrorResponse err = (ErrorResponse) response;
				throw new ServiceException(err.Message);
			}
		}

		public IEnumerable<Match> availableMatchesDescending()
		{
			sendRequest(new AvailableMatchesDescendingRequest());
			Response response = readResponse();
			if (response is OkResponse)
			{
				return (IEnumerable<Match>) ((OkResponse) response).Data;
			}
			if (response is ErrorResponse)
			{
				ErrorResponse err = (ErrorResponse) response;
				throw new ServiceException(err.Message);
			}
			return null;
		}
		
		private void closeConnection()
		{
			finished = true;
			try
			{
				stream.Close();

				connection.Close();
				_waitHandle.Close();
				client = null;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}

		}

		private void sendRequest(Request request)
		{
			try
			{
				formatter.Serialize(stream, request);
				stream.Flush();
			}
			catch (Exception e)
			{
				throw new ServiceException("Error sending object " + e);
			}

		}

		private Response readResponse()
		{
			Response response = null;
			try
			{
				_waitHandle.WaitOne();
				lock (responses)
				{
					//Monitor.Wait(responses); 
					response = responses.Dequeue();

				}


			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}

			return response;
		}

		private void initializeConnection()
		{
			try
			{
				connection = new TcpClient(host, port);
				stream = connection.GetStream();
				formatter = new BinaryFormatter();
				finished = false;
				_waitHandle = new AutoResetEvent(false);
				startReader();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}
		}

		private void startReader()
		{
			Thread tw = new Thread(run);
			tw.Start();
		}
		
		private void handleUpdate(UpdateResponse update)
		{
			if (update is TicketSoldResponse)
			{
				Console.WriteLine("Ticket sold");
				try
				{
					client.ticketSold();
				}
				catch (ServiceException e)
				{
					Console.WriteLine(e.StackTrace);
				}
			}
		}

		public virtual void run()
		{
			while (!finished)
			{
				try
				{
					object response = formatter.Deserialize(stream);
					Console.WriteLine("response received " + response);
					if (response is UpdateResponse)
					{
						handleUpdate((UpdateResponse) response);
					}
					else
					{
						lock (responses)
						{
							responses.Enqueue((Response) response);
						}
						_waitHandle.Set();
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("Reading error " + e);
				}
			}
		}
	}
}
