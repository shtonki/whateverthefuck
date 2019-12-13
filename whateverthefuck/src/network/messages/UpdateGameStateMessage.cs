namespace whateverthefuck.src.network.messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using whateverthefuck.src.model;

    public class UpdateGameStateMessage : WhateverthefuckMessage
    {
        public UpdateGameStateMessage(int tick, params GameEvent[] es)
            : this(tick, (IEnumerable<GameEvent>)es)
        {
        }

        public UpdateGameStateMessage(int tick, IEnumerable<GameEvent> events)
            : base(MessageType.UpdateGameStateMessage)
        {
            this.Tick = tick;
            this.Events = events.ToList();
        }

        public UpdateGameStateMessage()
            : base(MessageType.UpdateGameStateMessage)
        {
            this.Events = new List<GameEvent>();
        }

        public int Tick { get; private set; }

        public List<GameEvent> Events { get; private set; }

        protected override byte[] EncodeBody()
        {
            List<byte> bs = new List<byte>();

            bs.AddRange(BitConverter.GetBytes(this.Tick));

            foreach (var ge in this.Events)
            {
                // Structure of encoding:
                // one byte type of event
                // one byte Length
                // arbitrary length of body
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
            this.Tick = BitConverter.ToInt32(bs, bytec);
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

            this.Events = events.ToList();
        }
    }
}
