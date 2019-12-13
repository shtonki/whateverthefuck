namespace whateverthefuck.src.network.messages
{
    using System.Runtime.InteropServices;

    public class LoginCredentialsMessage : WhateverthefuckMessage
    {
        public LoginCredentialsMessage()
            : base(MessageType.LoginCredentialsMessage)
        {
            this.MessageBody = new LoginCredentialBody();
        }

        public LoginCredentialsMessage(LoginCredentials loginCredentials)
            : base(MessageType.LoginCredentialsMessage)
        {
            this.MessageBody = new LoginCredentialBody(loginCredentials.Username);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LoginCredentialBody : IMessageBody
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string Username;

        public LoginCredentialBody(string username)
        {
            this.Username = username;
        }
    }
}