namespace Azoxia.AdaIsAkademi.SeedRunner.Stages;

using Azoxia.AdaIsAkademi.Domain;
using Azoxia.AdaIsAkademi.Persistence;
using Azoxia.Core.ValueTypes;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Simulates worker payouts, commission audit rows, and monthly receivables.
/// </summary>
internal static class MonetizationStage
{
    #region Utils

    internal static async Task RunAsync(
        AdaIsAkademiDbContext db,
        SeederState state,
        SeedOptions _,
        Random rnd,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"[MonetizationStage] Başlıyor. payoutSource={state.PayoutSources.Count}, employers={state.Employers.Count}");

        if (state.PayoutSources.Count == 0)
        {
            await SeedReceivablesOnlyAsync(db, state, rnd, cancellationToken);
            Console.WriteLine("[MonetizationStage] Payout source yoktu, yalnızca receivable üretildi.");
            return;
        }

        foreach (SeederState.PayoutSource src in state.PayoutSources)
        {
            JobPosting? posting = await db.Set<JobPosting>()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == src.JobPostingId, cancellationToken);
            Employer? employer = await db.Set<Employer>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == src.EmployerId, cancellationToken);
            if (posting is null || employer is null)
            {
                continue;
            }

            Money gross = posting.Wage;
            Money commission = new(gross.Amount * employer.CommissionRate, gross.Currency);

            var payout = new WorkerPayout(src.AssignmentId, src.EmployerId, src.WorkerId, gross, commission);
            db.Set<WorkerPayout>().Add(payout);
            await db.SaveChangesAsync(cancellationToken);

            db.Set<CommissionAuditLog>().Add(new CommissionAuditLog(
                src.EmployerId,
                CommissionAuditEventType.WorkerPayoutCreated,
                commission,
                assignmentId: src.AssignmentId,
                workerPayoutId: payout.Id,
                note: "seed_created_from_assignment"));
            await db.SaveChangesAsync(cancellationToken);

            int roll = rnd.Next(100);

            if (roll < 70)
            {
                payout.MarkAsProcessing(assignmentIsDisputed: false);
                db.Set<CommissionAuditLog>().Add(new CommissionAuditLog(
                    src.EmployerId,
                    CommissionAuditEventType.WorkerPayoutMarkedAsPaid,
                    commission,
                    assignmentId: src.AssignmentId,
                    workerPayoutId: payout.Id));
                await db.SaveChangesAsync(cancellationToken);

                payout.ConfirmPaid();
                db.Set<CommissionAuditLog>().Add(new CommissionAuditLog(
                    src.EmployerId,
                    CommissionAuditEventType.WorkerPayoutConfirmed,
                    commission,
                    assignmentId: src.AssignmentId,
                    workerPayoutId: payout.Id));
                await db.SaveChangesAsync(cancellationToken);
            }
            else if (roll < 85)
            {
                payout.MarkAsProcessing(assignmentIsDisputed: false);
                db.Set<CommissionAuditLog>().Add(new CommissionAuditLog(
                    src.EmployerId,
                    CommissionAuditEventType.WorkerPayoutMarkedAsPaid,
                    commission,
                    assignmentId: src.AssignmentId,
                    workerPayoutId: payout.Id));
                await db.SaveChangesAsync(cancellationToken);
            }
            else if (roll < 95)
            {
                // Pending: only creation audit rows already persisted.
            }
            else
            {
                payout.MarkAsProcessing(assignmentIsDisputed: false);
                db.Set<CommissionAuditLog>().Add(new CommissionAuditLog(
                    src.EmployerId,
                    CommissionAuditEventType.WorkerPayoutMarkedAsPaid,
                    commission,
                    assignmentId: src.AssignmentId,
                    workerPayoutId: payout.Id));
                await db.SaveChangesAsync(cancellationToken);

                payout.Fail("Banka transferi reddedildi (seed)");
                db.Set<CommissionAuditLog>().Add(new CommissionAuditLog(
                    src.EmployerId,
                    CommissionAuditEventType.WorkerPayoutFailed,
                    commission,
                    assignmentId: src.AssignmentId,
                    workerPayoutId: payout.Id,
                    note: "Banka transferi reddedildi (seed)"));
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        await SeedReceivablesAsync(db, state, rnd, cancellationToken);
        Console.WriteLine("[MonetizationStage] Tamamlandı. payout ve receivable üretimi bitti.");
    }

    private static async Task SeedReceivablesAsync(
        AdaIsAkademiDbContext db,
        SeederState state,
        Random rnd,
        CancellationToken cancellationToken)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        for (int m = 0; m < 3; m++)
        {
            DateOnly month = today.AddMonths(-m);
            DateOnly start = new(month.Year, month.Month, 1);
            DateOnly nextMonthFirst = start.AddMonths(1);
            DateTimeOffset monthStart = new(start.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            DateTimeOffset monthEndExclusive = new(nextMonthFirst.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);

            foreach (SeederState.EmployerSeed bundle in state.Employers)
            {
                decimal sum = await db.Set<WorkerPayout>()
                    .Where(p =>
                        p.EmployerId == bundle.Employer.Id
                        && p.Status == WorkerPayoutStatus.Paid
                        && p.PaidAt >= monthStart
                        && p.PaidAt < monthEndExclusive)
                    .SumAsync(p => p.CommissionAmount.Amount, cancellationToken);

                if (sum <= 0m)
                {
                    sum = Math.Round((decimal)rnd.NextDouble() * 5000m + 500m, 2);
                }

                Money amount = new(sum, "TRY");
                DateOnly periodEnd = new(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
                var row = new CommissionReceivable(amount, bundle.Employer.Id, periodEnd, start);
                db.Set<CommissionReceivable>().Add(row);
                await db.SaveChangesAsync(cancellationToken);

                db.Set<CommissionAuditLog>().Add(new CommissionAuditLog(
                    bundle.Employer.Id,
                    CommissionAuditEventType.CommissionReceivableGenerated,
                    amount,
                    commissionReceivableId: row.Id,
                    note: "seed_period_close"));
                await db.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private static async Task SeedReceivablesOnlyAsync(
        AdaIsAkademiDbContext db,
        SeederState state,
        Random rnd,
        CancellationToken cancellationToken)
    {
        await SeedReceivablesAsync(db, state, rnd, cancellationToken);
    }

    #endregion Utils
}
