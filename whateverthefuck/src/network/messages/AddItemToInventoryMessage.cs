using System;
using whateverthefuck.src.model;
using whateverthefuck.src.util;

namespace whateverthefuck.src.network.messages
{
    public class AddItemToInventoryMessage : WhateverthefuckMessage
    {
        public AddItemToInventoryMessage()
            : base(MessageType.AddItemToInventoryMessage)
        {
        }

        public AddItemToInventoryMessage(Item item)
            : base(MessageType.AddItemToInventoryMessage)
        {
            Item = item;
        }

        public Item Item { get; private set; }

        public override void Encode(WhateverEncoder encoder)
        {
            Item.Encode(encoder);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.Item = Item.FromDecoder(decoder);
        }
    }
}
