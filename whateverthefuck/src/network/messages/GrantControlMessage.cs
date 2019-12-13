namespace whateverthefuck.src.network.messages
{
    using System.Runtime.InteropServices;
    using whateverthefuck.src.model;

    public class GrantControlMessage : WhateverthefuckMessage
    {
        public GrantControlMessage()
            : base(MessageType.GrantControlMessage)
        {
            this.MessageBody = new GrantControlBody();
        }

        public GrantControlMessage(GameEntity entity)
            : base(MessageType.GrantControlMessage)
        {
            this.MessageBody = new GrantControlBody(entity);
        }

        public GrantControlMessage(int id)
            : base(MessageType.GrantControlMessage)
        {
            var v = new GrantControlBody();
            v.Id = id;
            this.MessageBody = v;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GrantControlBody : IMessageBody
    {
        public int Id;

        public GrantControlBody(GameEntity entity)
        {
            this.Id = entity.Identifier.Id;
        }
    }
}
