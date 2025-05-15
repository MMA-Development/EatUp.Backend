using EatUp.RabbitMQ;

public interface IRabbitMqPublisher
{
    Task Publish<T>(T @event) where T : IEvent;
}