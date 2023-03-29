namespace EventStore.Prototype.Domain
{
    public class AccountCreatedEvent : IEvent
	{
		public AccountCreatedEvent(Guid id, string name)
		{
			Id = id;
			Name = name;
		}

        public Guid Id { get; }
		public string Name { get; }
    }
}