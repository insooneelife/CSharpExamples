using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;

public class Program
{
	
	public static int Main(String[] args)
	{
		AsyncTCPSocketServer.Server.StartListening();
		return 0;
	}
}