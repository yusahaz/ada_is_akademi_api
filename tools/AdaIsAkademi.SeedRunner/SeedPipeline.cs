namespace Azoxia.AdaIsAkademi.SeedRunner;

using Azoxia.AdaIsAkademi.Domain;
using Azoxia.AdaIsAkademi.Persistence;
using Azoxia.AdaIsAkademi.SeedRunner.Stages;
using Bogus;
using Microsoft.EntityFrameworkCore;

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

        if (!options.Reset
            && await db.Set<SystemUser>().AnyAsync(u => u.Email == WorkerMarkerEmail, cancellationToken))
        {
            Console.WriteLine($"Seed verisi zaten mevcut ({WorkerMarkerEmail}). Yeniden oluşturmak için --reset kullanın.");
            return;
        }

        if (options.Reset)
        {
            await ResetStage.ExecuteAsync(db, cancellationToken);
        }

        var state = new SeederState();
        await LookupStage.RunAsync(db, state, cancellationToken);
        await WorkforceStage.RunAsync(db, state, options, rnd, faker, cancellationToken);
        await JobPostingApplicationStage.RunAsync(db, state, options, rnd, faker, cancellationToken);
        await MonetizationStage.RunAsync(db, state, options, rnd, cancellationToken);

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

    #endregion Utils
}
