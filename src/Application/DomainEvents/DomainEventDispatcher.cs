namespace Azoxia.AdaIsAkademi.Application.DomainEvents
{
    using Azoxia.AdaIsAkademi.Domain.Events;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using System.Reflection;

    /// <summary>
    /// Resolves <see cref="IDomainEventHandler{TDomainEvent}"/> implementations per event type.
    /// </summary>
    internal sealed class DomainEventDispatcher(
        IServiceProvider serviceProvider,
        ILogger<DomainEventDispatcher> logger) :
        IDomainEventDispatcher
    {
        #region IDomainEventDispatcher Members

        /// <inheritdoc />
        public async Task DispatchAsync(IReadOnlyList<IDomainEvent> events, CancellationToken cancellationToken)
        {
            foreach (IDomainEvent evt in events)
            {
                Type eventType = evt.GetType();
                Type handlerInterface = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
                IEnumerable<object?> handlers = serviceProvider.GetServices(handlerInterface);
                bool any = false;
                foreach (object? handler in handlers)
                {
                    if (handler is null)
                    {
                        continue;
                    }

                    any = true;
                    // Resolve via handler interface so explicit interface implementations are found.
                    MethodInfo? method = handlerInterface.GetMethod(
                        nameof(IDomainEventHandler<IDomainEvent>.HandleAsync),
                        [eventType, typeof(CancellationToken)]);
                    if (method is null)
                    {
                        logger.LogWarning(
                            "Domain event handler {HandlerType} missing HandleAsync for {EventType}.",
                            handler.GetType().Name,
                            eventType.Name);
                        continue;
                    }

                    Task task = (Task)method.Invoke(handler, [evt, cancellationToken])!;
                    await task.ConfigureAwait(false);
                }

                if (!any)
                {
                    logger.LogDebug("No domain event handler registered for {EventType}.", eventType.Name);
                }
            }
        }

        #endregion IDomainEventDispatcher Members
    }
}
