namespace whateverthefuck.src.network.messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using whateverthefuck.src.model;
    using whateverthefuck.src.util;

    public class GameEventsMessage : WhateverthefuckMessage
    {
        public GameEventsMessage(int tick, IEnumerable<GameEvent> events)
            : base(MessageType.GameEventMessage)
        {
            this.Tick = tick;
            this.Events = events;
        }

        public GameEventsMessage(GameEvent e)
            : this(0, new GameEvent[] { e })
        {
        }

        public GameEventsMessage()
            : base(MessageType.GameEventMessage)
        {
        }

        public int Tick { get; private set; }

        public IEnumerable<GameEvent> Events { get; private set; }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode(this.Tick);

            encoder.Encode(this.Events.Count());

            foreach (var e in this.Events)
            {
                encoder.Encode((int)e.Type);
                e.Encode(encoder);
            }
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.Tick = decoder.DecodeInt();

            var eventsCount = decoder.DecodeInt();
            var events = new GameEvent[eventsCount];

            for (int i = 0; i < eventsCount; i++)
            {
                var type = (GameEventType)decoder.DecodeInt();
                events[i] = GameEvent.FromType(type);
                events[i].Decode(decoder);
            }

            this.Events = events;
        }
    }
}
