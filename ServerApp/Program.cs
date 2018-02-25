using Library;
using System;
using System.Net.Sockets;

namespace ServerApp
{
    public class Program
    {
        private static Server Server = 
            new Server(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), 
                "Server");

        public static void Main(string[] args)
        {
            Console.Title = "Server";
            while (true)
            {
                string command = Console.ReadLine();
                string response = string.Empty;
                switch (command)
                {
                    case "shutdown":
                        return;
                    case "commands":
                        response = "commands: " +
                            "\n\tcommands: \n\t\tshows a list of commands to execute" +
                            "\n\tclear: \n\t\tclears the console" +
                            "\n\tget time: \n\t\tgets the server time " +
                            "\n\tget active clients: \n\t\tshows a list of currently active clients" +
                            "\n\tshutdown: \n\t\tshuts down the server ";
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    case "get time":
                        response = DateTime.Now.ToLongTimeString();
                        break;
                    case "get active clients":
                        response = String.Format("Current active clients: {0}\n\t{1}", Server.ActiveClients.Length, String.Join(", ",Server.ActiveClients));
                        break;
                    default:
                        response = "Invalid command, type \"commands\" to see a list of commands to execute";
                        break;
                }
                Console.WriteLine(response);
            }
        }
    }
}
