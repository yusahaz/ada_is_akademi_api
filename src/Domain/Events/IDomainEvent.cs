namespace Azoxia.AdaIsAkademi.Domain.Events
{
    /// <summary>
    /// Marker for transactional domain events raised by aggregates (dispatched after successful persistence).
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>
        /// UTC instant when the event instance was materialized (typically immediately before dispatch).
        /// </summary>
        DateTimeOffset OccurredAt { get; }
    }

    /// <summary>
    /// Optional base for PRD-aligned domain events with a default occurrence timestamp.
    /// </summary>
    public abstract record DomainEvent : IDomainEvent
    {
        /// <inheritdoc />
        public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    }
}
