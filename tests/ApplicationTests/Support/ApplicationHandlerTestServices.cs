namespace Azoxia.AdaIsAkademi.Application.Tests.Support
{
    using Azoxia.AdaIsAkademi.Application.DependencyInjection;
    using Azoxia.AdaIsAkademi.Application.Services;
    using Azoxia.AdaIsAkademi.Persistence;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application;
    using Azoxia.Core.Identity;
    using Azoxia.Core.Persistence;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Builds a DI root with SQLite in-memory EF Core and production <see cref="UnitOfWork{TDbContext}"/> wiring for handler tests.
    /// </summary>
    public static class ApplicationHandlerTestServices
    {
        #region Methods

        /// <summary>
        /// Creates a new service provider with an isolated SQLite in-memory database and materialized schema.
        /// </summary>
        /// <param name="executionContext">Optional execution context override for claim-driven handler scenarios.</param>
        /// <returns>Root provider; dispose to release the SQLite connection holder.</returns>
        public static ServiceProvider CreateProvider(IExecutionContext? executionContext = null)
        {
            IServiceCollection services = new ServiceCollection();
            var holder = new SqliteConnectionHolder();
            services.AddSingleton(holder);
            services.AddDbContext<AdaIsAkademiDbContext>(options =>
                options.UseSqlite(holder.Connection));
            services.AddScoped<IUnitOfWork, UnitOfWork<AdaIsAkademiDbContext>>();
            services.AddAdaIsDomainEventHandling();
            services.AddScoped<IWorkerProfileCompletionEvaluator, WorkerProfileCompletionEvaluator>();
            services.AddScoped<IWorkerEmployerProfileAccess, WorkerEmployerProfileAccess>();
            services.AddSingleton<IObjectStoragePresigner, TestObjectStoragePresigner>();
            services.AddSingleton<ICacheService, NullCacheService>();
            services.AddSingleton<IExecutionContext>(executionContext ?? new TestExecutionContext());
            services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));

            ServiceProvider provider = services.BuildServiceProvider();
            using (IServiceScope bootstrap = provider.CreateScope())
            {
                AdaIsAkademiDbContext db = bootstrap.ServiceProvider.GetRequiredService<AdaIsAkademiDbContext>();
                db.Database.EnsureCreated();
            }

            return provider;
        }

        #endregion Methods
    }
}
