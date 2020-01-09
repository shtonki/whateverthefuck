namespace whateverthefuck.src.util
{
    using System;
    using System.Collections.Generic;

    public interface IEncodable
    {
        void Encode(WhateverEncoder encoder);

        void Decode(WhateverDecoder decoder);
    }

    public class WhateverEncoder
    {
        private List<byte> bytes;

        public WhateverEncoder()
        {
            this.bytes = new List<byte>();
        }

        public void Encode(int value)
        {
            this.AppendBytes(BitConverter.GetBytes(value));
        }

        public void Encode(float value)
        {
            this.AppendBytes(BitConverter.GetBytes(value));
        }

        public void Encode(byte value)
        {
            this.AppendBytes(value);
        }

        public void Encode(long value)
        {
            this.AppendBytes(BitConverter.GetBytes(value));
        }

        public void Encode(string value)
        {
            this.AppendBytes(System.Text.Encoding.UTF8.GetBytes(value));
            this.AppendBytes(0);
        }

        public byte[] GetBytes()
        {
            return this.bytes.ToArray();
        }

        private void AppendBytes(params byte[] bs)
        {
            this.bytes.AddRange(bs);
        }
    }

    public class WhateverDecoder
    {
        private byte[] bytes;
        private int position;

        public WhateverDecoder(byte[] bs)
        {
            this.bytes = bs;
            this.position = 0;
        }

        public int DecodeInt()
        {
            int returnVal = BitConverter.ToInt32(this.bytes, this.position);
            this.position += sizeof(int);

            return returnVal;
        }

        public float DecodeFloat()
        {
            float returnVal = BitConverter.ToSingle(this.bytes, this.position);
            this.position += sizeof(float);

            return returnVal;
        }

        public byte DecodeByte()
        {
            byte returnVal = this.bytes[this.position];
            this.position += sizeof(byte);

            return returnVal;
        }

        public long DecodeLong()
        {
            long returnVal = BitConverter.ToInt64(this.bytes, this.position);
            this.position += sizeof(long);

            return returnVal;
        }

        public string DecodeString()
        {
            var size = 0;

            while (this.bytes[this.position + size] != 0)
            {
                size++;
            }

            byte[] stringBytes = new byte[size];

            var positionCounter = 0;
            while (positionCounter < size)
            {
                var val = this.bytes[this.position++];
                stringBytes[positionCounter++] = val;
            }

            // skip zero terminator
            this.position++;

            return System.Text.Encoding.UTF8.GetString(stringBytes);
        }
    }
}
