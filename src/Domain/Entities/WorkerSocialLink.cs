namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Single outbound social or web profile URL for a worker profile.
    /// </summary>
    public class WorkerSocialLink :
        EntityBase
    {
        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected WorkerSocialLink() { }

        /// <summary>
        /// Creates an association scoped to <paramref name="workerId"/>.
        /// </summary>
        /// <param name="workerId">Owning worker aggregate key.</param>
        /// <param name="platform">Platform bucket.</param>
        /// <param name="url">Outbound URL.</param>
        protected internal WorkerSocialLink(
            int workerId,
            SocialMediaPlatform platform,
            string url)
        {
            WorkerId = workerId;
            Platform = platform;
            Url = url;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Normalized outbound link.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Bucket used for UX grouping and uniqueness per worker/platform.
        /// </summary>
        public SocialMediaPlatform Platform { get; private set; }

        /// <summary>
        /// Foreign key to <see cref="Worker"/>.
        /// </summary>
        public int WorkerId { get; private set; }

        /// <summary>
        /// Owning worker aggregate root.
        /// </summary>
        public virtual Worker Worker { get; private set; }

        #endregion Properties
    }
}
