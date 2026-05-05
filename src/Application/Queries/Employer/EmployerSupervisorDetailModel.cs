namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Employer supervisor detail row.
    /// </summary>
    public sealed record EmployerSupervisorDetailModel(
        int Id,
        int SystemUserId,
        int? LocationId,
        bool IsActive,
        EmployerSupervisorUserSummaryModel SystemUser) :
        ModelBase;
}
