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
            this.LooteeId = lootee.Identifier;
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
                encoder.Encode((int)item.Type);
                encoder.Encode((int)item.Rarity);
                encoder.Encode(item.StackSize);
                encoder.Encode(item.Bonuses.Length);
                foreach (var bonus in item.Bonuses)
                {
                    encoder.Encode(bonus.ToInt());
                }
            }
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.LooteeId = new EntityIdentifier(decoder.DecodeInt());

            var itemCount = decoder.DecodeInt();

            this.Items = new Item[itemCount];

            for (int i = 0; i < itemCount; i++)
            {
                var type = (ItemType)decoder.DecodeInt();
                var rarity = (Rarity)decoder.DecodeInt();
                var stackSize = decoder.DecodeInt();

                var bonusCount = decoder.DecodeInt();
                var bonuses = new ItemBonus[bonusCount];

                for (int j = 0; j < bonusCount; j++)
                {
                    bonuses[j] = new ItemBonus(decoder.DecodeInt());
                }

                this.Items[i] = new Item(type, stackSize, rarity, bonuses);
            }
        }
    }
}