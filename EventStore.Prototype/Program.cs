using EventStore.Client;
using EventStore.Prototype.Domain;
using Newtonsoft.Json;
using System.Text;

internal class Program
{
    private static string StreamId(Guid id) => string.Format("BankAccount-{0}", id);
    
    private static void Main(string[] args)
    {
        // Connection in EventStoreDB
        var connectionString = "esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000";
        var settings = EventStoreClientSettings.Create(connectionString);
        var connection = new EventStoreClient(settings);

        #region Append to Stream
        var aggregateId = Guid.NewGuid();
        var events = new List<IEvent>
        {
            new AccountCreatedEvent(aggregateId, "Danilo Magno"),
            new FundsDepositedEvent(aggregateId, 150),
            new FundsDepositedEvent(aggregateId, 100),
            new FundsWithdrawalEvent(aggregateId, 60),
            new FundsWithdrawalEvent(aggregateId, 94),
            new FundsDepositedEvent(aggregateId, 4)
        };

        foreach (var @event in events)
        {
            var jsonString = JsonConvert.SerializeObject(@event, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
            var jsonPayload = Encoding.UTF8.GetBytes(jsonString);
            var eventStoreDataType = new EventData(Uuid.NewUuid(), @event.GetType().Name, jsonPayload);
            connection.AppendToStreamAsync(StreamId(aggregateId), StreamState.Any, new[] { eventStoreDataType }).Wait();
        }

        #endregion

        #region Read Stream
        var results = Task.Run(() => connection.ReadStreamAsync(Direction.Forwards, StreamId(aggregateId), StreamPosition.Start, 999, false));
        var resultsData = results.Result.ToListAsync().Result;
        Task.WaitAll();

        var backAccountState = new BankAccount();
        foreach (var @event in resultsData)
        {
            var evnt = @event.Event;
            var jsonString = Encoding.UTF8.GetString(evnt.Data.ToArray());

            if (evnt.EventType == nameof(AccountCreatedEvent))
            {
                var obj = JsonConvert.DeserializeObject<AccountCreatedEvent>(jsonString);
                if (obj != null) backAccountState.Apply(obj);
                Console.WriteLine(backAccountState.ToString() + " | Account Create");

            }
            else if (evnt.EventType == nameof(FundsDepositedEvent))
            {
                var obj = JsonConvert.DeserializeObject<FundsDepositedEvent>(jsonString);
                if (obj != null) backAccountState.Apply(obj);
                Console.WriteLine(backAccountState.ToString() + $" | Amount Deposited: {obj?.Amount:F2}");
            }
            else
            {
                var obj = JsonConvert.DeserializeObject<FundsWithdrawalEvent>(jsonString);
                if (obj != null) backAccountState.Apply(obj);
                Console.WriteLine(backAccountState.ToString() + $" | Amount Withdrawal: {obj?.Amount:F2}");
            }
        }
        #endregion

        Console.ReadLine();
    }
}