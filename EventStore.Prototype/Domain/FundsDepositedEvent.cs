namespace EventStore.Prototype.Domain
{
    public class FundsDepositedEvent : IEvent
    {
        public FundsDepositedEvent(Guid id, decimal amount)
        {

            Id = id;
            Amount = amount;
        }

        public Guid Id { get; }
        public decimal Amount { get; }
    }
}