namespace Azoxia.AdaIsAkademi.SeedRunner.Stages;

using Azoxia.AdaIsAkademi.Domain;
using Azoxia.AdaIsAkademi.Persistence;
using Azoxia.AdaIsAkademi.SeedRunner.Generators;
using Azoxia.Core.ValueTypes;
using Bogus;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Creates postings, applications, assignments for completed flows, then moves closed posting dates to the past (SQL update).
/// </summary>
internal static class JobPostingApplicationStage
{
    #region Utils

    internal static async Task RunAsync(
        AdaIsAkademiDbContext db,
        SeederState state,
        SeedOptions options,
        Random rnd,
        Faker faker,
        CancellationToken cancellationToken)
    {
        if (state.Employers.Count == 0 || state.Workers.Count == 0)
        {
            throw new InvalidOperationException("Employers ve worker kayıtları gerekli.");
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        List<PostingPlan> plans = [];

        int employerRot = 0;
        foreach (int _ in Enumerable.Range(0, options.OpenPostings))
        {
            SeederState.EmployerSeed bundle = state.Employers[employerRot % state.Employers.Count];
            employerRot++;

            JobCategoryCatalog.CategoryRow leaf = PickLeafRow(rnd);
            DateOnly shift = today.AddDays(rnd.Next(1, 31));
            plans.Add(await CreatePostingAsync(
                db,
                state,
                bundle,
                leaf,
                shift,
                rnd,
                faker,
                PostingKind.Open,
                headCount: rnd.Next(1, 4),
                cancellationToken));
        }

        int closedTotal = options.ClosedPostings;
        int completedCount = closedTotal / 3;
        int filledCount = closedTotal / 3;
        int cancelledCount = closedTotal - completedCount - filledCount;

        DateOnly futureAnchor = today.AddDays(14);

        foreach (int _ in Enumerable.Range(0, completedCount))
        {
            SeederState.EmployerSeed bundle = state.Employers[employerRot % state.Employers.Count];
            employerRot++;
            JobCategoryCatalog.CategoryRow leaf = PickLeafRow(rnd);
            plans.Add(await CreatePostingAsync(
                db,
                state,
                bundle,
                leaf,
                futureAnchor.AddDays(rnd.Next(1, 10)),
                rnd,
                faker,
                PostingKind.Completed,
                headCount: 1,
                cancellationToken));
        }

        foreach (int _ in Enumerable.Range(0, filledCount))
        {
            SeederState.EmployerSeed bundle = state.Employers[employerRot % state.Employers.Count];
            employerRot++;
            JobCategoryCatalog.CategoryRow leaf = PickLeafRow(rnd);
            plans.Add(await CreatePostingAsync(
                db,
                state,
                bundle,
                leaf,
                futureAnchor.AddDays(rnd.Next(1, 10)),
                rnd,
                faker,
                PostingKind.Filled,
                headCount: rnd.Next(2, 4),
                cancellationToken));
        }

        foreach (int _ in Enumerable.Range(0, cancelledCount))
        {
            SeederState.EmployerSeed bundle = state.Employers[employerRot % state.Employers.Count];
            employerRot++;
            JobCategoryCatalog.CategoryRow leaf = PickLeafRow(rnd);
            plans.Add(await CreatePostingAsync(
                db,
                state,
                bundle,
                leaf,
                futureAnchor.AddDays(rnd.Next(1, 10)),
                rnd,
                faker,
                PostingKind.Cancelled,
                headCount: rnd.Next(1, 3),
                cancellationToken));
        }

        foreach (PostingPlan plan in plans)
        {
            await SeedApplicationsAsync(db, state, plan, rnd, faker, cancellationToken);
        }

        foreach (PostingPlan plan in plans.Where(p =>
                     p.Kind is PostingKind.Completed or PostingKind.Filled or PostingKind.Cancelled))
        {
            DateOnly past = today.AddDays(-rnd.Next(1, 90));
            await db.Database.ExecuteSqlInterpolatedAsync(
                $"""
                UPDATE "JobPosting" SET "ShiftDate" = {past} WHERE "Id" = {plan.Posting.Id};
                """,
                cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task ApplyAssignmentsAndCompleteAsync(
        AdaIsAkademiDbContext db,
        SeederState state,
        JobPosting posting,
        List<JobApplication> accepted,
        CancellationToken cancellationToken)
    {
        foreach (JobApplication app in accepted)
        {
            var assignment = new ShiftAssignment(
                posting.Id,
                app.Id,
                app.WorkerId,
                "seed-worker-qr-hash",
                "seed-supervisor-qr-hash");
            db.Set<ShiftAssignment>().Add(assignment);
            await db.SaveChangesAsync(cancellationToken);

            assignment.CheckIn("seed-worker-qr-hash");
            assignment.SupervisorCheckIn("seed-supervisor-qr-hash");
            assignment.CheckOut();
            await db.SaveChangesAsync(cancellationToken);

            state.PayoutSources.Add(new SeederState.PayoutSource(assignment.Id, posting.EmployerId, app.WorkerId, posting.Id));
        }

        posting.Complete();
        await db.SaveChangesAsync(cancellationToken);
    }

    private static Money BuildWage(Random rnd, string cluster)
    {
        decimal amount = cluster switch
        {
            JobCategoryCatalog.ClusterFood => rnd.Next(850, 1400),
            JobCategoryCatalog.ClusterLogistics => rnd.Next(700, 1300),
            JobCategoryCatalog.ClusterRetail => rnd.Next(750, 1250),
            JobCategoryCatalog.ClusterEvent => rnd.Next(800, 1500),
            JobCategoryCatalog.ClusterCleaning => rnd.Next(650, 1100),
            JobCategoryCatalog.ClusterOffice => rnd.Next(900, 1600),
            _ => rnd.Next(800, 1300),
        };

        return new Money(amount, "TRY");
    }

    private static string BuildDescription(Faker faker, string cluster)
        => $"{faker.Lorem.Paragraphs(2)} Öncelik: {cluster} operasyonları. Hafta içi yoğun tempo; ekip içi koordinasyon beklenir.";

    private static async Task<PostingPlan> CreatePostingAsync(
        AdaIsAkademiDbContext db,
        SeederState state,
        SeederState.EmployerSeed bundle,
        JobCategoryCatalog.CategoryRow leaf,
        DateOnly shiftDate,
        Random rnd,
        Faker faker,
        PostingKind kind,
        int headCount,
        CancellationToken cancellationToken)
    {
        Employer employer = bundle.Employer;
        EmployerLocation loc = bundle.Locations[rnd.Next(bundle.Locations.Count)];
        int catId = state.CategoryIdByKey[leaf.Key];
        Money wage = BuildWage(rnd, leaf.Cluster);
        (TimeOnly start, TimeOnly end) = PickShiftWindow(rnd);

        string title = $"{leaf.Name.Replace("[Seed] ", string.Empty, StringComparison.Ordinal)} — {faker.Commerce.ProductAdjective()} vardiya";
        string description = BuildDescription(faker, leaf.Cluster);

        JobPosting posting = employer.AddJobPosting(
            loc.Id,
            catId,
            title,
            description,
            shiftDate,
            start,
            end,
            wage,
            headCount);

        IReadOnlyList<string> skillPool = SkillCatalog.GetTagsForCluster(leaf.Cluster);
        int skillTotal = rnd.Next(3, 6);
        HashSet<string> picked = new(StringComparer.OrdinalIgnoreCase);
        while (picked.Count < skillTotal)
        {
            picked.Add(skillPool[rnd.Next(skillPool.Count)]);
        }

        List<string> required = picked.OrderBy(_ => rnd.Next()).Take(rnd.Next(1, 3)).ToList();
        foreach (string tag in picked)
        {
            posting.AddSkill(tag, required.Contains(tag));
        }

        posting.UpdateEmbedding(EmbeddingFaker.GenerateDeterministic(description));
        posting.Publish();
        await db.SaveChangesAsync(cancellationToken);
        state.Postings.Add(posting);

        return new PostingPlan(posting, kind, leaf.Cluster);
    }

    private static async Task<List<JobApplication>> LoadApplicationsAsync(
        AdaIsAkademiDbContext db,
        int postingId,
        CancellationToken cancellationToken)
        => await db.Set<JobApplication>()
            .Where(a => a.JobPostingId == postingId)
            .OrderBy(a => a.Id)
            .ToListAsync(cancellationToken);

    private static JobCategoryCatalog.CategoryRow PickLeafRow(Random rnd)
    {
        List<JobCategoryCatalog.CategoryRow> leaves = JobCategoryCatalog.Rows
            .Where(r => r.ParentKey is not null)
            .ToList();

        return leaves[rnd.Next(leaves.Count)];
    }

    private static (TimeOnly Start, TimeOnly End) PickShiftWindow(Random rnd)
    {
        if (rnd.NextDouble() < 0.5)
        {
            return (new TimeOnly(9, 0), new TimeOnly(18, 0));
        }

        return (new TimeOnly(14, 0), new TimeOnly(22, 0));
    }

    private static List<SeederState.WorkerSeed> RankWorkers(
        List<SeederState.WorkerSeed> workers,
        string cluster,
        JobPosting posting)
    {
        HashSet<string> postingTags = posting.Skills.Select(s => s.Tag.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

        return workers
            .OrderByDescending(w =>
            {
                int clusterBoost = string.Equals(w.Cluster, cluster, StringComparison.OrdinalIgnoreCase) ? 3 : 0;
                int overlap = w.SkillTags.Count(t => postingTags.Contains(t.ToUpperInvariant()));
                return overlap * 10 + clusterBoost;
            })
            .ThenBy(w => w.Worker.Id)
            .ToList();
    }

    private static async Task SeedApplicationsAsync(
        AdaIsAkademiDbContext db,
        SeederState state,
        PostingPlan plan,
        Random rnd,
        Faker faker,
        CancellationToken cancellationToken)
    {
        JobPosting posting = await db.Set<JobPosting>()
            .Include(p => p.Skills)
            .FirstAsync(p => p.Id == plan.Posting.Id, cancellationToken);

        int targetCount = rnd.Next(8, 26);
        if (plan.Kind == PostingKind.Filled)
        {
            targetCount = Math.Max(targetCount, posting.HeadCount + rnd.Next(3, 12));
        }

        List<SeederState.WorkerSeed> ranked = RankWorkers(state.Workers, plan.Cluster, posting);
        List<SeederState.WorkerSeed> pool = [];

        int strong = (int)Math.Round(targetCount * 0.85);
        for (int i = 0; i < strong && i < ranked.Count; i++)
        {
            pool.Add(ranked[i]);
        }

        while (pool.Count < targetCount)
        {
            SeederState.WorkerSeed w = state.Workers[rnd.Next(state.Workers.Count)];
            if (pool.All(x => x.Worker.Id != w.Worker.Id))
            {
                pool.Add(w);
            }
        }

        if (plan.Kind == PostingKind.Completed || plan.Kind == PostingKind.Filled)
        {
            pool = ranked.Take(Math.Min(ranked.Count, targetCount)).ToList();
        }
        else
        {
            pool = pool.OrderBy(_ => rnd.Next()).Take(targetCount).ToList();
        }

        foreach (SeederState.WorkerSeed w in pool)
        {
            posting.AddApplication(w.Worker.Id, hasConflictingShift: false, note: faker.Lorem.Sentence(8));
        }

        await db.SaveChangesAsync(cancellationToken);

        List<JobApplication> apps = await LoadApplicationsAsync(db, posting.Id, cancellationToken);

        switch (plan.Kind)
        {
            case PostingKind.Open:
                TransitionOpenPosting(posting, apps, rnd);
                await db.SaveChangesAsync(cancellationToken);
                break;

            case PostingKind.Completed:
                await RunCompletedAsync(db, state, posting, apps, rnd, cancellationToken);
                break;

            case PostingKind.Filled:
                RunFilled(posting, apps);
                await db.SaveChangesAsync(cancellationToken);
                break;

            case PostingKind.Cancelled:
                foreach (JobApplication a in apps.Take(Math.Min(apps.Count, rnd.Next(2, 8))))
                {
                    if (a.Status == JobApplicationStatus.Pending)
                    {
                        posting.WithdrawApplication(a.Id);
                    }
                }

                posting.Cancel();
                await db.SaveChangesAsync(cancellationToken);
                break;
        }
    }

    private static async Task RunCompletedAsync(
        AdaIsAkademiDbContext db,
        SeederState state,
        JobPosting posting,
        List<JobApplication> apps,
        Random rnd,
        CancellationToken cancellationToken)
    {
        List<JobApplication> ordered = apps.OrderBy(a => a.Id).ToList();
        List<JobApplication> accepted = [];
        int cap = Math.Min(posting.HeadCount, ordered.Count);
        for (int i = 0; i < cap; i++)
        {
            posting.AcceptApplication(ordered[i].Id);
            accepted.Add(ordered[i]);
        }

        for (int i = cap; i < ordered.Count; i++)
        {
            JobApplication a = ordered[i];
            double r = rnd.NextDouble();
            if (r < 0.35)
            {
                posting.RejectApplication(a.Id, "Deneyim profili uyumsuz.");
            }
            else if (r < 0.55)
            {
                posting.WithdrawApplication(a.Id);
            }
            else if (r < 0.75 && a.Status == JobApplicationStatus.Pending)
            {
                a.Expire();
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        await ApplyAssignmentsAndCompleteAsync(db, state, posting, accepted, cancellationToken);
    }

    private static void RunFilled(JobPosting posting, List<JobApplication> apps)
    {
        List<JobApplication> ordered = apps.OrderBy(a => a.Id).ToList();
        int need = posting.HeadCount;
        for (int i = 0; i < need && i < ordered.Count; i++)
        {
            posting.AcceptApplication(ordered[i].Id);
        }

        for (int i = need; i < ordered.Count; i++)
        {
            posting.RejectApplication(ordered[i].Id, "Kontenjan doldu.");
        }
    }

    private static void TransitionOpenPosting(JobPosting posting, List<JobApplication> apps, Random rnd)
    {
        if (apps.Count == 0)
        {
            return;
        }

        List<JobApplication> shuffled = apps.OrderBy(_ => rnd.Next()).ToList();
        int n = shuffled.Count;
        int acceptCount = Math.Min(posting.HeadCount, Math.Max(1, (int)Math.Ceiling(n * 0.05)));
        int withdrawCount = (int)Math.Round(n * 0.15);
        int rejectCount = (int)Math.Round(n * 0.10);

        int idx = 0;
        for (; idx < acceptCount && idx < shuffled.Count; idx++)
        {
            posting.AcceptApplication(shuffled[idx].Id);
        }

        for (int j = 0; j < withdrawCount && idx < shuffled.Count; j++, idx++)
        {
            posting.WithdrawApplication(shuffled[idx].Id);
        }

        for (int j = 0; j < rejectCount && idx < shuffled.Count; j++, idx++)
        {
            posting.RejectApplication(shuffled[idx].Id, "İlan önceliği değişti.");
        }
    }

    #endregion Utils

    #region Nested types

    private enum PostingKind
    {
        Open,

        Completed,

        Filled,

        Cancelled,
    }

    private sealed record PostingPlan(JobPosting Posting, PostingKind Kind, string Cluster);

    #endregion Nested types
}
