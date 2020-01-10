namespace whateverthefuck.src.network
{
    using System;
    using System.Net.Sockets;
    using whateverthefuck.src.network.messages;
    using whateverthefuck.src.util;

    internal class WhateverClientConnection : WhateverthefuckConnection
    {
        private const string ServerIp = "98.128.171.8";
        private const string BackupServerIp = "127.0.0.1";
        private const int ServerPort = 13000;

        public WhateverClientConnection()
            : base(ConnectToServer())
        {
        }

        protected override void HandleMessage(WhateverthefuckMessage message)
        {
            switch (message.MessageType)
            {
                case MessageType.GameEventMessage:
                {
                    GameEventsMessage gem = (GameEventsMessage)message;
                    Program.GameStateManager.UpdateGameState(gem.Tick, gem.Events);
                } break;

                case MessageType.GrantControlMessage:
                {
                    GrantControlMessage gcm = (GrantControlMessage)message;
                    Program.GameStateManager.TakeControl(gcm.ControlledIdentifier);
                } break;

                default:
                {
                    Logging.Log("Unhandled message of type " + message.MessageType, Logging.LoggingLevel.Warning);
                } break;
            }
        }

        protected override void HandleConnectionDeath()
        {
            throw new NotImplementedException();
        }

        private static NetworkStream ConnectToServer()
        {
            TcpClient serverConnection;
            while (true)
            {
                if (UserSettings.Config.ConnectToLocalHost)
                {
                    try
                    {
                        serverConnection = new TcpClient(BackupServerIp, ServerPort);
                        break;
                    }
                    catch (SocketException)
                    {
                        Logging.Log("Failed to connect to Localhost");
                    }
                }

                try
                {
                    serverConnection = new TcpClient(ServerIp, ServerPort);
                    break;
                }
                catch (SocketException)
                {
                    Logging.Log("Failed to connect to main server.");
                }
            }

            return serverConnection.GetStream();
        }
    }
}
