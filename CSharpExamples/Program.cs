using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;

public struct MessageHeader
{
	public uint type;
	public uint payloadSize;

	public MessageHeader(uint type, uint payloadSize)
	{
		this.type = type;
		this.payloadSize = payloadSize;
	}

	public void FromBytes(byte[] bytes)
	{
		byte[] typeBuffer = new byte[4];
		byte[] sizeBuffer = new byte[4];
		Array.Copy(bytes, 0, typeBuffer, 0, 4);
		Array.Copy(bytes, 4, sizeBuffer, 0, 4);

		type = BitConverter.ToUInt32(typeBuffer);
		payloadSize = BitConverter.ToUInt32(sizeBuffer);
	}

	public byte[] ToBytes()
	{
		byte[] buffer = new byte[8];
		byte[] typeBuffer = BitConverter.GetBytes(type);
		byte[] sizeBuffer = BitConverter.GetBytes(payloadSize);
		Array.Copy(typeBuffer, 0, buffer, 0, 4);
		Array.Copy(sizeBuffer, 0, buffer, 4, 4);

		return buffer;
	}
}

public class StateObject
{
	public const int HeaderSize = 8;

	public byte[] headerBuffer = new byte[HeaderSize];

	public MessageHeader header;

	// Receive buffer.  
	public byte[] buffer;

	// Received data string.
	public StringBuilder sb = new StringBuilder();

	// Client socket.
	public Socket workSocket = null;
}

public class AsynchronousSocketListener
{
	// Thread signal.  
	public static ManualResetEvent allDone = new ManualResetEvent(false);

	public AsynchronousSocketListener()
	{
	}

	public static void StartListening()
	{
		// Establish the local endpoint for the socket.  
		// The DNS name of the computer  
		// running the listener is "host.contoso.com".  
		//IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

		string ipString = "127.0.0.1";
		IPAddress ipAddress = IPAddress.Parse(ipString);
		IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

		Console.WriteLine(ipAddress.ToString());

		// Create a TCP/IP socket.  
		Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

		// Bind the socket to the local endpoint and listen for incoming connections.  
		try
		{
			listener.Bind(localEndPoint);
			listener.Listen(100);

			while (true)
			{
				// Set the event to nonsignaled state.  
				allDone.Reset();

				// Start an asynchronous socket to listen for connections.  
				Console.WriteLine("Waiting for a connection...");
				listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

				// Wait until a connection is made before continuing.  
				allDone.WaitOne();
			}

		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}

		Console.WriteLine("\nPress ENTER to continue...");
		Console.Read();

	}

	public static void AcceptCallback(IAsyncResult ar)
	{
		// Signal the main thread to continue.  
		allDone.Set();

		// Get the socket that handles the client request.  
		Socket listener = (Socket)ar.AsyncState;
		Socket handler = listener.EndAccept(ar);

		// Create the state object.  
		StateObject state = new StateObject();
		state.workSocket = handler;

		handler.BeginReceive(state.headerBuffer, 0, StateObject.HeaderSize, 0, new AsyncCallback(ReadHeaderCallback), state);
	}

	public static void ReadHeaderCallback(IAsyncResult ar)
	{
		StateObject state = (StateObject)ar.AsyncState;
		Socket handler = state.workSocket;

		// Read data from the client socket.
		int bytesRead = handler.EndReceive(ar);
		if (bytesRead > 0)
		{
			MessageHeader header = new MessageHeader();
			header.FromBytes(state.headerBuffer);

			Console.WriteLine($"ReadHeader  type : {header.type}  payloadSize : {header.payloadSize}");
			state.buffer = new byte[header.payloadSize];

			handler.BeginReceive(state.buffer, 0, (int)header.payloadSize, 0, new AsyncCallback(ReadCallback), state);
		}
	}

	public static void ReadCallback(IAsyncResult ar)
	{
		// Retrieve the state object and the handler socket  
		// from the asynchronous state object.  
		StateObject state = (StateObject)ar.AsyncState;
		Socket handler = state.workSocket;

		// Read data from the client socket.
		int bytesRead = handler.EndReceive(ar);
		if (bytesRead > 0)
		{
			string data = Encoding.UTF8.GetString(state.buffer);

			Console.WriteLine($"recv  size : {data.Length}  data : { data }");

			string content = "This is data from server.";
			Send(handler, content);
		}
	}

	private static void Send(Socket handler, string data)
	{
		byte[] byteData = Encoding.UTF8.GetBytes(data);

		MessageHeader header = new MessageHeader(0, (uint)byteData.Length);
		byte[] headerBuffer = header.ToBytes();

		byte[] buffer = new byte[headerBuffer.Length + byteData.Length];
		Array.Copy(headerBuffer, 0, buffer, 0, headerBuffer.Length);
		Array.Copy(byteData, 0, buffer, headerBuffer.Length, byteData.Length);

		for (int i = 0; i < buffer.Length; ++i)
		{
			Console.WriteLine(buffer[i]);
		}

		// Begin sending the data to the remote device.  
		handler.BeginSend(buffer, 0, buffer.Length, 0, new AsyncCallback(SendCallback), handler);
	}

	private static void SendCallback(IAsyncResult ar)
	{
		try
		{
			// Retrieve the socket from the state object.  
			Socket handler = (Socket)ar.AsyncState;

			// Complete sending the data to the remote device.  
			int bytesSent = handler.EndSend(ar);
			Console.WriteLine("Sent {0} bytes to client.", bytesSent);

			handler.Shutdown(SocketShutdown.Both);
			handler.Close();

		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}

	public static int Main(String[] args)
	{
		StartListening();
		return 0;
	}
}