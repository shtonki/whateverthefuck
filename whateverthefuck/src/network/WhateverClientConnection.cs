using System.Net.Sockets;
using System.Threading;
using whateverthefuck.network.messages;
using whateverthefuck.src.util;
using System.Linq;
using System;

namespace whateverthefuck.network
{
    class WhateverClientConnection
    {
        private bool arabsarebeingapaininmyass = true;

        private const string ServerIp = "98.128.171.8";
        private const string BackupServerIp = "127.0.0.1";
        private const int ServerPort = 13000;

        private TcpClient ServerConnection;
        private NetworkStream ServerStream;

        private byte[] HeaderBuffer = new byte[WhateverthefuckMessage.HeaderSize];

        public WhateverClientConnection()
        {
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

            ServerStream = ServerConnection.GetStream();

            Thread ReceiveThread = new Thread(ReceiveLoop);
            ReceiveThread.Start();
        }

        private void ReceiveLoop()
        {
            while (true)
            {
                int bytesRead = ServerStream.Read(HeaderBuffer, 0, WhateverthefuckMessage.HeaderSize);
                if (bytesRead != WhateverthefuckMessage.HeaderSize) { throw new Exception("error reading message header"); }

                MessageType messageType = (MessageType)HeaderBuffer[0];
                int messageLength = (HeaderBuffer[1]) | (HeaderBuffer[2] << 8);

                byte[] BodyBuffer = new byte[messageLength];
                bytesRead = ServerStream.Read(BodyBuffer, 0, messageLength);
                if (bytesRead != messageLength) { throw new Exception("error reading message body"); }

                var message = WhateverthefuckMessage.Decode(messageType, BodyBuffer);
                LogMessage msg = (LogMessage)message;

                Logging.Log(msg.Message, Logging.LoggingLevel.Info);
            }
        }
    }
}
