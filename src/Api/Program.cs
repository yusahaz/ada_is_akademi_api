namespace Azoxia.AdaIsAkademi.Api
{
    using Azoxia.AdaIsAkademi.Api.Automation;
    using Azoxia.AdaIsAkademi.Api.DependencyInjection;
    using Azoxia.Core.Api;
    using Hangfire;
    using Hangfire.InMemory;

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
                builder.Services.AddHangfire(configuration => configuration
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseInMemoryStorage());
                builder.Services.AddHangfireServer();
                builder.Services.AddScoped<OverdueAlarmRecurringJob>();
            };

            startup.OnConfigurePipelines += (app) =>
            {
                app.UseHangfireDashboard("/automation/jobs");
                RecurringJob.AddOrUpdate<OverdueAlarmRecurringJob>(
                    recurringJobId: "overdue-alarm-sweep",
                    methodCall: x => x.ExecuteAsync(),
                    cronExpression: "*/30 * * * *");
            };

            startup.Run(args);
        }

        #endregion Utils
    }
}
