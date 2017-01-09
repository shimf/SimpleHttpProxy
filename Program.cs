using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var arguments = new Arguments(args);

            var listenPort = arguments["port"].ToInt();
            var destHost = arguments["destHost"];
            var destPort = arguments["destPort"].ToInt();

            ServerListerner simpleHttpProxyServer = new ServerListerner(listenPort, destHost, destPort);
            simpleHttpProxyServer.StartServer();
            while (true)
            {
                simpleHttpProxyServer.AcceptConnection();
            }
        }
    }
}
