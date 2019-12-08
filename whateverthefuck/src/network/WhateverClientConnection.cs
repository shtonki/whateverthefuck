﻿using System.Net.Sockets;
using System.Threading;
using whateverthefuck.src.network.messages;
using whateverthefuck.src.util;
using System.Linq;
using System;
using whateverthefuck.src.model;

namespace whateverthefuck.src.network
{
    class WhateverClientConnection : WhateverthefuckConnection
    {

        private const bool ConnectToLocalhostFirst = false;

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
                case MessageType.LogMessage:
                {
                    LogMessage logMessage = (LogMessage)message;
                    LogBody logBody = (LogBody) message.MessageBody;
                    Logging.Log("Message from server: " + logBody.Message, Logging.LoggingLevel.Info);
                } break;

                case MessageType.UpdateEntityLocationsMessage:
                {
                    UpdateEntityLocationsMessage updateMessage = (UpdateEntityLocationsMessage)message;
                    Program.GameStateManager.UpdateLocations(updateMessage.EntityInfos);
                } break;

                case MessageType.CreateGameEntityMessage:
                {
                    CreateEntityInfo info = (CreateEntityInfo)message.MessageBody;
                    Program.GameStateManager.CreateEntity(info);
                } break;

                case MessageType.GrantControlMessage:
                {
                    GrantControlBody controlBody = (GrantControlBody) message.MessageBody;
                    Program.GameStateManager.TakeControl(controlBody.Id);
                } break;

                case MessageType.DeleteGameEntityMessage:
                {
                    DeleteGameEntityMessage deleteMessage = (DeleteGameEntityMessage)message;
                    DeleteGameEntityBody deleteBody = (DeleteGameEntityBody)message.MessageBody;
                    Program.GameStateManager.RemoveEntity(new EntityIdentifier(deleteBody.Id));
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
