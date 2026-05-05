namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application.Tests.Support;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.AdaIsAkademi.Persistence;
    using Azoxia.Core.Application;
    using Azoxia.Core.ValueTypes;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Tests worker live status feed query for assignment and matching updates.
    /// </summary>
    public sealed class Sprint5LiveStatusFeedQueryTests
    {
        #region Methods

        [Fact]
        public async Task Live_status_feed_returns_assignment_and_matching_items()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            (int workerId, int assignmentId, int postingId) = await SeedLiveStatusDataAsync(db);
            executionContext.ReplaceClaim("worker_id", workerId.ToString());

            var handler = new GetWorkerLiveStatusFeedQueryHandler(sp);
            WorkerLiveStatusFeedModel feed = await ((IRequestHandler<GetWorkerLiveStatusFeedQuery, WorkerLiveStatusFeedModel>)handler)
                .HandleAsync(
                    new GetWorkerLiveStatusFeedQuery
                    {
                        Limit = 10,
                    },
                    CancellationToken.None);

            feed.Items.Should().NotBeEmpty();
            feed.Items.Should().Contain(x => x.ItemType == "assignment_status" && x.ReferenceId == assignmentId);
            feed.Items.Should().Contain(x => x.ItemType == "matching_update" && x.ReferenceId == postingId);
        }

        #endregion Methods

        #region Utils

        private static async Task<(int workerId, int assignmentId, int postingId)> SeedLiveStatusDataAsync(AdaIsAkademiDbContext db)
        {
            JobCategory category = new("Retail", null);
            db.Set<JobCategory>().Add(category);
            await db.SaveChangesAsync();

            Employer employer = new("ACME", null, "1234567890");
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", "live-status@test.local", "+905551112233"));
            EmployerLocation location = employer.AddLocation("HQ");
            location.SetAddress(new Address("Depo Sok. 2", "Istanbul", "TR"));
            location.SetCoordinate(new GeoCoordinate(41.015137, 28.97953));
            location.SetGeofenceRadiusMetres(1000);
            employer.SetAsActive();
            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();

            SystemUser user = new("worker-live@test.local", "Password1!", SystemUserType.Worker);
            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();

            Worker worker = new(user.Id);
            db.Set<Worker>().Add(worker);
            await db.SaveChangesAsync();

            JobPosting assignedPosting = employer.AddJobPosting(
                location.Id,
                category.Id,
                "Kasiyer Assigned",
                "Açıklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                1);
            assignedPosting.Publish();
            JobApplication application = assignedPosting.AddApplication(worker.Id, hasConflictingShift: false, note: null);
            assignedPosting.AcceptApplication(application.Id);

            JobPosting recommendationPosting = employer.AddJobPosting(
                location.Id,
                category.Id,
                "Kasiyer Recommendation",
                "Açıklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                new TimeOnly(10, 0),
                new TimeOnly(19, 0),
                new Money(130m, "TRY"),
                1);
            recommendationPosting.Publish();
            await db.SaveChangesAsync();

            ShiftAssignment assignment = new(
                assignedPosting.Id,
                application.Id,
                worker.Id,
                "qr-token-hash",
                "supervisor-qr-token-hash");
            db.Set<ShiftAssignment>().Add(assignment);
            await db.SaveChangesAsync();

            return (worker.Id, assignment.Id, recommendationPosting.Id);
        }

        #endregion Utils
    }
}
