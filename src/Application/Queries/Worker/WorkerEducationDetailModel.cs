namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Worker education row.
    /// </summary>
    public sealed record WorkerEducationDetailModel(
        int Id,
        string School,
        string Department,
        EducationType EducationType,
        int StartYear,
        int? EndYear,
        bool IsOngoing) :
        ModelBase;
}
