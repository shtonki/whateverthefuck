using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.model.entities;

namespace whateverthefuck.src.network.messages
{
    public class CreateLootMessage : WhateverthefuckMessage
    {
        public Item Item => GenerateItem();
        public int LooteeId => ((ItemMessageBody)MessageBody).LooteeEntityId;

        private Item GenerateItem()
        {
            ItemMessageBody item = (ItemMessageBody)MessageBody;
            return new Item(item.Type, item.StackSize, item.Rarity, item.Bonuses.ToArray());
        }

        public CreateLootMessage() : base(MessageType.CreateLootMessage)
        {
            MessageBody = new ItemMessageBody();
        }

        public CreateLootMessage(GameEntity lootee, Item item) : base(MessageType.CreateLootMessage)
        {
            var itemBody = new ItemMessageBody(item);
            itemBody.LooteeEntityId = lootee.Identifier.Id;
            MessageBody = itemBody;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    class ItemMessageBody : MessageBody
    {
        public int LooteeEntityId;

        public ItemType Type;
        public int StackSize;
        public Rarity Rarity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        private int[] _bonuses = new int[4];

        public IEnumerable<ItemBonus> Bonuses => _bonuses.Select(b => new ItemBonus(b));


        public ItemMessageBody()
        {
        }

        public ItemMessageBody(Item item)
        {
            Type = item.Type;
            StackSize = item.StackSize;
            Rarity = item.Rarity;
            var bc = item.Bonuses.Select(b => b.ToInt()).ToArray();
            Array.Copy(bc, 0, _bonuses, 0, bc.Length);
        }
    }
}
