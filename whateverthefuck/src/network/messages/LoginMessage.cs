namespace whateverthefuck.src.network.messages
{
    using System.Runtime.InteropServices;
    using whateverthefuck.src.util;

    public class LoginMessage : WhateverthefuckMessage
    {
        public LoginMessage()
            : base(MessageType.LoginMessage)
        {
        }

        public LoginMessage(LoginCredentials loginCredentials)
            : base(MessageType.LoginMessage)
        {
            this.LoginCredentials = loginCredentials;
        }

        public LoginCredentials LoginCredentials { get; private set; }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode(this.LoginCredentials.Username);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            var username = decoder.DecodeString();

            this.LoginCredentials = new LoginCredentials(username);
        }
    }
}