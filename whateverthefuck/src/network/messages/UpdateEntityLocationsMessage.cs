using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;

namespace whateverthefuck.src.network.messages
{
    

    public class UpdateEntityLocationsMessage : WhateverthefuckMessage
    {
        private const char Separator = ';';

        public List<EntityLocationInfo> EntityInfos { get; }

        public UpdateEntityLocationsMessage(IEnumerable<GameEntity> entities) : base(MessageType.UpdateEntityLocations)
        {
            EntityInfos = new List<EntityLocationInfo>();

            foreach (var entity in entities)
            {
                EntityInfos.Add(new EntityLocationInfo(entity.Identifier.Id, entity.Location.X, entity.Location.Y));
            }
        }

        public UpdateEntityLocationsMessage(byte[] bytes) : base(MessageType.UpdateEntityLocations)
        {
            EntityInfos = new List<EntityLocationInfo>();

            var str = System.Text.Encoding.ASCII.GetString(bytes);

            var infostrings = str.Split(Separator);

            foreach (var info in infostrings)
            {
                EntityInfos.Add(EntityLocationInfo.Decode(System.Text.Encoding.ASCII.GetBytes(info)));
            }
        }

        protected override byte[] EncodeBody()
        {
            // todo sb -> string -> byte[] seems dubious

            StringBuilder sb = new StringBuilder();

            foreach (var entity in EntityInfos)
            {
                sb.Append(entity.Encode());
                sb.Append(Separator);
            }

            if (sb.Length > 0) { sb.Length--; }

            return System.Text.Encoding.ASCII.GetBytes(sb.ToString());
        }
    }
}
