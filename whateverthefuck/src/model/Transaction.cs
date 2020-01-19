using System;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model
{
    public enum TransactionIdentifier
    {
        None,

        ClearInventory,
        TradeBananasForDagger,
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
            Transactions[(int)TransactionIdentifier.ClearInventory] = new Transaction(
                TransactionIdentifier.ClearInventory,
                i => true,
                i => { i.Clear(); }
                );

            Transactions[(int)TransactionIdentifier.TradeBananasForDagger] = new Transaction(
                TransactionIdentifier.TradeBananasForDagger,
                i => i.GetByType(ItemType.Banana, Rarity.Common)?.StackSize >= 10,
                i => { 
                    i.RemoveItem(new Banana(Rarity.Common, 10));
                    i.AddItem(new BronzeDagger(Rarity.Rare));
                }
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
            ExecuteFunction(inventory);
        }
    }
}
