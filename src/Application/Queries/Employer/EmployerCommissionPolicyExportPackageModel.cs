namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// CSV export package for employer commission policies.
    /// </summary>
    public sealed record EmployerCommissionPolicyExportPackageModel(
        string ContentType,
        string CsvContent,
        string FileName,
        int RowCount) :
        ModelBase;
}
