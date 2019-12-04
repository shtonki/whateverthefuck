using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuckserver
{
    class WhateverClientConnection
    {
        private const string SERVER_IP = "98.128.171.8";
        private const int SERVER_PORT = 13000;

        private TcpClient ServerConnection;
        private NetworkStream ServerStream;

        public WhateverClientConnection()
        {
            ServerConnection = new TcpClient(SERVER_IP, SERVER_PORT);
            ServerStream = ServerConnection.GetStream();

            while (true)
            {
                int i = 2;
            }
        }
    }
}
