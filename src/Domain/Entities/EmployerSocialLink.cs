namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Single outbound social or web profile URL for an employer organization profile.
    /// </summary>
    public class EmployerSocialLink :
        EntityBase
    {
        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected EmployerSocialLink() { }

        /// <summary>
        /// Creates an association scoped to <paramref name="employerId"/>.
        /// </summary>
        /// <param name="employerId">Owning employer aggregate key.</param>
        /// <param name="platform">Platform bucket.</param>
        /// <param name="url">Outbound URL.</param>
        protected internal EmployerSocialLink(
            int employerId,
            SocialMediaPlatform platform,
            string url)
        {
            EmployerId = employerId;
            Platform = platform;
            Url = url;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Foreign key to <see cref="Employer"/>.
        /// </summary>
        public int EmployerId { get; private set; }

        /// <summary>
        /// Bucket used for UX grouping and uniqueness per employer/platform.
        /// </summary>
        public SocialMediaPlatform Platform { get; private set; }

        /// <summary>
        /// Normalized outbound link.
        /// </summary>
        public string Url { get; private set; }


        /// <summary>
        /// Owning employer aggregate root.
        /// </summary>
        public virtual Employer Employer { get; private set; }

        #endregion Properties
    }
}
