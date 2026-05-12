namespace Azoxia.AdaIsAkademi.Domain.Events
{
    /// <summary>
    /// Aggregate (or entity) that collects domain events until they are pulled after <c>SaveChanges</c>.
    /// </summary>
    public interface IDomainEventSource
    {
        /// <summary>
        /// Removes and returns all pending domain events for dispatch.
        /// </summary>
        /// <returns>Pending events; empty when none.</returns>
        IReadOnlyList<IDomainEvent> PullDomainEvents();
    }
}
