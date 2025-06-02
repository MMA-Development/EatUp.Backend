using Microsoft.Extensions.Logging;
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
    private readonly ILogger? _logger;

    public RabbitMqConsumer(string hostName, string exchange, string queue, string username, string password, EventDispatcher dispatcher, ILogger? logger = null)
    {

        _factory = new ConnectionFactory { Uri = new Uri(hostName) };
        _exchange = exchange;
        _queue = queue;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task Start()
    {
        var connection = await _factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(_exchange, ExchangeType.Fanout);
        await channel.QueueDeclareAsync(_queue, durable: false, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync(_queue, _exchange, "");
        await channel.QueueBindAsync(_queue, _exchange, _queue);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            var body = Encoding.UTF8.GetString(args.Body.ToArray());
            var eventType = args.BasicProperties.Type ?? Encoding.UTF8.GetString((byte[])args.BasicProperties.Headers["Type"]);

            using (_logger?.BeginScope(new Dictionary<string, object>
            {
                ["MessageId"] = args.BasicProperties.MessageId,
                ["MessageType"] = eventType,
                ["Queue"] = _queue
            }))
            {
                _logger?.LogInformation("Received message from RabbitMQ: {MessageId} {MessageType}", args.BasicProperties.MessageId, eventType);

                var eventObj = _dispatcher.DeserializeEvent(body, eventType);
                if (eventObj is not null)
                {
                    try
                    {
                        await _dispatcher.DispatchAsync(eventObj, logger: _logger);
                        _logger?.LogInformation("Sucessfully dispatched event: {MessageId} {EventType}", args.BasicProperties.MessageId, eventType);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error dispatching event: {MessageId} {EventType}", args.BasicProperties.MessageId, eventType);
                    }
                }
            }
        };

        await channel.BasicConsumeAsync(queue: _queue, autoAck: true, consumer: consumer);
    }
}
