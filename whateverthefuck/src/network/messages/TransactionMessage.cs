using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.util;

namespace whateverthefuck.src.network.messages
{
    public class TransactionMessage : WhateverthefuckMessage
    {
        public TransactionMessage()
            : this(null)
        {
        }

        public TransactionMessage(Transaction transaction)
            : base(MessageType.TransactionMessage)
        {
            this.Transaction = transaction;
        }

        public Transaction Transaction { get; private set; }

        public override void Decode(WhateverDecoder decoder)
        {
            Transaction = Transaction.GetTransaction((TransactionIdentifier)decoder.DecodeInt());
        }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode((int)Transaction.TransactionIdentifier);
        }
    }
}
