namespace whateverthefuck.src.network.messages
{
    using System;
    using whateverthefuck.src.util;

    public class GameEventMessage : WhateverthefuckMessage
    {
        public GameEventMessage()
            : base(MessageType.GameEventMessage)
        {
        }

        public override void Decode(WhateverDecoder decoder)
        {
            throw new NotImplementedException();
        }

        public override void Encode(WhateverEncoder encoder)
        {
            throw new NotImplementedException();
        }
    }
}
