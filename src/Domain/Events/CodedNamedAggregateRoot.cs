namespace Azoxia.AdaIsAkademi.Domain.Events
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// <see cref="CodedNamedEntityBase"/> aggregate root that records domain events via deferred factories.
    /// </summary>
    public abstract class CodedNamedAggregateRoot :
        CodedNamedEntityBase,
        IDomainEventSource
    {
        #region Fields

        private readonly List<Func<IDomainEvent>> _domainEventFactories = new();

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected CodedNamedAggregateRoot()
        {
        }

        /// <summary>
        /// Initializes coded-name aggregate state with name and optional description.
        /// </summary>
        /// <param name="name">Display name.</param>
        /// <param name="description">Optional description.</param>
        protected internal CodedNamedAggregateRoot(string name, string? description = null) :
            base(name, description)
        {
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Queues a domain event factory evaluated when <see cref="PullDomainEvents"/> runs (after persistence).
        /// </summary>
        /// <param name="factory">Deferred event materialization.</param>
        protected void RaiseDomainEvent(Func<IDomainEvent> factory)
        {
            ArgumentNullException.ThrowIfNull(factory);
            _domainEventFactories.Add(factory);
        }

        #endregion Utils

        #region IDomainEventSource Members

        /// <inheritdoc />
        public IReadOnlyList<IDomainEvent> PullDomainEvents()
        {
            if (_domainEventFactories.Count == 0)
            {
                return [];
            }

            List<IDomainEvent> events = _domainEventFactories
                .ConvertAll(static f => f());
            _domainEventFactories.Clear();
            return events;
        }

        #endregion IDomainEventSource Members
    }
}
