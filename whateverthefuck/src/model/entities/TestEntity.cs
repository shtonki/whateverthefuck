using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model.entities
{
    public class TestEntity : GameEntity
    {
        private TransactionIdentifier TransactionIdentifier;

        public TestEntity(EntityIdentifier id, TestCreationArguments args)
            : base(id, EntityType.Test, args)
        {
            OnInteract += e => Interact();
            TransactionIdentifier = args.Value;
        }

        public TestEntity(EntityIdentifier id, CreationArguments args)
            : this(id, args as TestCreationArguments)
        {
        }

        private void Interact()
        {
            Program.GameStateManager.RequestTransaction(Transaction.GetTransaction(TransactionIdentifier));
        }
    }

    public class TestCreationArguments : CreationArguments
    {
        public TestCreationArguments()
            : this(TransactionIdentifier.None)
        {
        }

        public TestCreationArguments(TransactionIdentifier value)
        {
            Value = value;
        }

        public TransactionIdentifier Value { get; private set; }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode((int)Value);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            Value = (TransactionIdentifier)decoder.DecodeInt();
        }
    }
}
