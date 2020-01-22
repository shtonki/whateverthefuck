using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using whateverthefuck.src.model;

namespace whateverthefuckserver.storage
{
    class ItemStorable
    {
        public ObjectId _id;
        public ItemType Type;
        public int StackSize;
        public Rarity Rarity;
        public ItemBonus[] Bonuses;

        public ItemStorable(Item i)
        {
            this.Type = i.Type;
            this.StackSize = i.StackSize;
            this.Rarity = i.Rarity;
            this.Bonuses = i.Bonuses;
        }
    }
}
