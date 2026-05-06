namespace Azoxia.AdaIsAkademi.Api
{
    using System;
    using Azoxia.AdaIsAkademi.Api.Automation;
    using Azoxia.AdaIsAkademi.Api.DependencyInjection;
    using Azoxia.Core.Api;
    using Hangfire;
    using Hangfire.InMemory;
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
                string[] allowedOrigins = builder.Configuration
                    .GetSection("Cors:AllowedOrigins")
                    .Get<string[]>() ?? [];
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("Frontend", policy =>
                    {
                        string[] effectiveOrigins = allowedOrigins.Length == 0
                            ? [
                                "https://adaisakademi.com",
                                "https://*.adaisakademi.com",
                                "http://localhost:3000",
                                "http://localhost:5173",
                                "https://localhost:3000",
                                "https://localhost:5173"
                            ]
                            : allowedOrigins;

                        bool isLocalDevelopmentOrigin(string origin)
                        {
                            if (!builder.Environment.IsDevelopment())
                            {
                                return false;
                            }

                            if (!Uri.TryCreate(origin, UriKind.Absolute, out Uri? uri))
                            {
                                return false;
                            }

                            return string.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase)
                                || string.Equals(uri.Host, "127.0.0.1", StringComparison.OrdinalIgnoreCase);
                        }

                        bool isConfiguredOrigin(string origin)
                        {
                            if (isLocalDevelopmentOrigin(origin))
                            {
                                return true;
                            }

                            if (!Uri.TryCreate(origin, UriKind.Absolute, out Uri? originUri))
                            {
                                return false;
                            }

                            foreach (string configuredOrigin in effectiveOrigins)
                            {
                                if (!Uri.TryCreate(configuredOrigin, UriKind.Absolute, out Uri? configuredUri))
                                {
                                    continue;
                                }

                                if (configuredUri.Host.StartsWith("*.", StringComparison.Ordinal))
                                {
                                    string wildcardSuffix = configuredUri.Host[1..];
                                    bool isWildcardMatch =
                                        string.Equals(originUri.Scheme, configuredUri.Scheme, StringComparison.OrdinalIgnoreCase)
                                        && originUri.Host.EndsWith(wildcardSuffix, StringComparison.OrdinalIgnoreCase);
                                    if (isWildcardMatch)
                                    {
                                        return true;
                                    }

                                    continue;
                                }

                                bool isExactMatch = string.Equals(origin, configuredOrigin, StringComparison.OrdinalIgnoreCase);
                                if (isExactMatch)
                                {
                                    return true;
                                }
                            }

                            return false;
                        }

                        policy
                            .SetIsOriginAllowed(isConfiguredOrigin)
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
                builder.Services.AddHangfire(configuration => configuration
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseInMemoryStorage());
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

        #endregion Utils
    }
}
