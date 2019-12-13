namespace whateverthefuck.src.util
{
    public static class WhateverEncoding
    {
        public static byte[] GetBytes(int i)
        {
            byte[] bs = new byte[sizeof(int)];

            bs[0] = (byte)(i & 0x000000FF);
            bs[1] = (byte)((i & 0x0000FF00) >> 8);
            bs[2] = (byte)((i & 0x00FF0000) >> 16);
            bs[3] = (byte)((i & 0xFF000000) >> 24);

            return bs;
        }

        public static int IntFromBytes(byte[] bs)
        {
            return bs[0] |
                   (bs[1] << 8) |
                   (bs[2] << 16) |
                   (bs[3] << 24);
        }
    }
}
