namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Worker reference row and contact summary.
    /// </summary>
    public sealed record WorkerReferenceDetailModel(
        int Id,
        string Company,
        string Position,
        string? ContactFirstName,
        string? ContactLastName,
        string? ContactEmail,
        string? ContactPhone) :
        ModelBase;
}
