using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EatUp.RabbitMQ
{
    public static class DispatcherRegistration
    {
        public static void RegisterAllEventHandlers(EventDispatcher dispatcher, IServiceProvider serviceProvider)
        {
            var handlerInterfaceType = typeof(IEventHandler<>);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var handlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
                    .Select(i => new { Handler = t, EventType = i.GetGenericArguments()[0], Interface = i }))
                .ToList();

            foreach (var typeInfo in handlerTypes)
            {
                var service = serviceProvider.GetService(typeInfo.Interface);
                if (service == null) continue;

                var registerMethod = typeof(EventDispatcher)
                    .GetMethod("Register")
                    ?.MakeGenericMethod(typeInfo.EventType);

                registerMethod?.Invoke(dispatcher, new[] { service });
            }
        }
    }
}
