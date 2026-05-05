namespace Azoxia.AdaIsAkademi.Infrastructure.DependencyInjection
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.AdaIsAkademi.Application.Services;
    using Azoxia.AdaIsAkademi.Infrastructure;
    using Azoxia.Core.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

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
            services.Configure<ObjectStoragePresignerOptions>(
                configuration.GetSection(ObjectStoragePresignerOptions.ConfigurationSectionName));

            services.AddScoped<IObjectStoragePresigner>(serviceProvider =>
            {
                ObjectStoragePresignerOptions optionsSnapshot =
                    serviceProvider.GetRequiredService<IOptions<ObjectStoragePresignerOptions>>().Value;

                bool fullyConfigured =
                    !string.IsNullOrWhiteSpace(optionsSnapshot.ServiceUrl) &&
                    !string.IsNullOrWhiteSpace(optionsSnapshot.AccessKey) &&
                    !string.IsNullOrWhiteSpace(optionsSnapshot.SecretKey) &&
                    !string.IsNullOrWhiteSpace(optionsSnapshot.BucketName);

                if (fullyConfigured)
                {
                    return new AwsS3CompatibleObjectStoragePresigner(
                        serviceProvider.GetRequiredService<IOptions<ObjectStoragePresignerOptions>>());
                }

                return new DevelopmentObjectStoragePresigner();
            });

            services.AddScoped<IPushNotificationSender, FakePushNotificationSender>();
        }

        #endregion Methods
    }
}
