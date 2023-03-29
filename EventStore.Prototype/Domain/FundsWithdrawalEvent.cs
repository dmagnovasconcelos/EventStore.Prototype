namespace EventStore.Prototype.Domain
{
    public class FundsWithdrawalEvent : IEvent
	{
		public FundsWithdrawalEvent(Guid id, decimal amount)
		{
			Id = id;
			Amount = amount;
		}

        public Guid Id { get; }
		public decimal Amount { get; }
    }
}