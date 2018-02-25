using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Server : IdentifiableSocket
    {
        private readonly List<IdentifiableSocket> ClientSockets = new List<IdentifiableSocket>();
        public bool IgnoreCallback { get; private set; }
        public string[] ActiveClients
        {
            get
            {
               return ClientSockets.Select(CS => CS.Name).ToArray();
            }
        }
        public Server(Socket socket, string name) : base(socket)
        {
            SetName(name);
            SetupServer();
        }
        ~Server()
        {
            if (ClientSockets.Count > 0)
            {
                CloseAllSockets();
            }
        }
        public void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            Socket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            Socket.Listen(1);
            Socket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            Console.WriteLine("Server setup complete");
        }
        public void CloseAllSockets()
        {
            Console.WriteLine("Shutting down server...");
            foreach (IdentifiableSocket IdentifiableSocket in ClientSockets)
            {
                IdentifiableSocket.Socket.Shutdown(SocketShutdown.Both);
                IdentifiableSocket.Socket.Close(100);
            }
            Socket.Close();
            IgnoreCallback = true;
            Console.WriteLine("Server shutdown complete");
            Console.WriteLine("Press any key to continue..");
            Console.ReadKey();
        }

        #region Callbacks
        private void AcceptCallback(IAsyncResult AsyncResult)
        {
            if (IgnoreCallback) return;
            IdentifiableSocket IdentifiableSocket = new IdentifiableSocket(Socket.EndAccept(AsyncResult));
            ClientSockets.Add(IdentifiableSocket);
            Console.WriteLine("Client connected, waiting for request...");
            IdentifiableSocket.Socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), IdentifiableSocket);
            Socket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }
        private void ReceiveCallback(IAsyncResult AsyncResult)
        {
            IdentifiableSocket IdentifiableSocket = (IdentifiableSocket)AsyncResult.AsyncState;
            int received = IdentifiableSocket.Socket.EndReceive(AsyncResult);
            byte[] DataBuffer = new byte[received];
            Array.Copy(Buffer, DataBuffer, received);
            string text = Encoding.ASCII.GetString(DataBuffer);
            string response = string.Empty;
            if (!IdentifiableSocket.IsIdentifiable)
            {
                IdentifiableSocket.SetName(text);
                Console.WriteLine("Client identified as {0}", text);
                response = "Client identified";
            }
            else
            {
                Console.WriteLine("{0}: {1}", IdentifiableSocket.Name, text);
                switch (text)
                {
                    case "commands":
                        response = "commands: \n\tget time: \n\t\tgets the server time \n\texit: \n\t\texits the client \n\tcommands: \n\t\tshows a list of commands to execute";
                        break;
                    case "get time":
                        response = DateTime.Now.ToLongTimeString();
                        break;
                    case "exit":
                        IdentifiableSocket.Socket.Shutdown(SocketShutdown.Both);
                        IdentifiableSocket.Socket.Close();
                        ClientSockets.Remove(IdentifiableSocket);
                        Console.WriteLine("{0}: disconnected", IdentifiableSocket.Name);
                        return;
                    default:
                        response = "Invalid Request, type \"commands\" to see a list of commands to execute";
                        break;
                }
            }
            byte[] data = Encoding.ASCII.GetBytes(response);
            IdentifiableSocket.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), IdentifiableSocket);
            IdentifiableSocket.Socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), IdentifiableSocket);
        }
        private void SendCallback(IAsyncResult AsyncResult)
        {
            IdentifiableSocket IdentifiableSocket = (IdentifiableSocket)AsyncResult.AsyncState;
            IdentifiableSocket.Socket.EndSend(AsyncResult);
        }
        #endregion Callbacks
    }
}
