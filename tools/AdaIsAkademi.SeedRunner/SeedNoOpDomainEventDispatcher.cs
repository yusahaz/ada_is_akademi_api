namespace Azoxia.AdaIsAkademi.SeedRunner;

using Azoxia.AdaIsAkademi.Domain.Events;

/// <summary>
/// Swallows domain events during seeding; cache invalidation and side effects are not needed for CLI runs.
/// </summary>
internal sealed class SeedNoOpDomainEventDispatcher : IDomainEventDispatcher
{
    /// <inheritdoc />
    public Task DispatchAsync(IReadOnlyList<IDomainEvent> events, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
