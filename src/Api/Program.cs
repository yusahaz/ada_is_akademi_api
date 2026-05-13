namespace Azoxia.AdaIsAkademi.Api
{
    using System.Collections.Generic;
    using System;
    using Azoxia.AdaIsAkademi.Api.Automation;
    using Azoxia.AdaIsAkademi.Api.DependencyInjection;
    using Azoxia.Core.Api;
    using Azoxia.Core.Configuration;
    using Azoxia.Core.Persistence.Configs;
    using Hangfire;
    using Hangfire.PostgreSql;
    using Microsoft.AspNetCore.DataProtection;
    using System.IO;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Host process entry type for the Ada Is Akademi API.
    /// </summary>
    class Program
    {
        private const string CorsPolicyName = "Frontend";

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
                string[] configuredOrigins = builder.Configuration
                    .GetSection("Cors:AllowedOrigins")
                    .Get<string[]>() ?? [];

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy(CorsPolicyName, policy =>
                    {
                        policy
                            .SetIsOriginAllowed(origin =>
                                builder.Environment.IsDevelopment()
                                || IsAllowedOrigin(
                                    origin,
                                    configuredOrigins))
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
                });
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
                IDataProtectionBuilder dataProtection = builder.Services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo("/home/app/.aspnet/DataProtection-Keys"))
                    .SetApplicationName("Azoxia.AdaIsAkademi.Api");

                string? certificatePath = builder.Configuration["DataProtection:ProtectKeysCertificatePath"];
                if (!string.IsNullOrWhiteSpace(certificatePath) && File.Exists(certificatePath))
                {
                    string password = builder.Configuration["DataProtection:ProtectKeysCertificatePassword"] ?? string.Empty;
                    X509Certificate2 certificate = X509CertificateLoader.LoadPkcs12FromFile(
                        certificatePath,
                        password,
                        X509KeyStorageFlags.EphemeralKeySet);
                    dataProtection.ProtectKeysWithCertificate(certificate);
                }
                string hangfireConnectionString =
                    builder.Configuration["Hangfire:ConnectionString"];
                if (string.IsNullOrWhiteSpace(hangfireConnectionString))
                {
                    DbConfig dbConfig = Config.GetOrCreateConfig<DbConfig>(builder.Configuration);
                    hangfireConnectionString = dbConfig.ConnectionString;
                }

                if (string.IsNullOrWhiteSpace(hangfireConnectionString))
                {
                    throw new InvalidOperationException(
                        "Hangfire PostgreSQL storage requires Hangfire:ConnectionString or a composed DbConfig connection string.");
                }

                string hangfireSchema =
                    builder.Configuration["Hangfire:SchemaName"];
                if (string.IsNullOrWhiteSpace(hangfireSchema))
                {
                    hangfireSchema = "hangfire";
                }

                builder.Services.AddHangfire(configuration => configuration
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(
                        bootstrap => bootstrap.UseNpgsqlConnection(hangfireConnectionString),
                        new PostgreSqlStorageOptions
                        {
                            SchemaName = hangfireSchema,
                            PrepareSchemaIfNecessary = true,
                        }));
                builder.Services.AddHangfireServer();
                builder.Services.AddScoped<OverdueAlarmRecurringJob>();
                builder.Services.AddScoped<EmbeddingRefreshRecurringJob>();
                builder.Services.AddScoped<CvExtractionRecurringJob>();
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
                RecurringJob.AddOrUpdate<CvExtractionRecurringJob>(
                    recurringJobId: "cv-extraction-sweep",
                    methodCall: x => x.ExecuteAsync(),
                    cronExpression: "*/15 * * * *");
                RecurringJob.AddOrUpdate<RetryFailedSystemUserNotificationsRecurringJob>(
                    recurringJobId: "notification-retry-sweep",
                    methodCall: x => x.ExecuteAsync(),
                    cronExpression: "*/10 * * * *");
            };

            startup.Run(args);
        }

        private static bool IsAllowedOrigin(string origin, IEnumerable<string> configuredOrigins)
        {
            if (!TryNormalizeOrigin(origin, out string? normalizedOrigin, out Uri? requestUri))
            {
                return false;
            }

            foreach (string configuredOrigin in configuredOrigins)
            {
                if (!TryNormalizeOrigin(configuredOrigin, out string? normalizedConfiguredOrigin, out Uri? configuredUri))
                {
                    continue;
                }

                if (IsWildcardHost(configuredUri.Host))
                {
                    if (MatchesWildcardHost(requestUri, configuredUri))
                    {
                        return true;
                    }

                    continue;
                }

                if (string.Equals(normalizedOrigin, normalizedConfiguredOrigin, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryNormalizeOrigin(string value, out string? normalizedOrigin, out Uri? uri)
        {
            normalizedOrigin = null;
            uri = null;
            if (!Uri.TryCreate(value, UriKind.Absolute, out Uri? parsedUri))
            {
                return false;
            }

            uri = parsedUri;
            normalizedOrigin = parsedUri.GetLeftPart(UriPartial.Authority);
            return true;
        }

        private static bool IsWildcardHost(string host)
        {
            return host.StartsWith("*.", StringComparison.Ordinal);
        }

        private static bool MatchesWildcardHost(Uri requestUri, Uri configuredWildcardUri)
        {
            if (!string.Equals(requestUri.Scheme, configuredWildcardUri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string suffix = configuredWildcardUri.Host[1..];
            return requestUri.Host.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
        }

        #endregion Utils
    }
}
