namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Aggregated employer-initiated worker profile view counts for an employer/worker pair (deduped by UTC calendar day).
    /// </summary>
    public class EmployerWorkerProfileViewStat :
        EntityBase
    {
        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected EmployerWorkerProfileViewStat() { }

        /// <summary>
        /// Creates a new statistic row scoped to <paramref name="employerId"/> and <paramref name="workerId"/>.
        /// </summary>
        /// <param name="employerId">Employer aggregate key.</param>
        /// <param name="workerId">Worker aggregate key.</param>
        protected internal EmployerWorkerProfileViewStat(int employerId, int workerId)
        {
            EmployerId = employerId;
            WorkerId = workerId;
            TotalViews = 0;
            LastRecordedUtc = null;
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Increments <see cref="TotalViews"/> once per UTC calendar day per pair; otherwise returns <see langword="false"/>.
        /// </summary>
        /// <param name="utcNow">Clock instant expressed with UTC offset semantics.</param>
        /// <returns><see langword="true"/> when a new view was counted.</returns>
        protected internal bool TryRecordView(DateTimeOffset utcNow)
        {
            DateOnly today = DateOnly.FromDateTime(utcNow.UtcDateTime);

            if (LastRecordedUtc.HasValue)
            {
                DateOnly previous = DateOnly.FromDateTime(LastRecordedUtc.Value.UtcDateTime);
                if (previous == today)
                {
                    return false;
                }
            }

            TotalViews++;
            LastRecordedUtc = utcNow;
            return true;
        }

        #endregion Utils

        #region Properties

        /// <summary>
        /// Owning employer aggregate key.
        /// </summary>
        public int EmployerId { get; private set; }

        /// <summary>
        /// UTC instant of the last counted view (after dedupe rules).
        /// </summary>
        public DateTimeOffset? LastRecordedUtc { get; private set; }

        /// <summary>
        /// Total counted employer profile opens after dedupe.
        /// </summary>
        public int TotalViews { get; private set; }

        /// <summary>
        /// Worker aggregate key.
        /// </summary>
        public int WorkerId { get; private set; }

        #endregion Properties
    }
}
