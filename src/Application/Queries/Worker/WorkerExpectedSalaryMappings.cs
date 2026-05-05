namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.ValueTypes;

    /// <summary>
    /// Maps persisted worker salary bound columns into <see cref="Money"/> for read models (EF cannot materialize nullable <see cref="Money"/> composites).
    /// </summary>
    internal static class WorkerExpectedSalaryMappings
    {
        #region Methods

        /// <summary>
        /// Builds the inclusive maximum expected salary Money from stored columns when both parts are populated.
        /// </summary>
        internal static Money? ExpectedSalaryMaxMoney(this Worker worker) =>
            ToMoney(worker.ExpectedSalaryMaxAmount, worker.ExpectedSalaryMaxCurrency);

        /// <summary>
        /// Builds the inclusive minimum expected salary Money from stored columns when both parts are populated.
        /// </summary>
        internal static Money? ExpectedSalaryMinMoney(this Worker worker) =>
            ToMoney(worker.ExpectedSalaryMinAmount, worker.ExpectedSalaryMinCurrency);

        private static Money? ToMoney(decimal? amount, string? currency)
        {
            if (!amount.HasValue || currency.IsNullOrWhiteSpace())
            {
                return null;
            }

            return new Money(amount.Value, currency.Trim().ToUpperInvariant());
        }

        #endregion Methods
    }
}
