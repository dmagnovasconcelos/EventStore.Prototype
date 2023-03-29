﻿namespace EventStore.Prototype.Domain
{
    public class Transaction
	{
		public Transaction(Guid id, decimal amount)
		{
			Id = id;
			Amount = amount;
		}

        public Guid Id { get; }
        public decimal Amount { get; }
    }
}

