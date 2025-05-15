using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class RabbitMqConsumer
{
    private readonly ConnectionFactory _factory;
    private readonly string _exchange;
    private readonly string _queue;
    private readonly EventDispatcher _dispatcher;

    public RabbitMqConsumer(string hostName, string exchange, string queue, EventDispatcher dispatcher)
    {
        _factory = new ConnectionFactory { HostName = hostName, UserName = "admin", Password = "password" };
        _exchange = exchange;
        _queue = queue;
        _dispatcher = dispatcher;
    }

    public async Task Start()
    {
        var connection = await _factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(_exchange, ExchangeType.Fanout);
        await channel.QueueDeclareAsync(_queue, durable: false, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync(_queue, _exchange, "");

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            Console.WriteLine("Message received");
            var body = Encoding.UTF8.GetString(args.Body.ToArray());
            var eventType = args.BasicProperties.Type ?? Encoding.UTF8.GetString((byte[])args.BasicProperties.Headers["Type"]);

            var eventObj = _dispatcher.DeserializeEvent(body, eventType);
            if (eventObj is not null)
            {
                await _dispatcher.DispatchAsync(eventObj);
            }
        };

        await channel.BasicConsumeAsync(queue: _queue, autoAck: true, consumer: consumer);
    }
}
