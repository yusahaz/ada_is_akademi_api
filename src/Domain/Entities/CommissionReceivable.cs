namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.ValueTypes;

    /// <summary>
    /// Represents a commission receivable row for an employer and billing period.
    /// </summary>
    public class CommissionReceivable :
        EntityBase
    {
        #region Ctors

        private CommissionReceivable() { }

        protected internal CommissionReceivable(
            Money amount,
            int employerId,
            DateOnly periodEnd,
            DateOnly periodStart)
        {
            (periodEnd >= periodStart)
                .ThrowIfFalse(DomainErrorCodes.CommissionReceivablePeriodInvalid);

            Amount = amount;
            EmployerId = employerId;
            PeriodEnd = periodEnd;
            PeriodStart = periodStart;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Commission amount receivable for the period.
        /// </summary>
        public Money Amount { get; private set; }

        /// <summary>
        /// UTC timestamp when receivable row is created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        /// Employer identifier owning the receivable.
        /// </summary>
        public int EmployerId { get; private set; }

        /// <summary>
        /// Billing period end date (inclusive).
        /// </summary>
        public DateOnly PeriodEnd { get; private set; }

        /// <summary>
        /// Billing period start date (inclusive).
        /// </summary>
        public DateOnly PeriodStart { get; private set; }

        /// <summary>
        /// Employer linked to this receivable.
        /// </summary>
        public virtual Employer Employer { get; private set; }

        #endregion Properties
    }
}
