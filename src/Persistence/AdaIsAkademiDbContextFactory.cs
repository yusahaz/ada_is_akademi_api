namespace Azoxia.AdaIsAkademi.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Design-time factory so <c>dotnet ef</c> can build the model without starting the web host.
    /// In Docker migrator jobs, reads <c>ConnectionStrings__AdaIs</c> or <c>DbConfig__*</c> from the environment.
    /// </summary>
    internal sealed class AdaIsAkademiDbContextFactory : IDesignTimeDbContextFactory<AdaIsAkademiDbContext>
    {
        /// <inheritdoc />
        public AdaIsAkademiDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<AdaIsAkademiDbContext> optionsBuilder = new();
            optionsBuilder.UseNpgsql(ResolveDesignTimeConnectionString());
            return new AdaIsAkademiDbContext(optionsBuilder.Options, new DesignTimeServiceScopeFactory());
        }

        private static string ResolveDesignTimeConnectionString()
        {
            string? fromEnv =
                Environment.GetEnvironmentVariable("ConnectionStrings__AdaIs")
                ?? Environment.GetEnvironmentVariable("DOTNET_ConnectionStrings__AdaIs");
            if (!string.IsNullOrWhiteSpace(fromEnv))
            {
                return fromEnv;
            }

            string? host = Environment.GetEnvironmentVariable("DbConfig__Host");
            string? port = Environment.GetEnvironmentVariable("DbConfig__Port") ?? "5432";
            string? database = Environment.GetEnvironmentVariable("DbConfig__Database");
            string? username = Environment.GetEnvironmentVariable("DbConfig__Username");
            string? password = Environment.GetEnvironmentVariable("DbConfig__Password");
            if (!string.IsNullOrWhiteSpace(host)
                && !string.IsNullOrWhiteSpace(database)
                && !string.IsNullOrWhiteSpace(username)
                && password is not null)
            {
                return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
            }

            return "Host=127.0.0.1;Port=5432;Database=adais_ef_design;Username=postgres;Password=postgres";
        }

        private sealed class DesignTimeServiceScopeFactory : IServiceScopeFactory
        {
            /// <inheritdoc />
            public IServiceScope CreateScope()
                => throw new NotSupportedException("Design-time DbContext only.");
        }
    }
}
