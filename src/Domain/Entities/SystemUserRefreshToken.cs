namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Long-lived refresh token metadata bound to a user and device pairing.
    /// </summary>
    public class SystemUserRefreshToken :
        EntityBase
    {
        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected SystemUserRefreshToken() { }

        /// <summary>
        /// Creates a refresh token row with expiry.
        /// </summary>
        /// <param name="systemUserId">Owning user key.</param>
        /// <param name="tokenHash">Opaque token hash.</param>
        /// <param name="deviceId">Device key.</param>
        /// <param name="expiresAt">Absolute expiry (UTC).</param>
        protected internal SystemUserRefreshToken(
            int systemUserId,
            string tokenHash,
            int deviceId,
            DateTimeOffset expiresAt)
        {
            SystemUserId = systemUserId;
            TokenHash = tokenHash;
            DeviceId = deviceId;
            ExpiresAt = expiresAt;
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Marks the refresh token as revoked.
        /// </summary>
        protected internal void Revoke()
            => IsRevoked = true;

        /// <summary>
        /// Updates the absolute expiry instant for this token row.
        /// </summary>
        protected internal void Until(DateTimeOffset expiresAt)
            => ExpiresAt = expiresAt;

        #endregion Utils

        #region Properties

        /// <summary>
        /// Device row this token issuance is constrained to.
        /// </summary>
        public int DeviceId { get; private set; }

        /// <summary>
        /// Absolute expiration instant (UTC) for rotating access.
        /// </summary>
        public DateTimeOffset ExpiresAt { get; private set; }

        /// <summary>
        /// True once the token is manually or administratively invalidated.
        /// </summary>
        public bool IsRevoked { get; private set; }

        /// <summary>
        /// Owning application user surrogate key.
        /// </summary>
        public int SystemUserId { get; private set; }

        /// <summary>
        /// Opaque persisted hash representing the refresh token payload.
        /// </summary>
        public string TokenHash { get; private set; }


        /// <summary>
        /// Derived active flag combining revocation and expiry.
        /// </summary>
        public bool IsActive => !IsRevoked && !IsExpired;

        /// <summary>
        /// Whether the expiry instant has elapsed (UTC).
        /// </summary>
        public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;


        /// <summary>
        /// Device linkage for this issuance.
        /// </summary>
        public virtual SystemUserDevice Device { get; private set; }

        /// <summary>
        /// Parent user owning this credential.
        /// </summary>
        public virtual SystemUser SystemUser { get; private set; }

        #endregion Properties
    }
}
