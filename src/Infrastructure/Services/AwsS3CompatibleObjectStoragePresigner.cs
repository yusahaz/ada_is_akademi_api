namespace Azoxia.AdaIsAkademi.Infrastructure
{
    using Amazon;
    using Amazon.Runtime;
    using Amazon.S3;
    using Amazon.S3.Model;

    using Azoxia.AdaIsAkademi.Application.Services;
    using Azoxia.AdaIsAkademi.Infrastructure.Configuration;

    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Issues presigned request URLs against AWS S3 or MinIO-compatible endpoints.
    /// </summary>
    internal sealed class AwsS3CompatibleObjectStoragePresigner :
        IObjectStoragePresigner,
        IDisposable
    {
        #region Fields

        private readonly AmazonS3Client _amazonS3Client;

        private readonly ObjectStorageConfig _options;

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Configures and owns the underlying <see cref="AmazonS3Client"/> lifetime.
        /// </summary>
        /// <param name="optionsBinding">MinIO/S3-compatible storage settings used for the client and presign operations.</param>
        public AwsS3CompatibleObjectStoragePresigner(ObjectStorageConfig optionsBinding)
        {
            optionsBinding = optionsBinding.ThrowIfNull(AzoxiaErrorCodes.ArgumentNull);

            _options = optionsBinding;

            _ = _options.ServiceUrl.ThrowIfNullOrWhiteSpace(AzoxiaErrorCodes.StringNullOrWhiteSpace);
            _ = _options.AccessKey.ThrowIfNullOrWhiteSpace(AzoxiaErrorCodes.StringNullOrWhiteSpace);
            _ = _options.SecretKey.ThrowIfNullOrWhiteSpace(AzoxiaErrorCodes.StringNullOrWhiteSpace);
            _ = _options.BucketName.ThrowIfNullOrWhiteSpace(AzoxiaErrorCodes.StringNullOrWhiteSpace);

            AmazonS3Config config =
                new()
                {
                    ServiceURL = _options.ServiceUrl.TrimEnd('/'),
                    ForcePathStyle = _options.ForcePathStyle,
                };

            if (!_options.RegionName.IsNullOrWhiteSpace())
            {
                config.RegionEndpoint =
                    RegionEndpoint.GetBySystemName(_options.RegionName);
            }

            AWSCredentials credentials =
                new BasicAWSCredentials(_options.AccessKey, _options.SecretKey);

            _amazonS3Client = new AmazonS3Client(credentials, config);
        }

        #endregion Ctors

        #region Methods

        /// <inheritdoc />
        public Task<string> CreatePresignedGetAsync(
            string objectKey,
            TimeSpan timeToLive,
            CancellationToken cancellationToken)
        {
            _ = objectKey.ThrowIfNullOrWhiteSpace(AzoxiaErrorCodes.StringNullOrWhiteSpace);

            cancellationToken.ThrowIfCancellationRequested();

            DateTime expiryUtc = DateTime.UtcNow.Add(timeToLive);

            GetPreSignedUrlRequest request =
                new()
                {
                    BucketName = _options.BucketName,
                    Expires = expiryUtc,
                    Key = objectKey,
                    Verb = HttpVerb.GET,
                };

            return Task.FromResult(_amazonS3Client.GetPreSignedURL(request));
        }

        /// <inheritdoc />
        public Task<PresignedBlobUploadResult> CreatePresignedPutAsync(
            string objectKey,
            string contentType,
            TimeSpan timeToLive,
            CancellationToken cancellationToken)
        {
            _ = objectKey.ThrowIfNullOrWhiteSpace(AzoxiaErrorCodes.StringNullOrWhiteSpace);
            _ = contentType.ThrowIfNullOrWhiteSpace(AzoxiaErrorCodes.StringNullOrWhiteSpace);

            cancellationToken.ThrowIfCancellationRequested();

            DateTime expiryUtc = DateTime.UtcNow.Add(timeToLive);

            GetPreSignedUrlRequest request =
                new()
                {
                    BucketName = _options.BucketName,
                    ContentType = contentType,
                    Expires = expiryUtc,
                    Key = objectKey,
                    Verb = HttpVerb.PUT,
                };

            string url = _amazonS3Client.GetPreSignedURL(request);
            DateTimeOffset expiresUtc = new(expiryUtc, TimeSpan.Zero);

            return Task.FromResult(new PresignedBlobUploadResult(url, expiresUtc));
        }

        /// <inheritdoc />
        public void Dispose()
            => _amazonS3Client.Dispose();

        #endregion Methods
    }
}
