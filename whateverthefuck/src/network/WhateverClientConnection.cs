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

        private const bool ConnectToLocalhostFirst = true;

        private const string ServerIp = "98.128.171.8";
        private const string BackupServerIp = "127.0.0.1";
        private const int ServerPort = 13000;

        public WhateverClientConnection() : base(ConnectToServer())
        {
            
        }

        private static NetworkStream ConnectToServer()
        {
            TcpClient ServerConnection;

            try
            {
                if (ConnectToLocalhostFirst)
                {
                    throw new SocketException();
                }
                else
                {
                    ServerConnection = new TcpClient(ServerIp, ServerPort);
                    Logging.Log("Connected to main server.");
                }
            }
            catch (SocketException)
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
                case MessageType.Log:
                {
                    LogMessage logMessage = (LogMessage)message;
                    Logging.Log("Message from server: " + logMessage.Message, Logging.LoggingLevel.Info);
                } break;

                case MessageType.UpdateEntityLocations:
                {
                    UpdateEntityLocationsMessage updateMessage = (UpdateEntityLocationsMessage)message;
                    Program.GameStateManager.UpdateLocations(updateMessage.EntityInfos);
                } break;

                case MessageType.CreateGameEntityMessage:
                {
                    CreateGameEntityMessage createGameEntityMessage = (CreateGameEntityMessage)message;
                    Program.GameStateManager.CreateEntity(createGameEntityMessage.CreateEntityInfo);
                } break;

                case MessageType.GrantControlMessage:
                {
                    GrantControlMessage controlMessage = (GrantControlMessage)message;
                    Program.GameStateManager.TakeControl(controlMessage.Id);
                } break;

                case MessageType.DeleteGameEntityMessage:
                {
                    DeleteGameEntityMessage deleteMessage = (DeleteGameEntityMessage)message;
                     Program.GameStateManager.RemoveEntity(deleteMessage.Identifier);
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