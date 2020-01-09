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

        private byte[] headerBuffer = new byte[WhateverMessageHeader.HeaderSize];

        protected WhateverthefuckConnection(NetworkStream networkStream)
        {
            this.networkStream = networkStream;
            new Thread(this.ReceiveLoop).Start();
        }

        public void SendMessage(WhateverthefuckMessage message)
        {
            byte[] messageBytes = WhateverthefuckMessage.EncodeMessage(message);

            WhateverMessageHeader header = new WhateverMessageHeader(message.MessageType, messageBytes.Length);
            var headerEncoder = new WhateverEncoder();
            header.Encode(headerEncoder);
            var headerBytes = headerEncoder.GetBytes();

            try
            {
                this.networkStream.Write(headerBytes, 0, headerBytes.Length);
                this.networkStream.Write(messageBytes, 0, messageBytes.Length);
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
                    bytesRead = this.networkStream.Read(this.headerBuffer, 0, this.headerBuffer.Length);
                }
                catch (System.IO.IOException)
                {
                    this.HandleConnectionDeath();
                    return;
                }

                if (bytesRead != this.headerBuffer.Length)
                {
                    Logging.Log("Broken message header.", Logging.LoggingLevel.Error);
                    continue;
                }

                WhateverDecoder headerDecoder = new WhateverDecoder(this.headerBuffer);
                WhateverMessageHeader header = new WhateverMessageHeader();
                header.Decode(headerDecoder);

                bytesRead = 0;
                byte[] bodyBuffer = new byte[header.MessageSize];

                while (true)
                {
                    bytesRead += this.networkStream.Read(bodyBuffer, bytesRead, bodyBuffer.Length);

                    if (bytesRead == bodyBuffer.Length)
                    {
                        break;
                    }
                    else if (bytesRead > bodyBuffer.Length)
                    {
                        throw new Exception(string.Format("Expected body length <{0}>, got <{1}>", bodyBuffer.Length, bytesRead));
                    }
                    else
                    {
                        Logging.Log("Read a message larger than advertised", Logging.LoggingLevel.Error);
                    }
                }

                WhateverthefuckMessage message = WhateverthefuckMessage.DecodeMessage(header.MessageType, bodyBuffer);

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
