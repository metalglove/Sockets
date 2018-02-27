using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Library
{
    public class IdentifiableSocket
    {
        protected const int BUFFER_SIZE = 2048;
        protected const int PORT = 100;
        protected readonly byte[] Buffer = new byte[BUFFER_SIZE];
        public string Name { get; private set; }
        public bool IsIdentifiableByName
        {
            get
            {
               return !string.IsNullOrWhiteSpace(Name);
            }
        }
        public Socket Socket { get; private set; }

        public IdentifiableSocket(Socket socket)
        {
            Socket = socket;
        }
        public void SetName(string name)
        {
            Name = name;
        }
    }
}
