namespace Azoxia.AdaIsAkademi.Application.DependencyInjection
{
    using Azoxia.AdaIsAkademi.Application.DomainEvents;
    using Azoxia.AdaIsAkademi.Domain.Events;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Registers dispatcher and reflection-discovered <see cref="IDomainEventHandler{TDomainEvent}"/> implementations.
    /// </summary>
    public static class DomainEventsRegistration
    {
        /// <summary>
        /// Adds Ada domain event dispatcher infrastructure to the service collection.
        /// </summary>
        /// <param name="services">Application services.</param>
        /// <returns>The same collection.</returns>
        public static IServiceCollection AddAdaIsDomainEventHandling(this IServiceCollection services)
        {
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            System.Reflection.Assembly assembly = typeof(DomainEventDispatcher).Assembly;
            foreach (Type handlerType in assembly.GetTypes().Where(static t => t is { IsClass: true, IsAbstract: false }))
            {
                foreach (Type iface in handlerType.GetInterfaces())
                {
                    if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))
                    {
                        services.AddScoped(iface, handlerType);
                    }
                }
            }

            return services;
        }
    }
}
