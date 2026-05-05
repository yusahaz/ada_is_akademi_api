namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;
    using Azoxia.Core.ValueTypes;

    /// <summary>
    /// External employment reference (company, role, and contact) for a worker.
    /// </summary>
    public class WorkerReference :
        EntityBase
    {
        #region Ctors

        protected WorkerReference() { }

        protected internal WorkerReference(
            int workerId,
            string company,
            string position,
            Contact contact)
        {
            WorkerId = workerId;
            Company = company;
            Position = position;
            Contact = contact;
        }

        #endregion Ctors

        #region Properties
        /// <summary>
        /// Organization name for this reference.
        /// </summary>
        public string Company { get; private set; }

        /// <summary>
        /// Primary contact details for verifying this reference.
        /// </summary>
        public Contact Contact { get; private set; }

        /// <summary>
        /// Role or job title held at the referenced company.
        /// </summary>
        public string Position { get; private set; }

        /// <summary>
        /// Foreign key to the owning worker.
        /// </summary>
        public int WorkerId { get; private set; }

        /// <summary>
        /// Owning worker aggregate.
        /// </summary>
        public virtual Worker Worker { get; private set; }
        #endregion Properties
    }
}
