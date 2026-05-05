namespace Azoxia.AdaIsAkademi.SeedRunner;

using Azoxia.AdaIsAkademi.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

/// <summary>
/// CLI entry that loads configuration and runs <see cref="SeedPipeline"/>.
/// </summary>
internal static class Program
{
    #region Utils

    private static async Task<int> Main(string[] args)
    {
        try
        {
            SeedOptions options = SeedOptions.Parse(args);

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Seed.json", optional: true)
                .Build();

            string? connectionString = options.ConnectionString
                ?? configuration.GetConnectionString("AdaIs")
                ?? Environment.GetEnvironmentVariable("DOTNET_ConnectionStrings__AdaIs")
                ?? Environment.GetEnvironmentVariable("ConnectionStrings__AdaIs");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.Error.WriteLine("Bağlantı dizesi bulunamadı: --connection-string, ConnectionStrings__AdaIs veya appsettings.Seed.json kullanın.");
                return 1;
            }

            var optionsBuilder = new DbContextOptionsBuilder<AdaIsAkademiDbContext>();
            optionsBuilder.UseNpgsql(connectionString).UseLazyLoadingProxies();

            await using var db = new AdaIsAkademiDbContext(optionsBuilder.Options);
            await SeedPipeline.RunAsync(db, options, CancellationToken.None);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 2;
        }
    }

    #endregion Utils
}
