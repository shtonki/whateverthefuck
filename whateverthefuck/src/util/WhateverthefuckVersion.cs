using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.util
{
    public class WhateverthefuckVersion
    {
        public short Major { get; }

        public short Minor { get; }

        public short Build { get; }

        public short Revision { get; }

        private const short Pre = short.MaxValue;

        public static WhateverthefuckVersion CurrentVersion => new WhateverthefuckVersion(0, 0, 1, Pre);

        private WhateverthefuckVersion(short major, short minor, short build, short revision)
        {
            Major = major;
            Minor = minor;
            Build = build;
            Revision = revision;
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}.{2}.{3}", Major, Minor, Build, Revision == Pre ? "pre" : Revision.ToString());
        }
    }
}
