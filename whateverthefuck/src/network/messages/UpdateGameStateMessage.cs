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
        public int Tick { get; private set; }
        public List<GameEvent> Events { get; private set; }

        public UpdateGameStateMessage(int tick, params GameEvent[] es) : this(tick, (IEnumerable<GameEvent>)es)
        {

        }

        public UpdateGameStateMessage(int tick, IEnumerable<GameEvent> events) : base(MessageType.UpdateGameStateMessage)
        {
            Tick = tick;
            Events = events.ToList();
        }

        public UpdateGameStateMessage() : base(MessageType.UpdateGameStateMessage)
        {
            Events = new List<GameEvent>();
        }

        protected override byte[] EncodeBody()
        {
            List<byte> bs = new List<byte>();

            bs.AddRange(BitConverter.GetBytes(Tick));

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
        }

        protected override void DecodeBody(byte[] bs)
        {
            var events = new List<GameEvent>();

            int bytec = 0;
            Tick = BitConverter.ToInt32(bs, bytec);
            bytec += sizeof(int);

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
