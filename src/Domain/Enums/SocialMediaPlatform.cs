namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Normalized social profile buckets for worker and employer outbound links (display + validation).
    /// </summary>
    public enum SocialMediaPlatform
    {
        /// <summary>
        /// Facebook profile or page.
        /// </summary>
        Facebook = 50,

        /// <summary>
        /// GitHub profile.
        /// </summary>
        GitHub = 20,

        /// <summary>
        /// Instagram profile.
        /// </summary>
        Instagram = 40,

        /// <summary>
        /// LinkedIn profile.
        /// </summary>
        LinkedIn = 10,

        /// <summary>
        /// Platform not covered by a specific bucket.
        /// </summary>
        Other = 99,

        /// <summary>
        /// General website URL.
        /// </summary>
        Website = 0,

        /// <summary>
        /// X (formerly Twitter) profile.
        /// </summary>
        X = 30,
    }
}
