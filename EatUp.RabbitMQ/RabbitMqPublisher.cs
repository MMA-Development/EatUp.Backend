using EatUp.RabbitMQ;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly ConnectionFactory _factory;
    private readonly string _exchangeName;

    public RabbitMqPublisher(string hostName, string exchangeName, string username, string password)
    {
        _factory = new ConnectionFactory { HostName = hostName, UserName = username, Password = password };
        _exchangeName = exchangeName;
    }

    public async Task Publish<T>(T @event) where T : IEvent
    {
        using var connection = await _factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Fanout);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

        var props = new BasicProperties();
        props.Type = typeof(T).Name;

        await channel.BasicPublishAsync(_exchangeName, "", false, props, body);
    }
}
