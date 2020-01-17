namespace whateverthefuck.src.network.messages
{
    using whateverthefuck.src.model;
    using whateverthefuck.src.util;

    public class CreateLootMessage : WhateverthefuckMessage
    {
        public CreateLootMessage()
            : base(MessageType.CreateLootMessage)
        {
        }

        public CreateLootMessage(GameEntity lootee, params Item[] items)
            : base(MessageType.CreateLootMessage)
        {
            this.LooteeId = lootee.Info.Identifier;
            this.Items = items;
        }

        public Item[] Items { get; private set; }

        public EntityIdentifier LooteeId { get; private set; }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode(this.LooteeId.Id);

            encoder.Encode(this.Items.Length);
            foreach (var item in this.Items)
            {
                item.Encode(encoder);
            }
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.LooteeId = new EntityIdentifier(decoder.DecodeInt());

            var itemCount = decoder.DecodeInt();

            this.Items = new Item[itemCount];

            for (int i = 0; i < itemCount; i++)
            {
                this.Items[i] = Item.FromDecoder(decoder);
            }
        }
    }
}