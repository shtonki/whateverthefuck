using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using whateverthefuck.src.model.entities;
using whateverthefuck.src.util;

namespace whateverthefuck.src.network.messages
{
    public class LoginCredentialsMessage : WhateverthefuckMessage
    {
        public LoginCredentialBody Body { get; private set; }

        public LoginCredentialsMessage(LoginCredentials loginCredentials) : base(MessageType.LoginCredentialsMessage)
        {
            Body = new LoginCredentialBody(loginCredentials.Username);
        }

        protected override MessageBody GetBody()
        {
            return Body;
        }

        protected override void SetBody(MessageBody body)
        {
            Body = (LoginCredentialBody)body;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LoginCredentialBody : MessageBody
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string Username;

        public LoginCredentialBody(string username)
        {
            Username = username;
        }
    }
}