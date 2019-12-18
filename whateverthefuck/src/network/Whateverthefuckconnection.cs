namespace whateverthefuck.src.network
{
    using System;
    using System.Net.Sockets;
    using System.Threading;
    using whateverthefuck.src.network.messages;
    using whateverthefuck.src.util;
    using static whateverthefuck.src.network.messages.WhateverthefuckMessage;

    public abstract class WhateverthefuckConnection
    {
        private const bool LogOutgoingMessages = false;
        private const bool LogIncomingMessages = false;

        private NetworkStream networkStream;

        private byte[] headerBuffer = new byte[WhateverthefuckMessage.HeaderSize];

        protected WhateverthefuckConnection(NetworkStream networkStream)
        {
            this.networkStream = networkStream;
            new Thread(this.ReceiveLoop).Start();
        }

        public void SendMessage(WhateverthefuckMessage message)
        {
            byte[] bytes = WhateverthefuckMessage.EncodeMessage(message);
            try
            {
                this.networkStream.Write(bytes, 0, bytes.Length);
            }
            catch (System.IO.IOException)
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
                    bytesRead = this.networkStream.Read(this.headerBuffer, 0, WhateverthefuckMessage.HeaderSize);
                }
                catch (System.IO.IOException)
                {
                    this.HandleConnectionDeath();
                    return;
                }

                if (bytesRead != WhateverthefuckMessage.HeaderSize)
                {
                    Logging.Log("Broken message header.", Logging.LoggingLevel.Error);
                    continue;
                }

                WhateverMessageHeader header = WhateverthefuckMessage.ParseHeader(this.headerBuffer);

                MessageType messageType = (MessageType)header.Type;
                int messageLength = header.Size;

                byte[] bodyBuffer = new byte[messageLength];
                bytesRead = this.networkStream.Read(bodyBuffer, 0, messageLength);
                if (bytesRead != messageLength)
                {
                    throw new Exception("error reading message body");
                }

                WhateverthefuckMessage message = WhateverthefuckMessage.DecodeMessage(header, bodyBuffer);

                if (LogIncomingMessages)
                {
#pragma warning disable CS0162 // Unreachable code detected
                    Logging.Log("<- " + message.ToString(), Logging.LoggingLevel.Info);
#pragma warning restore CS0162 // Unreachable code detected
                }

                this.HandleMessage(message);
            }
        }
    }
}
