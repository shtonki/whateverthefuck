using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using whateverthefuck.src.model.entities;
using whateverthefuck.src.util;

namespace whateverthefuck.src.network.messages
{
    public class SendLoginCredentialsMessage : WhateverthefuckMessage
    {
        public LoginCredentials LoginCredentials { get; private set; }

        public SendLoginCredentialsMessage(LoginCredentials loginCredentials) : base(MessageType.SendLoginCredentials)
        {
            LoginCredentials = loginCredentials;
        }

        public SendLoginCredentialsMessage(byte[] body) : base(MessageType.SendLoginCredentials)
        {
            var json = System.Text.Encoding.ASCII.GetString(body);
            LoginCredentials = JsonIO.ConvertToString<LoginCredentials>(json);
            Logging.Log(json);
        }

        protected override byte[] EncodeBody()
        {
            var loginCredentials = JsonIO.ConvertToJson<LoginCredentials>(LoginCredentials);
            Logging.Log(LoginCredentials.Username);
            return System.Text.Encoding.ASCII.GetBytes(loginCredentials);
        }
    }
}