using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Client : IdentifiableSocket
    {
        public IPAddress RemoteIpAddress { get; private set; }
        public Client(Socket socket, IPAddress remoteIpAddress, string name) : base(socket)
        {
            RemoteIpAddress = remoteIpAddress;
            SetName(name);
            ConnectToServer();
        }
        ~Client()
        {
            if (Socket.Connected)
            {
                Exit();
            }
        }
        public void ConnectToServer()
        {
            int ConnectionAttempts = 0;
            while (!Socket.Connected)
            {
                try
                {
                    Console.WriteLine("Connecting to server...");
                    Console.WriteLine("Connection attempts: {0}", ++ConnectionAttempts);
                    Socket.Connect(RemoteIpAddress, PORT);
                }
                catch (SocketException)
                {
                    Console.Clear();
                }
            }
            Console.Clear();
            Console.WriteLine("Connected");
            try
            {
                SendString(Name);
                ReceiveResponse();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ReceiveLoop()
        {
            while (true)
            {
                ReceiveResponse();
            }
        }
        public void SendLoop()
        {
            while (true)
            {
                SendRequest();
            }
        }
        public void RequestAndReceiveLoop()
        {
            Console.WriteLine("\n<Type \"commands\" to see a list of commands>");
            while (true)
            {
                SendRequest();
                ReceiveResponse();
            }
        }
        public void SendRequest()
        {
            Console.WriteLine("Enter a request: ");
            string request = Console.ReadLine();
            SendString(request);
            if (request.ToLower() == "exit")
            {
                Exit();
            }
        }
        public void ReceiveResponse()
        {
            byte[] Buffer = new byte[BUFFER_SIZE];
            int received = Socket.Receive(Buffer, SocketFlags.None);
            if (received == 0) return;
            byte[] Data = new byte[received];
            Array.Copy(Buffer, Data, received);
            string text = Encoding.ASCII.GetString(Data);
            if (text == "Client identified")
            {
                Console.WriteLine("Client is identified");
            }
            else
            {
                Console.WriteLine(text);
            }
        }
        public void SendString(string request)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(request);
            Socket.Send(buffer);
        }
        public void Exit()
        {
            SendString("exit");
            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
            Environment.Exit(0);
        }
    }
}
