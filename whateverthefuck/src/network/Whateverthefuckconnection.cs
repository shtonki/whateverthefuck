using System;
using System.Net.Sockets;
using System.Threading;
using whateverthefuck.src.network.messages;
using whateverthefuck.src.util;

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

        public  void SendMessage(WhateverthefuckMessage message)
        {
            var bytes = message.Encode();
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
                Logging.Log("-> MessageType" + message.MessageType.ToString() + ", payload: '" +
                    System.Text.Encoding.ASCII.GetString(bytes) + "'", Logging.LoggingLevel.Info);
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

                MessageType messageType = (MessageType)HeaderBuffer[0];
                int messageLength = (HeaderBuffer[1]) | (HeaderBuffer[2] << 8);

                byte[] BodyBuffer = new byte[messageLength];
                bytesRead = NetworkStream.Read(BodyBuffer, 0, messageLength);
                if (bytesRead != messageLength) { throw new Exception("error reading message body"); }

                var message = WhateverthefuckMessage.Decode(messageType, BodyBuffer);

                if (LogIncomingMessages)
                {
#pragma warning disable CS0162 // Unreachable code detected
                    Logging.Log("<- MessageType" + message.MessageType.ToString() + ", payload: '" +
                        System.Text.Encoding.ASCII.GetString(BodyBuffer) + "'", Logging.LoggingLevel.Info);
#pragma warning restore CS0162 // Unreachable code detected
                }

                HandleMessage(message);
            }
        }
    }
}
