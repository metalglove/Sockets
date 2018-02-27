using Library;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

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
            Client = new Client(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), IPAddress.Loopback, Username);
            Console.Title = Username + " Client";
            Parallel.Invoke(
                () => Client.ReceiveLoop(), 
                () => Client.SendLoop()
                );
            Console.ReadLine();
        }        
    }
}
