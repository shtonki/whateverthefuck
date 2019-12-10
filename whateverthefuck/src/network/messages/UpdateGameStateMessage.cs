using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.util;

namespace whateverthefuck.src.network.messages
{
    public class UpdateGameStateMessage : WhateverthefuckMessage
    {
        public List<GameEvent> Events { get; private set; }

        public UpdateGameStateMessage(params GameEvent[] es) : this((IEnumerable<GameEvent>)es)
        {

        }

        public UpdateGameStateMessage(IEnumerable<GameEvent> events) : base(MessageType.UpdateGameStateMessage)
        {
            Events = events.ToList();
        }

        public UpdateGameStateMessage() : base(MessageType.UpdateGameStateMessage)
        {
            Events = new List<GameEvent>();
        }

        protected override byte[] EncodeBody()
        {
            List<byte> bs = new List<byte>();

            // Structure of encoding:
            // one byte type of event
            // one byte Length
            // arbitrary length of body

            foreach (var ge in Events)
            {
                var v = ge.ToBytes();
                bs.Add((byte)ge.Type);
                bs.Add((byte)v.Length);
                bs.AddRange(v);
            }

            return bs.ToArray();
#if false
            // todo sb -> string -> byte[] seems dubious

            StringBuilder sb = new StringBuilder();

            foreach (var entity in EntityInfos)
            {
                sb.Append(entity.Encode());
                sb.Append(Separator);
            }

            if (sb.Length > 0) { sb.Length--; }

            return System.Text.Encoding.ASCII.GetBytes(sb.ToString());

#endif
        }

        protected override void DecodeBody(byte[] bs)
        {
            var events = new List<GameEvent>();

            int bytec = 0;

            while (true)
            {
                if (bytec == bs.Length)
                {
                    break;
                }

                var gameEventType = (GameEventType)bs[bytec];
                bytec += 1;
                var bodyLength = bs[bytec];
                bytec += 1;
                byte[] body = new byte[bodyLength];
                Array.Copy(bs, bytec, body, 0, bodyLength);
                bytec += bodyLength;

                events.Add(GameEvent.DecodeWithEventType(gameEventType, body.ToArray()));
            }

            Events = events.ToList();
        }

    }
}
