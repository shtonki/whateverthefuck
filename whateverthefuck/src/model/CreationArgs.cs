using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.model
{
    public class CreationArgs
    {
        public UInt64 Value { get; set; }

        public static CreationArgs Zero => new CreationArgs(0);

        public CreationArgs(ulong value)
        {
            Value = value;
        }

        public int FirstInt
        {
            get { return (int)((Value & 0xFFFFFFFF00000000) >> 32); }
            set
            {
                Value &= 0x00000000FFFFFFFF;
                Value |= (((ulong)value << 32));
            }
        }

        public int SecondInt
        {
            get { return (int)((Value & 0xFFFFFFFF)); }
            set
            {
                Value &= 0xFFFFFFFF00000000;
                Value |= (ulong)value;
            }
        }
    }
}
