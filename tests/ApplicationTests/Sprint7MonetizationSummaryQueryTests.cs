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
    /// Sprint 7 tests for monetization summary query.
    /// </summary>
    public sealed class Sprint7MonetizationSummaryQueryTests
    {
        #region Methods

        [Fact]
        public async Task Monetization_summary_returns_expected_counters_and_estimations()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            await SeedMonetizationDataAsync(db);

            var handler = new GetMonetizationSummaryQueryHandler(sp);
            MonetizationSummaryModel model =
                await ((IRequestHandler<GetMonetizationSummaryQuery, MonetizationSummaryModel>)handler)
                    .HandleAsync(new GetMonetizationSummaryQuery(), CancellationToken.None);

            model.AcceptedJobApplicationCount.Should().Be(2);
            model.ActiveEmployerCount.Should().Be(1);
            model.FilledOrCompletedJobPostingCount.Should().Be(2);
            model.EstimatedGrossTransactionVolume.Should().Be(200m);
            model.EstimatedCommissionAmount.Should().Be(20m);
        }

        #endregion Methods

        #region Utils

        private static async Task SeedMonetizationDataAsync(AdaIsAkademiDbContext db)
        {
            JobCategory category = new("Retail", null);
            db.Set<JobCategory>().Add(category);
            await db.SaveChangesAsync();

            Employer activeEmployer = new("ACME", null, "1234567890");
            activeEmployer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            activeEmployer.SetContact(new Contact("Ali", "Yilmaz", "employer-s7@test.local", "+905551112233"));
            EmployerLocation location = activeEmployer.AddLocation("HQ");
            location.SetAddress(new Address("Depo Sok. 2", "Istanbul", "TR"));
            location.SetCoordinate(new GeoCoordinate(41.015137, 28.97953));
            location.SetGeofenceRadiusMetres(1000);
            activeEmployer.SetAsActive();

            Employer suspendedEmployer = new("Other", null, "9876543210");
            suspendedEmployer.SetAddress(new Address("No:1", "Istanbul", "TR"));
            suspendedEmployer.SetContact(new Contact("Ayse", "Kara", "suspended@test.local", "+905552223344"));
            suspendedEmployer.SetAsSuspended();

            db.Set<Employer>().Add(activeEmployer);
            db.Set<Employer>().Add(suspendedEmployer);
            await db.SaveChangesAsync();

            Worker worker1 = await AddWorkerAsync(db, "worker1-s7@test.local");
            Worker worker2 = await AddWorkerAsync(db, "worker2-s7@test.local");

            JobPosting postingFilled = activeEmployer.AddJobPosting(
                location.Id,
                category.Id,
                "Filled Posting",
                "Aciklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                headCount: 1);
            postingFilled.Publish();
            JobApplication accepted1 = postingFilled.AddApplication(worker1.Id, false, null);
            postingFilled.AcceptApplication(accepted1.Id);

            JobPosting postingCompleted = activeEmployer.AddJobPosting(
                location.Id,
                category.Id,
                "Completed Posting",
                "Aciklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                headCount: 2);
            postingCompleted.Publish();
            JobApplication accepted2 = postingCompleted.AddApplication(worker2.Id, false, null);
            postingCompleted.AcceptApplication(accepted2.Id);
            postingCompleted.Complete();

            await db.SaveChangesAsync();
        }

        private static async Task<Worker> AddWorkerAsync(AdaIsAkademiDbContext db, string email)
        {
            SystemUser user = new(email, "Password1!", SystemUserType.Worker);
            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();

            Worker worker = new(user.Id);
            db.Set<Worker>().Add(worker);
            await db.SaveChangesAsync();

            return worker;
        }

        #endregion Utils
    }
}
