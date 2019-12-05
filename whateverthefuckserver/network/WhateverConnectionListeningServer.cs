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

namespace whateverthefuckserver.network
{
    class WhateverConnectionListeningServer
    {
        TcpListener server = null;
        List<WhateverthefuckServerConnection> ActiveConnections = new List<WhateverthefuckServerConnection>();

        public void StartListening()
        {
            Thread listenThread = new Thread(ListenForNewConnections);
            listenThread.Start();
        }

        public void SendMessageToEveryone(WhateverthefuckMessage message)
        {
            // todo we encode the message seperately for each client

            foreach (var client in ActiveConnections)
            {
                client.SendMessage(message);
            }
        }

        private void AddClient(TcpClient client)
        {
            var connection = new WhateverthefuckServerConnection(client.GetStream());
            
            ActiveConnections.Add(connection);
            Logging.Log("Client has connected.", Logging.LoggingLevel.Info);
            Program.GameServer.AddPlayer(connection);
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

                    AddClient(client);
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
