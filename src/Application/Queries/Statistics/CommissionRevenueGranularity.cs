namespace Azoxia.AdaIsAkademi.Application
{
    /// <summary>
    /// Time bucket width for commission receivable revenue reporting.
    /// </summary>
    public enum CommissionRevenueGranularity
    {
        /// <summary>One column per calendar month.</summary>
        Monthly = 0,

        /// <summary>One column per calendar quarter (three months).</summary>
        Quarterly = 1,

        /// <summary>One column per half calendar year.</summary>
        HalfYearly = 2,

        /// <summary>One column per calendar year.</summary>
        Yearly = 3,
    }
}
