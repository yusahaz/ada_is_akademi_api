namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Represents an idempotent overdue alarm record generated for a job posting and day.
    /// </summary>
    public class OverdueJobAlarm :
        EntityBase
    {
        #region Ctors

        private OverdueJobAlarm() { }

        protected internal OverdueJobAlarm(
            int jobPostingId,
            DateOnly alarmDate)
        {
            JobPostingId = jobPostingId;
            AlarmDate = alarmDate;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// UTC timestamp when this alarm row was generated.
        /// </summary>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        /// Calendar day for idempotent alarm generation.
        /// </summary>
        public DateOnly AlarmDate { get; private set; }

        /// <summary>
        /// Job posting identifier for which the overdue alarm is created.
        /// </summary>
        public int JobPostingId { get; private set; }

        /// <summary>
        /// Job posting linked to this alarm.
        /// </summary>
        public virtual JobPosting JobPosting { get; private set; }

        #endregion Properties
    }
}
