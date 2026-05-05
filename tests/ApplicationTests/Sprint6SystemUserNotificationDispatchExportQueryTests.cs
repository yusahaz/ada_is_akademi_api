namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application.Tests.Support;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.AdaIsAkademi.Persistence;
    using Azoxia.Core.Application;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Tests CSV export query for notification dispatch rows.
    /// </summary>
    public sealed class Sprint6SystemUserNotificationDispatchExportQueryTests
    {
        #region Methods

        [Fact]
        public async Task ExportSystemUserNotificationDispatchesCsv_returns_rows_after_dispatch_creation()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            int systemUserId = await Sprint6SystemUserNotificationDispatchCommandTestsAccessor.SeedSystemUserAsync(db, withPushToken: true);
            executionContext.ReplaceClaim("system_user_id", systemUserId.ToString());

            var sendHandler = new SendSystemUserNotificationCommandHandler(sp);
            _ = await ((IRequestHandler<SendSystemUserNotificationCommand, int>)sendHandler).HandleAsync(
                new SendSystemUserNotificationCommand
                {
                    SystemUserId = systemUserId,
                    TemplateCode = "admin.ops.alert",
                    Title = "Bildirim",
                    Body = "Deneme bildirimi",
                },
                CancellationToken.None);

            var exportHandler = new ExportSystemUserNotificationDispatchesCsvQueryHandler(sp);
            SystemUserNotificationDispatchExportPackageModel result =
                await ((IRequestHandler<ExportSystemUserNotificationDispatchesCsvQuery, SystemUserNotificationDispatchExportPackageModel>)exportHandler)
                    .HandleAsync(new ExportSystemUserNotificationDispatchesCsvQuery(), CancellationToken.None);

            result.RowCount.Should().BeGreaterThan(0);
            result.CsvContent.Should().Contain("dispatch_id,system_user_id,system_user_type");
            result.CsvContent.Should().Contain(systemUserId.ToString());
        }

        #endregion Methods
    }

    internal static class Sprint6SystemUserNotificationDispatchCommandTestsAccessor
    {
        public static async Task<int> SeedSystemUserAsync(AdaIsAkademiDbContext db, bool withPushToken)
        {
            SystemUser user = new($"export-notif-{Guid.NewGuid():N}@test.local", "Password1!", SystemUserType.Admin);
            user.RequestEmailVerification("verify-export", DateTimeOffset.UtcNow.AddHours(1));
            user.VerifyEmail("verify-export");
            if (withPushToken)
            {
                user.AddOrUpdateDevice("device-export", DevicePlatform.Android, "push-export-token");
            }

            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();
            return user.Id;
        }
    }
}
