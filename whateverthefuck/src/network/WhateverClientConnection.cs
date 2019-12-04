﻿using System.Net.Sockets;
using System.Threading;
using whateverthefuck.src.network.messages;
using whateverthefuck.src.util;
using System.Linq;
using System;

namespace whateverthefuck.src.network
{
    class WhateverClientConnection : WhateverthefuckConnection
    {
        private const bool arabsarebeingapaininmyass = true;

        private const string ServerIp = "98.128.171.8";
        private const string BackupServerIp = "127.0.0.1";
        private const int ServerPort = 13000;

        private TcpClient ServerConnection;


        public WhateverClientConnection() : base(ConnectToServer())
        {

        }

        private static NetworkStream ConnectToServer()
        {
            TcpClient ServerConnection;

            try
            {
                if (arabsarebeingapaininmyass)
                {
                    throw new SocketException();
                }
                else
                {
                    ServerConnection = new TcpClient(ServerIp, ServerPort);
                    Logging.Log("Connected to main server.");
                }
            }
            catch(SocketException e)
            {
                Logging.Log("Failed to connect to main server, attempting to connect to backup.");
                ServerConnection = new TcpClient(BackupServerIp, ServerPort);
                Logging.Log("Connected to backup server.");
            }

            return ServerConnection.GetStream();
        }

        protected override void HandleMessage(WhateverthefuckMessage message)
        {
            switch (message.MessageType)
            {
                case MessageType.LogMessage:
                {
                    LogMessage logMessage = (LogMessage)message;
                    Logging.Log("Message from server: " + logMessage.Message, Logging.LoggingLevel.Info);
                } break;

                default: throw new NotImplementedException();
            }
        }
    }
}
