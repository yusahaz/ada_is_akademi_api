namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Declared weekly time window when a worker is available.
    /// </summary>
    public class WorkerAvailability :
        EntityBase
    {
        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected WorkerAvailability() { }

        /// <summary>
        /// Creates an availability window for a worker.
        /// </summary>
        /// <param name="workerId">Owning worker key.</param>
        /// <param name="dayOfWeek">Weekday.</param>
        /// <param name="timeFrom">Start time.</param>
        /// <param name="timeTo">End time.</param>
        protected internal WorkerAvailability(
            int workerId,
            DayOfWeek dayOfWeek,
            TimeOnly timeFrom,
            TimeOnly timeTo)
        {
            WorkerId = workerId;
            DayOfWeek = dayOfWeek;
            TimeFrom = timeFrom;
            TimeTo = timeTo;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Calendar weekday for this availability window.
        /// </summary>
        public DayOfWeek DayOfWeek { get; private set; }

        /// <summary>
        /// Inclusive start time on the given weekday.
        /// </summary>
        public TimeOnly TimeFrom { get; private set; }

        /// <summary>
        /// Exclusive or inclusive end time on the given weekday (paired with validation rules).
        /// </summary>
        public TimeOnly TimeTo { get; private set; }

        /// <summary>
        /// Foreign key to the owning worker.
        /// </summary>
        public int WorkerId { get; private set; }


        /// <summary>
        /// Owning worker aggregate.
        /// </summary>
        public virtual Worker Worker { get; private set; }

        #endregion Properties
    }
}
