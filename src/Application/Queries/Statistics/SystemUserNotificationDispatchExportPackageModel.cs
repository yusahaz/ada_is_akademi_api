namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// CSV export package for system-user notification dispatches.
    /// </summary>
    public sealed record SystemUserNotificationDispatchExportPackageModel(
        string ContentType,
        string CsvContent,
        string FileName,
        int RowCount) :
        ModelBase;
}
