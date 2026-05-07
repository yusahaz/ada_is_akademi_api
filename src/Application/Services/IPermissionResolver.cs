namespace Azoxia.AdaIsAkademi.Application.Services
{
    /// <summary>
    /// Resolves whether the current system user is allowed to perform the given permission.
    /// </summary>
    public interface IPermissionResolver
    {
        /// <summary>
        /// Checks whether <paramref name="systemUserId"/> is allowed to perform <paramref name="permission"/>.
        /// </summary>
        /// <param name="systemUserId">Authenticated system user surrogate key.</param>
        /// <param name="employerId">Optional employer scope discriminator derived from JWT claims.</param>
        /// <param name="permission">Permission name in <c>resource.action</c> format.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        Task<bool> HasPermissionAsync(
            int systemUserId,
            int? employerId,
            string permission,
            CancellationToken cancellationToken = default);
    }
}

