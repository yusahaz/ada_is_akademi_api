namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;
    using System;

    /// <summary>
    /// Summary of overdue job postings and pending applications.
    /// </summary>
    public sealed record OverdueJobSummaryModel(
        int OverduePendingApplicationCount,
        int OverduePostingCount,
        DateOnly SnapshotDate) :
        ModelBase;
}
