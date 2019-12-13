namespace whateverthefuck.src.network.messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using whateverthefuck.src.model;

    public class CreateLootMessage : WhateverthefuckMessage
    {
        public Item Item => this.GenerateItem();

        public int LooteeId => ((ItemMessageBody)this.MessageBody).LooteeEntityId;

        private Item GenerateItem()
        {
            ItemMessageBody item = (ItemMessageBody)this.MessageBody;
            return new Item(item.Type, item.StackSize, item.Rarity, item.Bonuses.ToArray());
        }

        public CreateLootMessage()
            : base(MessageType.CreateLootMessage)
        {
            this.MessageBody = new ItemMessageBody();
        }

        public CreateLootMessage(GameEntity lootee, Item item)
            : base(MessageType.CreateLootMessage)
        {
            var itemBody = new ItemMessageBody(item);
            itemBody.LooteeEntityId = lootee.Identifier.Id;
            this.MessageBody = itemBody;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal class ItemMessageBody : IMessageBody
    {
        public int LooteeEntityId;

        public ItemType Type;
        public int StackSize;
        public Rarity Rarity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        private int[] bonuses = new int[4];

        public IEnumerable<ItemBonus> Bonuses => this.bonuses.Select(b => new ItemBonus(b));

        public ItemMessageBody()
        {
        }

        public ItemMessageBody(Item item)
        {
            this.Type = item.Type;
            this.StackSize = item.StackSize;
            this.Rarity = item.Rarity;
            var bc = item.Bonuses.Select(b => b.ToInt()).ToArray();
            Array.Copy(bc, 0, this.bonuses, 0, bc.Length);
        }
    }
}
