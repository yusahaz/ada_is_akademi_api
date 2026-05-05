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
    /// Sprint 6 tests for overdue summary reporting query.
    /// </summary>
    public sealed class Sprint6OverdueSummaryQueryTests
    {
        #region Methods

        [Fact]
        public async Task Overdue_summary_returns_only_open_or_filled_past_shift_postings_and_pending_applications()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            await SeedOverdueDataAsync(db);

            var handler = new GetOverdueJobSummaryQueryHandler(sp);
            OverdueJobSummaryModel model =
                await ((IRequestHandler<GetOverdueJobSummaryQuery, OverdueJobSummaryModel>)handler)
                    .HandleAsync(new GetOverdueJobSummaryQuery(), CancellationToken.None);

            model.OverduePostingCount.Should().Be(2);
            model.OverduePendingApplicationCount.Should().Be(1);
        }

        #endregion Methods

        #region Utils

        private static async Task SeedOverdueDataAsync(AdaIsAkademiDbContext db)
        {
            var category = new JobCategory("Retail", null);
            db.Set<JobCategory>().Add(category);
            await db.SaveChangesAsync();

            var employer = new Employer("ACME", null, "1234567890");
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", "employer-s6@test.local", "+905551112233"));
            EmployerLocation hq = employer.AddLocation("HQ");
            hq.SetAddress(new Address("Depo Sok. 2", "Istanbul", "TR"));
            hq.SetCoordinate(new GeoCoordinate(41.015137, 28.97953));
            hq.SetGeofenceRadiusMetres(1000);
            employer.SetAsActive();
            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();

            JobPosting overdueOpen = employer.AddJobPosting(
                hq.Id,
                category.Id,
                "Overdue Open",
                "Açıklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                headCount: 2);
            overdueOpen.Publish();

            JobPosting overdueFilled = employer.AddJobPosting(
                hq.Id,
                category.Id,
                "Overdue Filled",
                "Açıklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                headCount: 1);
            overdueFilled.Publish();

            JobPosting futureOpen = employer.AddJobPosting(
                hq.Id,
                category.Id,
                "Future Open",
                "Açıklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                headCount: 1);
            futureOpen.Publish();

            await db.SaveChangesAsync();

            Worker worker1 = await AddWorkerAsync(db, "worker1-s6@test.local");
            Worker worker2 = await AddWorkerAsync(db, "worker2-s6@test.local");
            Worker worker3 = await AddWorkerAsync(db, "worker3-s6@test.local");

            JobApplication pendingOverdueOpen = overdueOpen.AddApplication(worker1.Id, false, null);
            overdueFilled.AddApplication(worker2.Id, false, null);
            overdueFilled.AcceptApplication(overdueFilled.Applications[0].Id);
            overdueOpen.AddApplication(worker2.Id, false, null).Reject("reason");
            futureOpen.AddApplication(worker3.Id, false, null);
            pendingOverdueOpen.Withdraw();
            overdueOpen.AddApplication(worker3.Id, false, null);

            db.Entry(overdueOpen).Property(x => x.ShiftDate).CurrentValue = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2));
            db.Entry(overdueFilled).Property(x => x.ShiftDate).CurrentValue = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            await db.SaveChangesAsync();
        }

        private static async Task<Worker> AddWorkerAsync(AdaIsAkademiDbContext db, string email)
        {
            var user = new SystemUser(email, "Password1!", SystemUserType.Worker);
            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();

            var worker = new Worker(user.Id);
            db.Set<Worker>().Add(worker);
            await db.SaveChangesAsync();

            return worker;
        }

        #endregion Utils
    }
}
