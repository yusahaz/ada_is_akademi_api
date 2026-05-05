namespace Azoxia.AdaIsAkademi.Application.Tests.Support
{
    using Azoxia.AdaIsAkademi.Application.Services;

    /// <summary>
    /// Deterministik stub presigner for handler tests (no AWS dependencies).
    /// </summary>
    internal sealed class TestObjectStoragePresigner :
        IObjectStoragePresigner
    {
        #region Methods

        /// <inheritdoc />
        public Task<string> CreatePresignedGetAsync(
            string objectKey,
            TimeSpan timeToLive,
            CancellationToken cancellationToken) =>
            Task.FromResult($"https://stub.local/get?key={Uri.EscapeDataString(objectKey)}");

        /// <inheritdoc />
        public Task<PresignedBlobUploadResult> CreatePresignedPutAsync(
            string objectKey,
            string contentType,
            TimeSpan timeToLive,
            CancellationToken cancellationToken) =>
            Task.FromResult(
                new PresignedBlobUploadResult(
                    Url: $"https://stub.local/put?key={Uri.EscapeDataString(objectKey)}",
                    ExpiresAtUtc: DateTimeOffset.UtcNow.Add(timeToLive)));

        #endregion Methods
    }
}
