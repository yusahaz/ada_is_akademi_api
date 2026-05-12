namespace Azoxia.AdaIsAkademi.Application.Services
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence;

    using System.Security.Claims;

    /// <summary>
    /// Builds JWT claims for <see cref="SystemUser"/> principals (worker / employer scope).
    /// </summary>
    internal static class SystemUserClaimsBuilder
    {
        /// <summary>
        /// Materializes standard Ada claims for access tokens.
        /// </summary>
        public static async Task<Claim[]> BuildAsync(
            SystemUser user,
            IUnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            List<Claim> claims =
            [
                new("system_user_id", user.Id.ToString()),
                new("system_user_type", ((int)user.Type).ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
            ];

            Worker? worker = await unitOfWork
                .GetRepository<Worker>()
                .Filter(x => x.SystemUserId == user.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            if (worker is not null)
            {
                claims.Add(new Claim("worker_id", worker.Id.ToString()));
            }

            if (user.EmployerId is int employerId && employerId > 0 &&
                (user.Type == SystemUserType.Employer || user.Type == SystemUserType.Supervisor))
            {
                claims.Add(new Claim("employer_id", employerId.ToString()));
            }

            return [.. claims];
        }
    }
}
