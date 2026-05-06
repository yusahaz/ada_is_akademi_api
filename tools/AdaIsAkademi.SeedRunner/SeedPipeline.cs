namespace Azoxia.AdaIsAkademi.SeedRunner;

using Azoxia.AdaIsAkademi.Domain;
using Azoxia.AdaIsAkademi.Persistence;
using Azoxia.AdaIsAkademi.SeedRunner.Stages;
using Bogus;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

/// <summary>
/// Orchestrates deterministic demo seed stages against one <see cref="AdaIsAkademiDbContext"/> scope.
/// </summary>
internal static class SeedPipeline
{
    #region Fields

    private const string WorkerMarkerEmail = "worker001@adaisakademi.seed.local";

    #endregion Fields

    #region Utils

    /// <summary>
    /// Runs reset (optional), lookup data, workforce, postings, and monetization stages.
    /// </summary>
    internal static async Task RunAsync(
        AdaIsAkademiDbContext db,
        SeedOptions options,
        CancellationToken cancellationToken)
    {
        if (IsProductionBlocked(options))
        {
            throw new InvalidOperationException(
                "ASPNETCORE_ENVIRONMENT=Production ile çalıştırma engellendi. Test için --allow-production ekleyin.");
        }

        var rnd = new Random(options.Seed);
        Randomizer.Seed = new Random(options.Seed);
        var faker = new Faker("tr");
        Console.WriteLine("[SeedPipeline] Deterministik seed başlatıldı.");

        if (!options.Reset
            && await db.Set<SystemUser>().AnyAsync(u => u.Email == WorkerMarkerEmail, cancellationToken))
        {
            Console.WriteLine($"Seed verisi zaten mevcut ({WorkerMarkerEmail}). Yeniden oluşturmak için --reset kullanın.");
            return;
        }

        if (options.Reset)
        {
            Console.WriteLine("[SeedPipeline] Reset aşaması başlıyor...");
            await ResetStage.ExecuteAsync(db, cancellationToken);
            Console.WriteLine("[SeedPipeline] Reset aşaması tamamlandı.");
        }

        var state = new SeederState();
        await RunStageAsync("Lookup", () => LookupStage.RunAsync(db, state, cancellationToken));
        await RunStageAsync("Workforce", () => WorkforceStage.RunAsync(db, state, options, rnd, faker, cancellationToken));
        await RunStageAsync("JobPostingApplication", () => JobPostingApplicationStage.RunAsync(db, state, options, rnd, faker, cancellationToken));
        await RunStageAsync("Monetization", () => MonetizationStage.RunAsync(db, state, options, rnd, cancellationToken));

        Console.WriteLine(
            $"[SeedPipeline] Özet: workers={state.Workers.Count}, employers={state.Employers.Count}, postings={state.Postings.Count}, payoutSources={state.PayoutSources.Count}");

        Console.WriteLine("Ada İş Akademi demo seed tamamlandı.");
        Console.WriteLine($"  Worker giriş örneği: worker001@adaisakademi.seed.local / {SeedConstants.DefaultPassword}");
        Console.WriteLine($"  İşveren giriş örneği: employer01@adaisakademi.seed.local / {SeedConstants.DefaultPassword}");
        Console.WriteLine($"  Demo admin: admin@adaisakademi.test / {SeedConstants.DefaultPassword}");
    }

    private static bool IsProductionBlocked(SeedOptions options)
    {
        if (options.AllowProduction)
        {
            return false;
        }

        string? env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return string.Equals(env, "Production", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task RunStageAsync(string stageName, Func<Task> run)
    {
        var stopwatch = Stopwatch.StartNew();
        Console.WriteLine($"[SeedPipeline] {stageName} başladı.");
        await run();
        Console.WriteLine($"[SeedPipeline] {stageName} tamamlandı ({stopwatch.Elapsed.TotalSeconds:F1}s).");
    }

    #endregion Utils
}
