namespace Azoxia.AdaIsAkademi.Persistence.DependencyInjection
{
    using Azoxia.Core.Configuration;
    using Azoxia.Core.DependencyInjection;
    using Azoxia.Core.Persistence;
    using Azoxia.Core.Persistence.Configs;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Registers persistence and infrastructure services for dependency injection.
    /// </summary>
    public class ServiceRegister :
        IServiceRegister
    {
        #region Methods

        /// <inheritdoc />
        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AdaIsAkademiDbContext>((_, options) =>
            {
                DbConfig dbConfig = Config.GetConfig<DbConfig>();

                options.UseNpgsql(dbConfig.ConnectionString);
                options.UseLazyLoadingProxies();
            });

            services.AddScoped<IUnitOfWork, UnitOfWork<AdaIsAkademiDbContext>>();
        }

        #endregion Methods
    }
}
