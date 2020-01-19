using System;
using whateverthefuck.src.model.entities;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model
{
    public abstract class CreationArguments : IEncodable
    {
        public CreationArguments()
        {
        }

        public static CreationArguments FromEntityType(EntityType type)
        {
            switch (type)
            {
                case EntityType.Block:
                {
                    return new BlockCreationArguments();
                }

                case EntityType.Door:
                {
                    return new DoorCreationArguments();
                }

                case EntityType.Floor:
                {
                    return new FloorCreationArguments();
                }

                case EntityType.NPC:
                {
                    return new NPCCreationArguments();
                }

                case EntityType.PC:
                {
                    return new PCCreationArguments();
                }

                case EntityType.Test:
                {
                    return new TestCreationArguments();
                }

                default: throw new Exception();
            }
        }
#if false
        public CreationArgs(ulong value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the unencoded value of the CreationArgs.
        /// </summary>
        public ulong Value { get; protected set; }

        /// <summary>
        /// Gets or sets the most significant int.
        /// </summary>
        protected int FirstInt
        {
            get
            {
                return (int)((this.Value & 0xFFFFFFFF00000000) >> 32);
            }

            set
            {
                this.Value &= 0x00000000FFFFFFFF;
                this.Value |= (ulong)value << 32;
            }
        }

        /// <summary>
        /// Gets or sets the least significant int.
        /// </summary>
        protected int SecondInt
        {
            get
            {
                return (int)(this.Value & 0xFFFFFFFF);
            }

            set
            {
                this.Value &= 0xFFFFFFFF00000000;
                this.Value |= (ulong)value << 0;
            }
        }
#endif
        public abstract void Decode(WhateverDecoder decoder);

        public abstract void Encode(WhateverEncoder encoder);
    }
}
