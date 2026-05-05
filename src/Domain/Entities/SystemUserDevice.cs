namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Registered device used by a system user for sign-in or push-capable flows.
    /// </summary>
    public class SystemUserDevice :
        EntityBase
    {
        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected SystemUserDevice() { }

        /// <summary>
        /// Creates a device registration row for a user.
        /// </summary>
        /// <param name="systemUserId">Owning user key.</param>
        /// <param name="deviceIdentifier">Stable device id.</param>
        /// <param name="platform">Client platform.</param>
        protected internal SystemUserDevice(
            int systemUserId,
            string deviceIdentifier,
            DevicePlatform platform)
        {
            SystemUserId = systemUserId;
            DeviceIdentifier = deviceIdentifier;
            Platform = platform;
            LastActiveAt = DateTimeOffset.UtcNow;
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Updates last activity timestamp to the current UTC instant.
        /// </summary>
        protected internal void RecordActivity()
            => LastActiveAt = DateTimeOffset.UtcNow;

        /// <summary>
        /// Replaces the optional push or device token snapshot.
        /// </summary>
        protected internal void UpdateDeviceToken(string? deviceToken)
            => DeviceToken = deviceToken;

        #endregion Utils

        #region Properties

        /// <summary>
        /// Stable identifier for the physical or logical device.
        /// </summary>
        public string DeviceIdentifier { get; private set; }

        /// <summary>
        /// Optional push or device-specific token snapshot.
        /// </summary>
        public string? DeviceToken { get; private set; }

        /// <summary>
        /// Last observation time for activity on this device (UTC).
        /// </summary>
        public DateTimeOffset LastActiveAt { get; private set; }

        /// <summary>
        /// Hosting platform classification for diagnostics and policy.
        /// </summary>
        public DevicePlatform Platform { get; private set; }

        /// <summary>
        /// Foreign key to the owning user.
        /// </summary>
        public int SystemUserId { get; private set; }


        /// <summary>
        /// Owning system user aggregate.
        /// </summary>
        public virtual SystemUser SystemUser { get; private set; }

        #endregion Properties
    }
}
