using System.Net;
using System.Text;
using EventStore.ClientAPI;
using EventStore.Prototype.Domain;
using Newtonsoft.Json;

internal class Program
{
    static string StreamId(Guid id)
    {
        return string.Format("BankAccount-{0}", id);
    }

    private static void Main(string[] args)
    {
        var endPoint = new Uri("tcp://admin:changeit@localhost:1113");
        var connection = EventStoreConnection.Create(endPoint);
        connection.ConnectAsync();

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
            var eventStoreDataType = new EventData(Guid.NewGuid(), @event.GetType().Name, true, jsonPayload, null);
            connection.AppendToStreamAsync(StreamId(aggregateId), ExpectedVersion.Any, eventStoreDataType);
        }

        Console.ReadLine();
    }
}