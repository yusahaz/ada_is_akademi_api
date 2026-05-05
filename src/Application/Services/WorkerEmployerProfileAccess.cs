namespace Azoxia.AdaIsAkademi.Application.Services
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Persistence;

    /// <inheritdoc />
    internal sealed class WorkerEmployerProfileAccess :
        IWorkerEmployerProfileAccess
    {
        #region Methods

        /// <inheritdoc />
        public async Task EnsureEmployerSharesJobApplicationWithWorkerAsync(
            IUnitOfWork unitOfWork,
            int employerId,
            int workerId,
            CancellationToken cancellationToken)
        {
            bool shared = await unitOfWork
                .GetRepository<JobApplication>()
                .Filter(ja => ja.WorkerId == workerId && ja.JobPosting.EmployerId == employerId)
                .AsNoTracking()
                .AnyAsync(cancellationToken);

            shared.ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);
        }

        /// <inheritdoc />
        public async Task<int> GetEmployerSourcedProfileViewCountAsync(
            IUnitOfWork unitOfWork,
            int employerId,
            int workerId,
            CancellationToken cancellationToken)
        {
            EmployerWorkerProfileViewStat? stat = await unitOfWork
                .GetRepository<EmployerWorkerProfileViewStat>()
                .Filter(x => x.EmployerId == employerId && x.WorkerId == workerId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            return stat?.TotalViews ?? 0;
        }

        #endregion Methods
    }
}
