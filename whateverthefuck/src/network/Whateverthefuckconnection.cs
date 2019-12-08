using System;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using whateverthefuck.src.network.messages;
using whateverthefuck.src.util;
using static whateverthefuck.src.network.messages.WhateverthefuckMessage;

namespace whateverthefuck.src.network
{
    public abstract class WhateverthefuckConnection
    {
        private const bool LogOutgoingMessages = false;
        private const bool LogIncomingMessages = false;

        private NetworkStream NetworkStream;

        private byte[] HeaderBuffer = new byte[WhateverthefuckMessage.HeaderSize];

        protected WhateverthefuckConnection(NetworkStream networkStream)
        {
            NetworkStream = networkStream;
            new Thread(ReceiveLoop).Start();
        }

        public void SendMessage(WhateverthefuckMessage message)
        {
            byte[] bytes = WhateverthefuckMessage.EncodeMessage(message);
            try
            {
                NetworkStream.Write(bytes, 0, bytes.Length);
            }
            catch(System.IO.IOException)
            {

            }

            if (LogOutgoingMessages)
            {
#pragma warning disable CS0162 // Unreachable code detected
                Logging.Log("-> " + message.ToString(), Logging.LoggingLevel.Info);
#pragma warning restore CS0162 // Unreachable code detected
            }
        }

        protected abstract void HandleMessage(WhateverthefuckMessage message);

        protected abstract void HandleConnectionDeath();

        private void ReceiveLoop()
        {
            while (true)
            {
                int bytesRead;

                try
                {
                    bytesRead = NetworkStream.Read(HeaderBuffer, 0, WhateverthefuckMessage.HeaderSize);
                }
                catch (System.IO.IOException)
                {
                    HandleConnectionDeath();
                    return;
                }
                if (bytesRead != WhateverthefuckMessage.HeaderSize) 
                {
                    Logging.Log("Broken message header.", Logging.LoggingLevel.Error);
                    continue;
                }

                WhateverMessageHeader header = WhateverthefuckMessage.ParseHeader(HeaderBuffer);

                MessageType messageType = (MessageType)header.Type;
                int messageLength = header.Size;

                byte[] BodyBuffer = new byte[messageLength];
                bytesRead = NetworkStream.Read(BodyBuffer, 0, messageLength);
                if (bytesRead != messageLength) { throw new Exception("error reading message body"); }

                //var message = WhateverthefuckMessage.Decode(messageType, BodyBuffer);
                WhateverthefuckMessage message = WhateverthefuckMessage.DecodeMessage(header, BodyBuffer);

                if (LogIncomingMessages)
                {
#pragma warning disable CS0162 // Unreachable code detected
                    Logging.Log("<- " + message.ToString(), Logging.LoggingLevel.Info);
#pragma warning restore CS0162 // Unreachable code detected
                }

                HandleMessage(message);
            }
        }
    }
}
