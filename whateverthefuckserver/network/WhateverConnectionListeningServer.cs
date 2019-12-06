using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using whateverthefuck.src.network;
using whateverthefuck.src.network.messages;
using whateverthefuck.src.util;
using whateverthefuckserver.storage;

namespace whateverthefuckserver.network
{
    class WhateverConnectionListeningServer
    {
        TcpListener server = null;

        public void StartListening()
        {
            Thread listenThread = new Thread(ListenForNewConnections);
            listenThread.Start();
        }
        
        private void ListenForNewConnections()
        {
            try
            {
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Any;

                server = new TcpListener(localAddr, port);
                server.Start();

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();

                    new WhateverthefuckServerConnection(client.GetStream());
                }
            }
            catch (SocketException e)
            {
                Logging.Log(String.Format("SocketException: {0}", e), Logging.LoggingLevel.Error);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
