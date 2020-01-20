using System;
using System.Collections.Generic;
using System.Linq;
using whateverthefuck.src.model;
using whateverthefuck.src.util;

namespace whateverthefuck.src.network.messages
{
    public class AddItemsToInventoryMessage : WhateverthefuckMessage
    {
        public AddItemsToInventoryMessage()
            : base(MessageType.AddItemsToInventoryMessage)
        {
        }

        public AddItemsToInventoryMessage(Item item)
            : this(new Item[] { item })
        {
        }

        public AddItemsToInventoryMessage(IEnumerable<Item> items)
            : base(MessageType.AddItemsToInventoryMessage)
        {
            Items = items.ToArray();
        }

        public Item[] Items { get; private set; }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode(Items);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.Items = decoder.DecodeItemArray();
        }
    }
}
