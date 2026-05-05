namespace Azoxia.AdaIsAkademi.Api
{
    using Azoxia.AdaIsAkademi.Api.Automation;
    using Azoxia.AdaIsAkademi.Api.DependencyInjection;
    using Azoxia.Core.Api;
    using Hangfire;
    using Hangfire.InMemory;
    using Microsoft.AspNetCore.DataProtection;
    using System.IO;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Host process entry type for the Ada Is Akademi API.
    /// </summary>
    class Program
    {
        #region Utils

        /// <summary>
        /// Boots the API host using the shared startup pipeline.
        /// </summary>
        /// <param name="args">Raw command-line arguments.</param>
        static void Main(string[] args)
        {
            Startup startup = new();

            startup.OnConfigureServices += (builder) =>
            {
                builder.Services.AddAzoxiaCore(builder.Configuration);
                builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(
                    configureOptions: options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(
                            item: new JsonStringEnumConverter());
                    });
                builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(
                    configureOptions: options =>
                    {
                        options.SerializerOptions.Converters.Add(
                            item: new JsonStringEnumConverter());
                    });
                builder.Services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo("/home/app/.aspnet/DataProtection-Keys"));
                builder.Services.AddHangfire(configuration => configuration
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseInMemoryStorage());
                builder.Services.AddHangfireServer();
                builder.Services.AddScoped<OverdueAlarmRecurringJob>();
                builder.Services.AddScoped<EmbeddingRefreshRecurringJob>();
                builder.Services.AddScoped<RetryFailedSystemUserNotificationsRecurringJob>();
            };

            startup.OnConfigurePipelines += (app) =>
            {
                app.UseHangfireDashboard("/automation/jobs");
                RecurringJob.AddOrUpdate<OverdueAlarmRecurringJob>(
                    recurringJobId: "overdue-alarm-sweep",
                    methodCall: x => x.ExecuteAsync(),
                    cronExpression: "*/30 * * * *");
                RecurringJob.AddOrUpdate<EmbeddingRefreshRecurringJob>(
                    recurringJobId: "embedding-refresh-sweep",
                    methodCall: x => x.ExecuteAsync(),
                    cronExpression: "0 * * * *");
                RecurringJob.AddOrUpdate<RetryFailedSystemUserNotificationsRecurringJob>(
                    recurringJobId: "notification-retry-sweep",
                    methodCall: x => x.ExecuteAsync(),
                    cronExpression: "*/10 * * * *");
            };

            startup.Run(args);
        }

        #endregion Utils
    }
}
