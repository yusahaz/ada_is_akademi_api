namespace Azoxia.AdaIsAkademi.Persistence
{
    using Azoxia.AdaIsAkademi.Domain.Events;
    using Azoxia.Core.Persistence;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Entity Framework Core database context for Ada Is Akademi persistence.
    /// </summary>
    internal sealed class AdaIsAkademiDbContext(
        DbContextOptions<AdaIsAkademiDbContext> options,
        IServiceScopeFactory serviceScopeFactory) :
        DbContextBase<AdaIsAkademiDbContext>(options)
    {
        #region Fields

        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

        #endregion Fields

        #region Methods

        /// <inheritdoc />
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return SaveChangesAsync(acceptAllChangesOnSuccess, CancellationToken.None)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            int written = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            if (written > 0)
            {
                await DispatchPendingDomainEventsAsync(cancellationToken);
            }

            return written;
        }

        private async Task DispatchPendingDomainEventsAsync(CancellationToken cancellationToken)
        {
            List<IDomainEvent> batch = [];
            foreach (EntityEntry entry in ChangeTracker.Entries())
            {
                if (entry.Entity is IDomainEventSource source)
                {
                    IReadOnlyList<IDomainEvent> pending = source.PullDomainEvents();
                    if (pending.Count > 0)
                    {
                        batch.AddRange(pending);
                    }
                }
            }

            if (batch.Count == 0)
            {
                return;
            }

            await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
            IDomainEventDispatcher dispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();
            await dispatcher.DispatchAsync(batch, cancellationToken);
        }

        #endregion Methods
    }
}
