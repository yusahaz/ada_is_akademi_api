namespace Azoxia.AdaIsAkademi.Application.Tests.Support
{
    using Azoxia.Core.Application.Caching;

    /// <summary>
    /// No-op cache for handler tests (avoids Redis or L2 setup).
    /// </summary>
    public sealed class NullCacheService :
        ICacheService
    {
        #region Methods

        /// <inheritdoc />
        public Task<T?> GetAsync<T>(CacheKey key, CancellationToken cancellationToken = default)
            => Task.FromResult<T?>(default);

        /// <inheritdoc />
        public Task InvalidateByDependencyAsync(CacheDependency dependency, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        /// <inheritdoc />
        public Task RemoveAsync(CacheKey key, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        /// <inheritdoc />
        public Task SetAsync<T>(CacheKey key, T value, CacheEntryOptions options, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        #endregion Methods
    }
}
