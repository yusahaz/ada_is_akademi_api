namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Minimal linked system user snapshot under worker detail.
    /// </summary>
    public sealed record WorkerSystemUserSummaryModel(
        int Id,
        string Email,
        string? FirstName,
        string? LastName,
        string? Phone,
        AccountStatus AccountStatus) :
        ModelBase;
}
