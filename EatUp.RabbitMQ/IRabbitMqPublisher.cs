using EatUp.RabbitMQ;

public interface IRabbitMqPublisher
{
    Task Publish<T>(T @event) where T : IEvent;
    Task Publish<T>(T @event, string routingKey) where T : IEvent;
}