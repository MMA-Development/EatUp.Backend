using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events;
using System;
using System.Text.Json;

public class EventDispatcher
{
    private readonly Dictionary<string, Func<IEvent, Task>> _handlers = new();

    public void Register<T>(IEventHandler<T> handler) where T : IEvent
    {
        var typeName = typeof(T).Name;
        _handlers[typeName] = (e) => handler.HandleAsync((T)e);
    }

    public IEvent? DeserializeEvent(string json, string eventType)
    {
        return eventType switch
        {
            "VendorCreatedEvent" => JsonSerializer.Deserialize<VendorCreatedEvent>(json, new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
            }),
            _ => null
        };
    }

    public async Task DispatchAsync(IEvent @event)
    {
        var typeName = @event.GetType().Name;
        if (_handlers.TryGetValue(typeName, out var handler))
        {
            await handler(@event);
        }
        else
        {
            Console.WriteLine($"No handler registered for {typeName}");
        }
    }
}
