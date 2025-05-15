using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Vendor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;

public class EventDispatcher(IServiceScopeFactory scopeFactory)
{
    internal IEvent? DeserializeEvent(string json, string eventType)
    {
        return eventType switch
        {
            "VendorCreatedEvent" => JsonSerializer.Deserialize<VendorCreatedEvent>(json),
            "VendorDeletedEvent" => JsonSerializer.Deserialize<VendorDeletedEvent>(json),
            "VendorUpdatedEvent" => JsonSerializer.Deserialize<VendorUpdatedEvent>(json),
            _ => null
        };
    }

    internal async Task DispatchAsync(IEvent @event)
    {
        try
        {
            var scope = scopeFactory.CreateScope();
            var typeName = @event.GetType().Name;
            var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
            var handler = scope.ServiceProvider.GetService(handlerType);
            if (handler == null)
            {
                Console.WriteLine($"No handler registered for {typeName}");
                return;
            }
            handler.GetType()
                .GetMethod("HandleAsync")
                ?.Invoke(handler, [@event]);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling event {@event}: {ex.Message}");
        }
    }
}
