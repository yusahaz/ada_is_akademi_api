namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// CSV export package for overdue alarms.
    /// </summary>
    public sealed record OverdueAlarmExportPackageModel(
        string ContentType,
        string CsvContent,
        string FileName,
        int RowCount) :
        ModelBase;
}
