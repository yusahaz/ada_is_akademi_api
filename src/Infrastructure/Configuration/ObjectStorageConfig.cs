namespace Azoxia.AdaIsAkademi.Infrastructure.Configuration
{
    using Azoxia.Core.Configuration;

    /// <summary>
    /// MinIO/S3-compatible endpoint settings bound from the <c>ObjectStorageConfig</c> configuration section.
    /// </summary>
    public sealed record ObjectStorageConfig :
        IConfig
    {
        #region Properties

        /// <summary>
        /// Gets or sets the API access identity (MinIO access key or IAM access key ID).
        /// </summary>
        public string? AccessKey { get; set; }

        /// <summary>
        /// Gets or sets the target bucket name.
        /// </summary>
        public string? BucketName { get; set; }

        /// <summary>
        /// Gets or sets whether RFC path-style addressing applies (typical for MinIO and custom endpoints).
        /// </summary>
        public bool ForcePathStyle { get; set; } = true;

        /// <summary>
        /// Gets or sets the AWS region name for the SDK (optional when using MinIO or path-style setups).
        /// </summary>
        public string? RegionName { get; set; }

        /// <summary>
        /// Gets or sets the secret used to sign requests.
        /// </summary>
        public string? SecretKey { get; set; }

        /// <summary>
        /// Gets or sets the service endpoint base URL (<c>http://localhost:9000</c>, etc.).
        /// </summary>
        public string? ServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the base URL embedded in presigned GET/PUT URLs for browsers and external clients.
        /// Use when <see cref="ServiceUrl"/> is only reachable inside Docker/Kubernetes (e.g. <c>http://minio:9000</c>)
        /// while developers hit MinIO via <c>http://localhost:{published-port}</c>.
        /// When empty, <see cref="ServiceUrl"/> is used for signing.
        /// </summary>
        public string? PublicServiceUrl { get; set; }

        #endregion Properties
    }
}
