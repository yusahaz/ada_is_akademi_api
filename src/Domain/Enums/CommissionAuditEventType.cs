namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Event type recorded in immutable commission audit stream.
    /// </summary>
    public enum CommissionAuditEventType
    {
        /// <summary>
        /// Commission receivable row was generated.
        /// </summary>
        CommissionReceivableGenerated = 10,

        /// <summary>
        /// Worker payout was confirmed by worker.
        /// </summary>
        WorkerPayoutConfirmed = 40,

        /// <summary>
        /// Worker payout row was created from assignment completion.
        /// </summary>
        WorkerPayoutCreated = 20,

        /// <summary>
        /// Worker payout processing failed.
        /// </summary>
        WorkerPayoutFailed = 50,

        /// <summary>
        /// Worker payout was marked by employer as paid.
        /// </summary>
        WorkerPayoutMarkedAsPaid = 30,

        /// <summary>
        /// Worker payout was retried after failure.
        /// </summary>
        WorkerPayoutRetried = 60,
    }
}
