namespace whateverthefuck.src.network.messages
{
    using System.Runtime.InteropServices;
    using whateverthefuck.src.util;

    public class LoginCredentialsMessage : WhateverthefuckMessage
    {
        public LoginCredentialsMessage(byte[] bs)
            : base(MessageType.LoginCredentialsMessage)
        {
            WhateverDecoder decoder = new WhateverDecoder(bs);

            var username = decoder.DecodeString();

            this.LoginCredentials = new LoginCredentials(username);
        }

        public LoginCredentialsMessage(LoginCredentials loginCredentials)
            : base(MessageType.LoginCredentialsMessage)
        {
            this.LoginCredentials = loginCredentials;
        }

        public LoginCredentials LoginCredentials { get; }

        protected override byte[] EncodeBody()
        {
            WhateverEncoder encoder = new WhateverEncoder();

            encoder.Encode(this.LoginCredentials.Username);

            return encoder.GetBytes();
        }
    }

}