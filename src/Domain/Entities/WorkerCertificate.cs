namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Professional certification or license attached to a worker.
    /// </summary>
    public class WorkerCertificate :
        EntityBase
    {
        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected WorkerCertificate() { }

        /// <summary>
        /// Creates a certificate row with optional document URL.
        /// </summary>
        protected internal WorkerCertificate(
            int workerId,
            string name,
            string issuingOrganization,
            DateOnly issuedAt,
            DateOnly? expiresAt,
            string? documentUrl = null)
        {
            WorkerId = workerId;
            Name = name;
            IssuingOrganization = issuingOrganization;
            IssuedAt = issuedAt;
            ExpiresAt = expiresAt;
            DocumentUrl = documentUrl;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// UTC timestamp when this certificate row was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        /// Optional URL to a stored credential document.
        /// </summary>
        public string? DocumentUrl { get; private set; }

        /// <summary>
        /// Calendar date when the credential expires, if applicable.
        /// </summary>
        public DateOnly? ExpiresAt { get; private set; }

        /// <summary>
        /// Calendar date the credential was issued.
        /// </summary>
        public DateOnly IssuedAt { get; private set; }

        /// <summary>
        /// Authority or organization that issued the credential.
        /// </summary>
        public string IssuingOrganization { get; private set; }

        /// <summary>
        /// Display name of the certification or license.
        /// </summary>
        public string Name { get; private set; }

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
