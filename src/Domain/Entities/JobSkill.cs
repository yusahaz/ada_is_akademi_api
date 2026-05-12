namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.AdaIsAkademi.Domain.Events;

    /// <summary>
    /// Global predefined skill dictionary item used by job posting and matching flows.
    /// </summary>
    public class JobSkill :
        CodedNamedAggregateRoot
    {
        #region Ctors

        /// <summary>
        /// Initializes an empty row for persistence materialization.
        /// </summary>
        protected JobSkill() { }

        /// <summary>
        /// Initializes a named skill entry.
        /// </summary>
        /// <param name="name">PascalCase display/code name.</param>
        /// <param name="description">Optional description.</param>
        protected internal JobSkill(
            string name,
            string? description = null) :
            base(name, description)
        {
        }

        #endregion Ctors
    }
}
