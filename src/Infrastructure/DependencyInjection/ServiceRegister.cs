namespace Azoxia.AdaIsAkademi.Infrastructure.DependencyInjection
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.AdaIsAkademi.Application.Services;
    using Azoxia.AdaIsAkademi.Infrastructure;
    using Azoxia.AdaIsAkademi.Infrastructure.Configuration;
    using Azoxia.Core.Configuration;
    using Azoxia.Core.DependencyInjection;
    using Azoxia.Core.Extensions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Registers infrastructure adapters for external providers.
    /// </summary>
    public class ServiceRegister :
        IServiceRegister
    {
        #region Methods

        /// <inheritdoc />
        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            ObjectStorageConfig objectStorage =
                Config.GetOrCreateConfig<ObjectStorageConfig>(configuration);

            services.AddScoped<IObjectStoragePresigner>(_ =>
            {
                bool fullyConfigured =
                    !objectStorage.ServiceUrl.IsNullOrWhiteSpace() &&
                    !objectStorage.AccessKey.IsNullOrWhiteSpace() &&
                    !objectStorage.SecretKey.IsNullOrWhiteSpace() &&
                    !objectStorage.BucketName.IsNullOrWhiteSpace();

                if (fullyConfigured)
                {
                    return new AwsS3CompatibleObjectStoragePresigner(objectStorage);
                }

                return new DevelopmentObjectStoragePresigner();
            });

            services.AddScoped<IPushNotificationSender, FakePushNotificationSender>();
            services.AddScoped<ICvExtractionService, FakeCvExtractionService>();
        }

        #endregion Methods
    }
}
