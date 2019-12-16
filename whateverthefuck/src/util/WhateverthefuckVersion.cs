namespace whateverthefuck.src.util
{
    using System;

    public class WhateverthefuckVersion
    {
        private const short Pre = short.MaxValue;

        private WhateverthefuckVersion(short major, short minor, short build, short revision)
        {
            this.Major = major;
            this.Minor = minor;
            this.Build = build;
            this.Revision = revision;
        }

        public static WhateverthefuckVersion CurrentVersion => new WhateverthefuckVersion(0, 0, 1, Pre);

        public short Major { get; }

        public short Minor { get; }

        public short Build { get; }

        public short Revision { get; }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", this.Major, this.Minor, this.Build, this.Revision == Pre ? "pre" : this.Revision.ToString());
        }
    }
}
