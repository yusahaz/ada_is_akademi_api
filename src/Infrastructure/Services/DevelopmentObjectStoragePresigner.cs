namespace Azoxia.AdaIsAkademi.Infrastructure
{
    using Azoxia.AdaIsAkademi.Application.Services;
    using System;

    /// <summary>
    /// Yerel ortam ya da yapılandırılmamış MinIO için deterministik stub URL üretimi (geri dönüş: üretim dışı).
    /// </summary>
    internal sealed class DevelopmentObjectStoragePresigner :
        IObjectStoragePresigner
    {
        #region Methods

        /// <inheritdoc />
        public Task<string> CreatePresignedGetAsync(
            string objectKey,
            TimeSpan timeToLive,
            CancellationToken cancellationToken) =>
            Task.FromResult(BuildStub($"get/{Uri.EscapeDataString(objectKey)}", timeToLive));

        /// <inheritdoc />
        public Task<PresignedBlobUploadResult> CreatePresignedPutAsync(
            string objectKey,
            string contentType,
            TimeSpan timeToLive,
            CancellationToken cancellationToken) =>
            Task.FromResult(
                new PresignedBlobUploadResult(
                    Url: BuildStub($"put/{Uri.EscapeDataString(objectKey)}&ct={Uri.EscapeDataString(contentType)}", timeToLive),
                    ExpiresAtUtc: DateTimeOffset.UtcNow.Add(timeToLive)));

        private static string BuildStub(string tail, TimeSpan timeToLive) =>
            $"https://localhost/object-storage/dev/{tail}&ttl={(int)timeToLive.TotalSeconds}";

        #endregion Methods
    }
}
