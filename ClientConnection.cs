using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleHttpProxy
{
    class ClientConnection
    {
        private Socket clientSocket;
        private string destHost;
        private int destPort;

        public ClientConnection(Socket client, string destHost, int destPort)
        {
            this.clientSocket = client;
            this.destHost = destHost;
            this.destPort = destPort;
        }

        public void StartHandling()
        {
            Thread handler = new Thread(Handler);
            handler.Priority = ThreadPriority.AboveNormal;
            handler.Start();
        }

        private void Handler()
        {
            bool recvRequest = true;
            //string EOL = "rn";

            string requestPayload = "";
            //string requestTempLine = "";
            //List<string> requestLines = new List<string>();
            byte[] requestBuffer = new byte[1024];
            byte[] responseBuffer = new byte[1024];

            //requestLines.Clear();

            try
            {
                //State 0: Handle Request from Client
                while (recvRequest)
                {
                    var len = this.clientSocket.Receive(requestBuffer, requestBuffer.Length, SocketFlags.None);
                    string fromByte = ASCIIEncoding.ASCII.GetString(requestBuffer, 0, len);
                    requestPayload += fromByte;
                    //requestTempLine += fromByte;

                    recvRequest = len >= requestBuffer.Length;
                }
                Console.WriteLine("Raw Request Received...");
                //Console.WriteLine(requestPayload);

                //State 1: Rebuilding Request Information and Create Connection to Destination Server
                //string remoteHost = requestLines[0].Split(' ')[1].Replace("http://", "").Split('/')[0];
                //string requestFile = requestLines[0].Replace("http://", "").Replace(remoteHost, "");
                //requestLines[0] = requestFile;

                //requestPayload = "";
                //foreach (string line in requestLines)
                //{
                //    requestPayload += line;
                //    requestPayload += EOL;
                //}

                Socket destServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                destServerSocket.Connect(destHost, destPort);

                //State 2: Sending New Request Information to Destination Server and Relay Response to Client
                destServerSocket.Send(ASCIIEncoding.ASCII.GetBytes(requestPayload));

                Console.WriteLine("Begin Receiving Response...");
                int recvLen = destServerSocket.Receive(responseBuffer, responseBuffer.Length, SocketFlags.None);
                int totalRecv = 0;
                while (recvLen >= responseBuffer.Length)
                {
                    //Console.Write(ASCIIEncoding.ASCII.GetString(responseBuffer, 0, recvLen));
                    totalRecv += recvLen;
                    Console.WriteLine("Receiving partial response. size of part is:{0}. Total size (temp) {1}", recvLen, totalRecv);
                    this.clientSocket.Send(responseBuffer, recvLen, SocketFlags.None);
                    recvLen = destServerSocket.Receive(responseBuffer, responseBuffer.Length, SocketFlags.None);
                }

                totalRecv += recvLen;
                this.clientSocket.Send(responseBuffer, recvLen, SocketFlags.None);
                Console.WriteLine("Received response. Size:{0}", totalRecv);

                destServerSocket.Disconnect(false);
                destServerSocket.Dispose();
                this.clientSocket.Disconnect(false);
                this.clientSocket.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Occured: " + e.Message);
                //Console.WriteLine(e.StackTrace);
            }
        }

    }
}
