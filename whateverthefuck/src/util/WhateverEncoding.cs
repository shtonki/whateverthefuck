using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.util
{
    static class WhateverEncoding
    {


        public static byte[] EncodeInt(int value)
        {
            byte[] bytes = new byte[sizeof(int)];

            bytes[0] = (byte)(value >> 24);
            bytes[1] = (byte)(value >> 16);
            bytes[2] = (byte)(value >> 8);
            bytes[3] = (byte)value;

            return bytes;
        }

        public static int DecodeInt(byte[] body)
        {
            if (body.Length != 4)
            {
                throw new Exception();
            }

            return (body[0] << 24) |
                   (body[1] << 16) |
                   (body[2] << 8) |
                   (body[3]);
        }
    }
}
