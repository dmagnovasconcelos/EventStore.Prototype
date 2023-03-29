namespace EventStore.Prototype.Domain
{
    public class BankAccount
	{
        public Guid Id { get; set; }
		public string Name { get; set; }
		public decimal CurrentBalance { get; set; }
		public List<Transaction> Transactions { get; set; }

		public void Apply(AccountCreatedEvent @event)
		{
			Id = @event.Id;
			Name = @event.Name;
			CurrentBalance = 0;
		}

		public void Apply(FundsDepositedEvent @event)
		{
			var newTransaction = new Transaction(@event.Id, @event.Amount);
			Transactions ??= new();
			Transactions.Add(newTransaction);
			CurrentBalance += @event.Amount;
		}

        public void Apply(FundsWithdrawalEvent @event)
        {
            var newTransaction = new Transaction(@event.Id, @event.Amount);
            Transactions ??= new();
            Transactions.Add(newTransaction);
            CurrentBalance -= @event.Amount;
        }
    }
}

