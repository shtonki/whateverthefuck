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
        public LoginCredentialsMessage() : base(MessageType.LoginCredentialsMessage)
        {
            MessageBody = new LoginCredentialBody();
        }

        public LoginCredentialsMessage(LoginCredentials loginCredentials) : base(MessageType.LoginCredentialsMessage)
        {
            MessageBody = new LoginCredentialBody(loginCredentials.Username);
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