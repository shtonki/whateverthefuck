using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;

namespace whateverthefuck.src.network.messages
{
    public struct EntityLocationInfo
    {
        public int Identifier { get; }
        public float X { get; }
        public float Y { get; }

        public EntityLocationInfo(int id, float x, float y)
        {
            Identifier = id;
            X = x;
            Y = y;
        }
    }

    public class UpdateEntityLocationsMessage : WhateverthefuckMessage
    {
        private const char InfoSeperator = ',';
        private const char EntitySeperator = ';';

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

            var infostrings = str.Split(EntitySeperator);

            foreach (var info in infostrings)
            {
                var data = info.Split(InfoSeperator);
                int id = Int32.Parse(data[0]);
                float X = float.Parse(data[1]);
                float Y = float.Parse(data[2]);
                EntityInfos.Add(new EntityLocationInfo(id, X, Y));
            }
        }

        protected override byte[] EncodeBody()
        {
            // todo sb -> string -> byte[] seems dubious

            StringBuilder sb = new StringBuilder();

            foreach (var entity in EntityInfos)
            {
                string id = entity.Identifier.ToString();
                string X = entity.X.ToString("0.00");
                string Y = entity.Y.ToString("0.00");
                sb.Append(String.Format("{0}{3}{1}{3}{2}{4}", id, X, Y, InfoSeperator, EntitySeperator));
            }

            if (sb.Length > 0) { sb.Length--; }

            return System.Text.Encoding.ASCII.GetBytes(sb.ToString());
        }
    }
}
