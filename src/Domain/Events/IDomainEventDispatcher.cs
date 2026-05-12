namespace Azoxia.AdaIsAkademi.Domain.Events
{
    /// <summary>
    /// Dispatches domain events to application handlers after persistence succeeds.
    /// </summary>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// Invokes handlers registered for each event type.
        /// </summary>
        /// <param name="events">Events collected from aggregates in the current unit of work.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task DispatchAsync(IReadOnlyList<IDomainEvent> events, CancellationToken cancellationToken);
    }
}
