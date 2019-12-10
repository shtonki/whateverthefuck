using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model
{
    public abstract class GameEvent
    {
        public static GameEvent DecodeWithEventType(GameEventType type, byte[] body)
        {
            switch (type)
            {
                case GameEventType.Dummy:
                {
                    return new DummyEvent(body);
                }

            default: throw new Exception();
            }
        }

        public GameEventType Type { get; protected set; }
        public abstract byte[] ToBytes();
        public int Size => ToBytes().Length;
    }

    public class DummyEvent : GameEvent
    {
        private int DummyVal { get; set; }


        public DummyEvent(int dummyVal)
        {
            DummyVal = dummyVal;
            Type = GameEventType.Dummy;
        }

        public DummyEvent(byte[] bs)
        {
            DummyVal = WhateverEncoding.IntFromBytes(bs);
            Type = GameEventType.Dummy;
        }

        public override byte[] ToBytes()
        {
            return WhateverEncoding.GetBytes(DummyVal);
        }
    }

    public enum GameEventType
    {
        paddy,
        Dummy,
    }
}
