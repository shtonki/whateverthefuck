using System;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model
{
    public enum TransactionIdentifier
    {
        None,

        TestTransaction1,
        TestTransaction2,
    }

    public class Transaction
    {
        private Transaction(TransactionIdentifier transactionIdentifier, Func<Inventory, bool> canAfford, Action<Inventory> excecute)
        {
            TransactionIdentifier = transactionIdentifier;
            CanAffordFunction = canAfford;
            ExecuteFunction = excecute;
        }

        static Transaction()
        {
            Transactions[(int)TransactionIdentifier.TestTransaction1] = new Transaction(
                TransactionIdentifier.TestTransaction1,
                i => true,
                i => { i.Clear(); }
                );

            Transactions[(int)TransactionIdentifier.TestTransaction2] = new Transaction(
                TransactionIdentifier.TestTransaction2,
                i => i.GetByType(ItemType.Banana, Rarity.Common)?.StackSize >= 10,
                i => { i.RemoveItem(new Banana(Rarity.Common, 10)); }
                );
        }

        public static Transaction GetTransaction(TransactionIdentifier tid)
        {
            if (tid == TransactionIdentifier.None) { Logging.Log("GetTransaction was called on None.", Logging.LoggingLevel.Warning); }

            return Transactions[(int)tid];
        }

        private static Transaction[] Transactions = new Transaction[Enum.GetValues(typeof(TransactionIdentifier)).Length];

        public TransactionIdentifier TransactionIdentifier { get; }

        private Func<Inventory, bool> CanAffordFunction { get; }

        private Action<Inventory> ExecuteFunction { get; }

        public bool CanAfford(Inventory inventory)
        {
            return CanAffordFunction(inventory);
        }

        public void Execute(Inventory inventory)
        {
            Logging.Log("Excecuting " + TransactionIdentifier);
            ExecuteFunction(inventory);
        }
    }
}
