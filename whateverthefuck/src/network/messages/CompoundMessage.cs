using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;

namespace whateverthefuck.src.network.messages
{
    public class CompoundMessage : WhateverthefuckMessage
    {
        public CompoundMessage()
            : this(new List<WhateverthefuckMessage>())
        {

        }

        public CompoundMessage(List<WhateverthefuckMessage> messages)
            : base(MessageType.CompoundMessage)
        {
            Messages = messages;
        }

        public List<WhateverthefuckMessage> Messages { get; private set; }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode(Messages.Count);
            foreach (var message in Messages)
            {
                byte[] bs = WhateverthefuckMessage.EncodeMessage(message);
                encoder.Encode((int)message.MessageType);
                encoder.Encode(bs);
            }
        }

        public override void Decode(WhateverDecoder decoder)
        {
            var messageCount = decoder.DecodeInt();
            Messages = new List<WhateverthefuckMessage>(messageCount);
            for (int i = 0; i < messageCount; i++)
            {
                var type = (MessageType)decoder.DecodeInt();
                var bs = decoder.DecodeBytes();
                var message = WhateverthefuckMessage.DecodeMessage(type, bs);

                Messages.Add(message);
            }
        }

    }
}
