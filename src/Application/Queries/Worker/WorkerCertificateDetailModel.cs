namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;
    using System;

    /// <summary>
    /// Worker certificate row.
    /// </summary>
    public sealed record WorkerCertificateDetailModel(
        int Id,
        string Name,
        string IssuingOrganization,
        DateOnly IssuedAt,
        DateOnly? ExpiresAt,
        string? DocumentUrl,
        DateTimeOffset CreatedAt) :
        ModelBase;
}
