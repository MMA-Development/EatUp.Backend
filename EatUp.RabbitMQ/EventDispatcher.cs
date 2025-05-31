using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Vendor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Text.Json;

public class EventDispatcher(IServiceScopeFactory scopeFactory)
{
    private static readonly Dictionary<string, Type> _eventTypeMap = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(t => typeof(IEvent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
        .ToDictionary(t => t.Name, t => t);

    internal IEvent? DeserializeEvent(string json, string eventType)
    {
        if (_eventTypeMap.TryGetValue(eventType, out var type))
        {
            return JsonSerializer.Deserialize(json, type) as IEvent;
        }
        return null;
    }

    internal async Task DispatchAsync(IEvent @event, ILogger? logger = null)
    {
        try
        {
            var scope = scopeFactory.CreateScope();
            var typeName = @event.GetType().Name;
            var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
            var handler = scope.ServiceProvider.GetService(handlerType);
            if (handler == null)
            {
                logger?.LogInformation("No handler registered for {EventType}", typeName);
            }

            object? handlerTask = handler.GetType()
                .GetMethod("HandleAsync")
                ?.Invoke(handler, [@event]);

            if (handlerTask is Task task)
            {
                await task;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling event {@event}: {ex.Message}");
        }
    }
}
