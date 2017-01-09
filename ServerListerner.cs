using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpProxy
{
    class ServerListerner
    {
        private int listenPort;
        private TcpListener listener;
        private int destPort;
        private string destHost;

        public ServerListerner(int port, string destHost, int destPort)
        {
            this.listenPort = port;
            this.destHost = destHost;
            this.destPort = destPort;
            this.listener = new TcpListener(IPAddress.Any, this.listenPort);
        }

        public void StartServer()
        {
            this.listener.Start();
        }

        public void AcceptConnection()
        {
            Console.WriteLine("Ready for connection on {0}, Proxying to {1}:{2}", this.listenPort, this.destHost, destPort);
            Socket newClient = this.listener.AcceptSocket();
            Console.WriteLine("Connection accepted: {0}", newClient.LocalEndPoint.AddressFamily.ToString());
            ClientConnection client = new ClientConnection(newClient, this.destHost, this.destPort);
            client.StartHandling();
        }

    }
}
