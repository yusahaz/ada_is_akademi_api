namespace Azoxia.AdaIsAkademi.Infrastructure
{
    using System;

    using Azoxia.AdaIsAkademi.Application.Services;

    /// <summary>
    /// Deterministic stub URLs for local dev or incomplete MinIO configuration (non-production fallback).
    /// </summary>
    internal sealed class DevelopmentObjectStoragePresigner :
        IObjectStoragePresigner
    {
        #region Utils

        private static string BuildStub(string tail, TimeSpan timeToLive) =>
            $"https://localhost/object-storage/dev/{tail}&ttl={(int)timeToLive.TotalSeconds}";

        #endregion Utils

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

        #endregion Methods
    }
}
