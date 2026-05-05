namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application.Tests.Support;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.AdaIsAkademi.Persistence;
    using Azoxia.Core.Application;
    using Azoxia.Core.ValueTypes;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Tests worker notification dispatch and retry command flow.
    /// </summary>
    public sealed class Sprint6SystemUserNotificationDispatchCommandTests
    {
        #region Methods

        [Fact]
        public async Task SendWorkerNotification_uses_push_when_device_token_exists()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (int employerId, int workerId) = await SeedWorkerAsync(db, withPushToken: true, verifiedEmail: true);
            executionContext.ReplaceClaim("employer_id", employerId.ToString());

            var handler = new SendWorkerNotificationCommandHandler(sp);
            int dispatchId = await ((IRequestHandler<SendWorkerNotificationCommand, int>)handler).HandleAsync(
                new SendWorkerNotificationCommand
                {
                    WorkerId = workerId,
                    TemplateCode = "worker.assignment.reminder",
                    Title = "Vardiya Hatirlatma",
                    Body = "Yarin vardiyan var.",
                },
                CancellationToken.None);

            SystemUserNotificationDispatch dispatch = await db.Set<SystemUserNotificationDispatch>().AsNoTracking().FirstAsync(x => x.Id == dispatchId);
            dispatch.Status.Should().Be(NotificationDispatchStatus.Sent);
            dispatch.Channel.Should().Be(NotificationChannel.Push);
            dispatch.FallbackReason.Should().BeNull();
        }

        [Fact]
        public async Task SendWorkerNotification_falls_back_to_email_when_push_token_missing()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (int employerId, int workerId) = await SeedWorkerAsync(db, withPushToken: false, verifiedEmail: true);
            executionContext.ReplaceClaim("employer_id", employerId.ToString());

            var handler = new SendWorkerNotificationCommandHandler(sp);
            int dispatchId = await ((IRequestHandler<SendWorkerNotificationCommand, int>)handler).HandleAsync(
                new SendWorkerNotificationCommand
                {
                    WorkerId = workerId,
                    TemplateCode = "worker.assignment.reminder",
                    Title = "Vardiya Hatirlatma",
                    Body = "Yarin vardiyan var.",
                },
                CancellationToken.None);

            SystemUserNotificationDispatch dispatch = await db.Set<SystemUserNotificationDispatch>().AsNoTracking().FirstAsync(x => x.Id == dispatchId);
            dispatch.Status.Should().Be(NotificationDispatchStatus.Sent);
            dispatch.Channel.Should().Be(NotificationChannel.Email);
            dispatch.FallbackReason.Should().Be("missing_push_token");
        }

        [Fact]
        public async Task SendSystemUserNotification_supports_employer_account()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            int employerSystemUserId = await SeedSystemUserAsync(db, SystemUserType.Employer, withPushToken: false, verifiedEmail: true);
            executionContext.ReplaceClaim("system_user_id", employerSystemUserId.ToString());

            var handler = new SendSystemUserNotificationCommandHandler(sp);
            int dispatchId = await ((IRequestHandler<SendSystemUserNotificationCommand, int>)handler).HandleAsync(
                new SendSystemUserNotificationCommand
                {
                    SystemUserId = employerSystemUserId,
                    TemplateCode = "employer.ops.reminder",
                    Title = "Hatirlatma",
                    Body = "Panelde yeni islem var.",
                },
                CancellationToken.None);

            SystemUserNotificationDispatch dispatch = await db.Set<SystemUserNotificationDispatch>().AsNoTracking().FirstAsync(x => x.Id == dispatchId);
            dispatch.SystemUserId.Should().Be(employerSystemUserId);
            dispatch.WorkerId.Should().BeNull();
            dispatch.Channel.Should().Be(NotificationChannel.Email);
        }

        [Fact]
        public async Task SendSystemUserNotification_supports_admin_account()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            int adminSystemUserId = await SeedSystemUserAsync(db, SystemUserType.Admin, withPushToken: true, verifiedEmail: true);
            executionContext.ReplaceClaim("system_user_id", adminSystemUserId.ToString());

            var handler = new SendSystemUserNotificationCommandHandler(sp);
            int dispatchId = await ((IRequestHandler<SendSystemUserNotificationCommand, int>)handler).HandleAsync(
                new SendSystemUserNotificationCommand
                {
                    SystemUserId = adminSystemUserId,
                    TemplateCode = "admin.ops.alert",
                    Title = "Sistem Uyarisi",
                    Body = "Inceleme gerektiren yeni kayit var.",
                },
                CancellationToken.None);

            SystemUserNotificationDispatch dispatch = await db.Set<SystemUserNotificationDispatch>().AsNoTracking().FirstAsync(x => x.Id == dispatchId);
            dispatch.SystemUserId.Should().Be(adminSystemUserId);
            dispatch.WorkerId.Should().BeNull();
            dispatch.Channel.Should().Be(NotificationChannel.Push);
        }

        #endregion Methods

        #region Utils

        private static async Task<(int employerId, int workerId)> SeedWorkerAsync(
            AdaIsAkademiDbContext db,
            bool withPushToken,
            bool verifiedEmail)
        {
            Employer employer = new("ACME", null, "1234567890");
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", "notif-seed@test.local", "+905551112233"));
            employer.SetAsActive();
            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();

            SystemUser user = new("worker-notif@test.local", "Password1!", SystemUserType.Worker);
            if (verifiedEmail)
            {
                user.RequestEmailVerification("verify-notif", DateTimeOffset.UtcNow.AddHours(1));
                user.VerifyEmail("verify-notif");
            }
            if (withPushToken)
            {
                user.AddOrUpdateDevice("device-notif", DevicePlatform.Android, "push-token");
            }

            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();

            Worker worker = new(user.Id);
            db.Set<Worker>().Add(worker);
            await db.SaveChangesAsync();

            return (employer.Id, worker.Id);
        }

        private static async Task<int> SeedSystemUserAsync(
            AdaIsAkademiDbContext db,
            SystemUserType type,
            bool withPushToken,
            bool verifiedEmail)
        {
            SystemUser user = new($"{type.ToString().ToLowerInvariant()}-notif@test.local", "Password1!", type);
            if (verifiedEmail)
            {
                user.RequestEmailVerification("verify-notif-generic", DateTimeOffset.UtcNow.AddHours(1));
                user.VerifyEmail("verify-notif-generic");
            }

            if (withPushToken)
            {
                user.AddOrUpdateDevice("device-notif-generic", DevicePlatform.Android, "push-token-generic");
            }

            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();
            return user.Id;
        }

        #endregion Utils
    }
}
