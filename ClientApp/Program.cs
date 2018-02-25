using Library;
using System;
using System.Net;
using System.Net.Sockets;

namespace ClientApp
{
    public class Program
    {
        private static Client Client;

        public static void Main(string[] args)
        {
            Console.Title = "Client";
            Console.WriteLine("Enter a username: ");
            string Username = Console.ReadLine();
            Client = new Client(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
                IPAddress.Loopback, Username);
            Console.Title = Username + " Client";
            Client.RequestAndReceiveLoop();
            Console.ReadLine();
        }        
    }
}
