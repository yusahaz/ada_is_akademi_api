namespace Azoxia.AdaIsAkademi.SeedRunner.Stages;

using Azoxia.AdaIsAkademi.Persistence;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Truncates operational tables and removes non-migration users (keeps default migration admin).
/// </summary>
internal static class ResetStage
{
    #region Fields

    private const string MigrationAdminEmail = "admin@adaisakademi.local";

    #endregion Fields

    #region Utils

    /// <summary>
    /// Executes destructive reset SQL in a single transaction.
    /// </summary>
    internal static async Task ExecuteAsync(AdaIsAkademiDbContext db, CancellationToken cancellationToken)
    {
        Console.WriteLine("[ResetStage] Yıkıcı reset işlemi başlıyor (migration admin korunur).");
        await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction trx =
            await db.Database.BeginTransactionAsync(cancellationToken);

        // Operational facts first; exclude permission catalog tables.
        await db.Database.ExecuteSqlRawAsync(
            """
            TRUNCATE TABLE
              "SystemUserNotificationDispatch",
              "CommissionAuditLog",
              "WorkerPayout",
              "CommissionReceivable",
              "ShiftAssignment",
              "JobApplication",
              "JobPostingSkill",
              "OverdueJobAlarm",
              "JobPosting",
              "Supervisor",
              "EmployerLocation",
              "WorkerInterestedJobCategory",
              "WorkerSkill",
              "WorkerAvailability",
              "WorkerCertificate",
              "WorkerEducation",
              "WorkerExperience",
              "WorkerLanguage",
              "WorkerReference",
              "WorkerSocialLink",
              "Worker",
              "EmployerSocialLink",
              "Employer",
              "JobCategory",
              "SystemUserRefreshToken",
              "SystemUserDevice",
              "AppLogs"
            RESTART IDENTITY CASCADE;
            """,
            cancellationToken);

        await db.Database.ExecuteSqlRawAsync(
            $"""
            DELETE FROM "SystemUser" WHERE "Email" <> '{MigrationAdminEmail}';
            """,
            cancellationToken);

        await trx.CommitAsync(cancellationToken);
        Console.WriteLine("[ResetStage] Reset işlemi commit edildi.");
    }

    #endregion Utils
}
