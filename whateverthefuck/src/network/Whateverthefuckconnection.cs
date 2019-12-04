using System;
using System.Net.Sockets;
using System.Threading;
using whateverthefuck.src.network.messages;

namespace whateverthefuck.src.network
{
    public abstract class WhateverthefuckConnection
    {
        private NetworkStream NetworkStream;

        private byte[] HeaderBuffer = new byte[WhateverthefuckMessage.HeaderSize];

        protected WhateverthefuckConnection(NetworkStream networkStream)
        {
            NetworkStream = networkStream;
            new Thread(ReceiveLoop).Start();
        }

        public void SendMessage(WhateverthefuckMessage message)
        {
            var bytes = message.Encode();
            NetworkStream.Write(bytes, 0, bytes.Length);
        }

        protected abstract void HandleMessage(WhateverthefuckMessage message);

        private void ReceiveLoop()
        {
            while (true)
            {
                int bytesRead = NetworkStream.Read(HeaderBuffer, 0, WhateverthefuckMessage.HeaderSize);
                if (bytesRead != WhateverthefuckMessage.HeaderSize) { throw new Exception("error reading message header"); }

                MessageType messageType = (MessageType)HeaderBuffer[0];
                int messageLength = (HeaderBuffer[1]) | (HeaderBuffer[2] << 8);

                byte[] BodyBuffer = new byte[messageLength];
                bytesRead = NetworkStream.Read(BodyBuffer, 0, messageLength);
                if (bytesRead != messageLength) { throw new Exception("error reading message body"); }

                var message = WhateverthefuckMessage.Decode(messageType, BodyBuffer);

                HandleMessage(message);
            }
        }
    }
}
