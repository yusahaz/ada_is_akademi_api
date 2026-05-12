namespace Azoxia.AdaIsAkademi.Application.DomainEvents
{
    using Azoxia.AdaIsAkademi.Domain.Events;

    /// <summary>
    /// Handles a persisted <see cref="IDomainEvent"/> from the Ada domain model.
    /// </summary>
    /// <typeparam name="TDomainEvent">Concrete event type.</typeparam>
    public interface IDomainEventHandler<in TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        /// <summary>
        /// Executes the handler logic for a single event instance.
        /// </summary>
        /// <param name="domainEvent">Event payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken);
    }
}
