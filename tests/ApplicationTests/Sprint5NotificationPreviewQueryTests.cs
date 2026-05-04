namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application.Tests.Support;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.AdaIsAkademi.Persistence;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.ValueTypes;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Sprint 5 query tests for personalized notification preview flow.
    /// </summary>
    public sealed class Sprint5NotificationPreviewQueryTests
    {
        #region Methods

        [Fact]
        public async Task Notification_preview_falls_back_to_email_when_worker_has_no_push_token()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (int workerId, int postingId) = await SeedWorkerAndPostingAsync(db, withDeviceToken: false);
            executionContext.ReplaceClaim("worker_id", workerId.ToString());

            var handler = new GetWorkerPersonalizedNotificationPreviewQueryHandler(sp);
            WorkerNotificationPreviewModel model = await ((IRequestHandler<GetWorkerPersonalizedNotificationPreviewQuery, WorkerNotificationPreviewModel>)handler)
                .HandleAsync(
                    new GetWorkerPersonalizedNotificationPreviewQuery
                    {
                        JobPostingId = postingId,
                    },
                    CancellationToken.None);

            model.Channel.Should().Be("email");
            model.FallbackApplied.Should().BeTrue();
            model.JobPostingId.Should().Be(postingId);
        }

        [Fact]
        public void Notification_preview_query_requires_positive_job_posting_id()
        {
            var validator = new GetWorkerPersonalizedNotificationPreviewQueryValidator();
            ValidationResult result = validator.Validate(
                new GetWorkerPersonalizedNotificationPreviewQuery
                {
                    JobPostingId = 0,
                });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.Field == nameof(GetWorkerPersonalizedNotificationPreviewQuery.JobPostingId));
        }

        #endregion Methods

        #region Utils

        private static async Task<(int workerId, int postingId)> SeedWorkerAndPostingAsync(
            AdaIsAkademiDbContext db,
            bool withDeviceToken)
        {
            var category = new JobCategory("Retail", null);
            db.Set<JobCategory>().Add(category);
            await db.SaveChangesAsync();

            var employer = new Employer("ACME", null, "1234567890");
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", "employer-s5@test.local", "+905551112233"));
            EmployerLocation hq = employer.AddLocation("HQ");
            hq.SetAddress(new Address("Depo Sok. 2", "Istanbul", "TR"));
            hq.SetCoordinate(new GeoCoordinate(41.015137, 28.97953));
            hq.SetGeofenceRadiusMetres(1000);
            employer.SetAsActive();
            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();

            JobPosting posting = employer.AddJobPosting(
                hq.Id,
                category.Id,
                "Kasiyer",
                "Açıklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                headCount: 2);
            posting.Publish();
            await db.SaveChangesAsync();

            var user = new SystemUser("worker-s5@test.local", "Password1!", SystemUserType.Worker);
            user.RequestEmailVerification("verify-s5", DateTimeOffset.UtcNow.AddHours(1));
            user.VerifyEmail("verify-s5");
            if (withDeviceToken)
            {
                user.AddOrUpdateDevice("device-s5", DevicePlatform.Android, "push-token");
            }
            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();

            var worker = new Worker(user.Id);
            db.Set<Worker>().Add(worker);
            await db.SaveChangesAsync();

            return (worker.Id, posting.Id);
        }

        #endregion Utils
    }
}
