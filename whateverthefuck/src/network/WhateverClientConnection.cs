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

        protected override void HandleMessage(WhateverthefuckMessage message)
        {
            switch (message.MessageType)
            {
                case MessageType.LogMessage:
                {
                    LogMessage logMessage = (LogMessage)message;
                    LogBody logBody = (LogBody)message.MessageBody;
                    Logging.Log("Message from server: " + logBody.Message, Logging.LoggingLevel.Info);
                } break;

                case MessageType.UpdateGameStateMessage:
                {
                    UpdateGameStateMessage updateMessage = (UpdateGameStateMessage)message;
                    Program.GameStateManager.UpdateGameState(updateMessage.Tick, updateMessage.Events);
                } break;

                case MessageType.GrantControlMessage:
                {
                    GrantControlBody controlBody = (GrantControlBody)message.MessageBody;
                    Program.GameStateManager.TakeControl(controlBody.Id);
                } break;

                case MessageType.CreateLootMessage:
                {
                    CreateLootMessage loot = (CreateLootMessage)message;
                    Program.GameStateManager.SpawnLoot(loot);
                } break;

                default: throw new NotImplementedException();
            }
        }

        protected override void HandleConnectionDeath()
        {
            throw new NotImplementedException();
        }
    }
}
