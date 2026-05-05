namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Lifecycle state of a worker payout.
    /// </summary>
    public enum WorkerPayoutStatus
    {
        /// <summary>
        /// Payout row is created and waiting for employer payment mark.
        /// </summary>
        Pending = 10,

        /// <summary>
        /// Employer marked payout as paid and waiting worker confirmation.
        /// </summary>
        Processing = 20,

        /// <summary>
        /// Worker confirmed payout as completed.
        /// </summary>
        Paid = 30,

        /// <summary>
        /// Payment failed and can be retried within retry threshold.
        /// </summary>
        Failed = 40,
    }
}
