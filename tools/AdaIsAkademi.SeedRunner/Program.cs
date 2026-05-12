namespace Azoxia.AdaIsAkademi.SeedRunner;

using Azoxia.AdaIsAkademi.Domain.Events;
using Azoxia.AdaIsAkademi.Persistence;
using Azoxia.Core.Identity;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// CLI entry that loads configuration and runs <see cref="SeedPipeline"/>.
/// </summary>
internal static class Program
{
    #region Utils

    private static async Task<int> Main(string[] args)
    {
        var timer = Stopwatch.StartNew();

        try
        {
            Console.WriteLine($"[SeedRunner] Başlatılıyor. UTC={DateTime.UtcNow:O}");
            Console.WriteLine($"[SeedRunner] Argümanlar: {(args.Length == 0 ? "<yok>" : string.Join(' ', args))}");

            SeedOptions options = SeedOptions.Parse(args);
            Console.WriteLine(
                $"[SeedRunner] Seçenekler: reset={options.Reset}, workers={options.Workers}, employers={options.Employers}, openPostings={options.OpenPostings}, closedPostings={options.ClosedPostings}, seed={options.Seed}, allowProduction={options.AllowProduction}, skipMedia={options.SkipMediaUpload}");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Seed.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            string? connectionString = options.ConnectionString
                ?? Environment.GetEnvironmentVariable("DOTNET_ConnectionStrings__AdaIs")
                ?? Environment.GetEnvironmentVariable("ConnectionStrings__AdaIs")
                ?? configuration.GetConnectionString("AdaIs");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.Error.WriteLine("Bağlantı dizesi bulunamadı: --connection-string, ConnectionStrings__AdaIs veya appsettings.Seed.json kullanın.");
                return 1;
            }

            Console.WriteLine($"[SeedRunner] Connection string kaynağı: {ResolveConnectionSource(options, configuration)}");
            Console.WriteLine($"[SeedRunner] Connection string özeti: {DescribeConnectionString(connectionString)}");

            ServiceCollection services = new();
            services.AddLogging();
            services.AddSingleton<IExecutionContext, SeedExecutionContext>();
            services.AddScoped<IDomainEventDispatcher, SeedNoOpDomainEventDispatcher>();
            services.AddDbContext<AdaIsAkademiDbContext>(optionsBuilder =>
                optionsBuilder.UseNpgsql(connectionString).UseLazyLoadingProxies());

            await using ServiceProvider serviceProvider = services.BuildServiceProvider();
            await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
            AdaIsAkademiDbContext db = scope.ServiceProvider.GetRequiredService<AdaIsAkademiDbContext>();
            ObjectStorageMediaUploader? mediaUploader = ObjectStorageMediaUploader.TryCreate(configuration);
            if (mediaUploader is null)
            {
                Console.WriteLine("[SeedRunner] ObjectStorage yapılandırması yok; DB'de object key yazılır, dosya yüklemesi atlanır.");
            }
            else if (!mediaUploader.CanUploadForScale(options))
            {
                Console.WriteLine(
                    $"[SeedRunner] Worker/işveren sayısı çok yüksek; MinIO ikili yükleme atlandı (sınır: workers≤2500, employers≤500). --skip-media-upload veya daha küçük bir seed kullanın.");
            }

            try
            {
                await SeedPipeline.RunAsync(db, options, mediaUploader, CancellationToken.None);
            }
            finally
            {
                if (mediaUploader is not null)
                {
                    await mediaUploader.DisposeAsync();
                }
            }
            Console.WriteLine($"[SeedRunner] Tamamlandı. Süre={timer.Elapsed.TotalSeconds:F1}s");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("[SeedRunner] Hata:");
            Console.Error.WriteLine(ex.ToString());
            Console.Error.WriteLine($"[SeedRunner] Başarısız sonlandı. Süre={timer.Elapsed.TotalSeconds:F1}s");
            return 2;
        }
    }

    private static string ResolveConnectionSource(SeedOptions options, IConfigurationRoot configuration)
    {
        if (!string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return "--connection-string";
        }

        string? fromConfig = configuration.GetConnectionString("AdaIs");
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOTNET_ConnectionStrings__AdaIs")))
        {
            return "DOTNET_ConnectionStrings__AdaIs";
        }

        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ConnectionStrings__AdaIs")))
        {
            return "ConnectionStrings__AdaIs";
        }

        if (!string.IsNullOrWhiteSpace(fromConfig))
        {
            return "appsettings.Seed.json";
        }

        return "bilinmiyor";
    }

    private static string DescribeConnectionString(string connectionString)
    {
        string[] parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        string host = ExtractValue(parts, "Host") ?? "<yok>";
        string port = ExtractValue(parts, "Port") ?? "<yok>";
        string db = ExtractValue(parts, "Database") ?? "<yok>";
        string user = ExtractValue(parts, "Username") ?? "<yok>";
        return $"Host={host};Port={port};Database={db};Username={user};Password=<redacted>";
    }

    private static string? ExtractValue(string[] parts, string key)
        => parts
            .Select(p => p.Split('=', 2, StringSplitOptions.TrimEntries))
            .Where(a => a.Length == 2 && string.Equals(a[0], key, StringComparison.OrdinalIgnoreCase))
            .Select(a => a[1])
            .FirstOrDefault();

    #endregion Utils
}
