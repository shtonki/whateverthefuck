using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;

namespace whateverthefuck.src.network.messages
{
    class CreateGameStateMessage : WhateverthefuckMessage
    {
        public CreateGameStateMessage()
            : base(MessageType.CreateGameStateMessage)
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
