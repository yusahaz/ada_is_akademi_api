namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Linked system user summary for employer supervisor rows.
    /// </summary>
    public sealed record EmployerSupervisorUserSummaryModel(
        int Id,
        string Email,
        string? FirstName,
        string? LastName,
        string? Phone,
        AccountStatus AccountStatus) :
        ModelBase;
}
