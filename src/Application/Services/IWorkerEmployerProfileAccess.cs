namespace Azoxia.AdaIsAkademi.Application.Services
{
    using Azoxia.Core.Persistence;

    /// <summary>
    /// Employer/worker profile visibility backed by shared job applications plus employer-sourced view counters.
    /// </summary>
    public interface IWorkerEmployerProfileAccess
    {
        #region Methods

        /// <summary>
        /// Ensures a job application ties the employer and worker; otherwise surfaces actor access denied validation.
        /// </summary>
        Task EnsureEmployerSharesJobApplicationWithWorkerAsync(
            IUnitOfWork unitOfWork,
            int employerId,
            int workerId,
            CancellationToken cancellationToken);

        /// <summary>
        /// Returns the persisted employer-sourced profile view counter if present; otherwise zero.
        /// </summary>
        Task<int> GetEmployerSourcedProfileViewCountAsync(
            IUnitOfWork unitOfWork,
            int employerId,
            int workerId,
            CancellationToken cancellationToken);

        #endregion Methods
    }
}
