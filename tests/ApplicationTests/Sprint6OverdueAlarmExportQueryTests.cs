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
    /// Sprint 6 tests for overdue alarm CSV export query.
    /// </summary>
    public sealed class Sprint6OverdueAlarmExportQueryTests
    {
        #region Methods

        [Fact]
        public async Task Export_query_returns_csv_package_from_overdue_alarms()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            await SeedOverduePostingsAsync(db);
            await RunSweepAsync(sp);

            var handler = new ExportOverdueAlarmsCsvQueryHandler(sp);
            OverdueAlarmExportPackageModel model =
                await ((IRequestHandler<ExportOverdueAlarmsCsvQuery, OverdueAlarmExportPackageModel>)handler)
                    .HandleAsync(new ExportOverdueAlarmsCsvQuery(), CancellationToken.None);

            model.ContentType.Should().Be("text/csv");
            model.RowCount.Should().Be(2);
            model.CsvContent.Should().Contain("alarm_date,job_posting_id,title,job_posting_status,shift_date");
            model.CsvContent.Should().Contain("Overdue Open");
            model.CsvContent.Should().Contain("Overdue Second");
        }

        #endregion Methods

        #region Utils

        private static async Task RunSweepAsync(IServiceProvider sp)
        {
            var sweepHandler = new RunOverdueAlarmSweepCommandHandler(sp);
            await ((IRequestHandler<RunOverdueAlarmSweepCommand, int>)sweepHandler)
                .HandleAsync(new RunOverdueAlarmSweepCommand(), CancellationToken.None);
        }

        private static async Task SeedOverduePostingsAsync(AdaIsAkademiDbContext db)
        {
            var category = new JobCategory("Retail", null);
            db.Set<JobCategory>().Add(category);
            await db.SaveChangesAsync();

            var employer = new Employer("ACME", null, "1234567890");
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", "employer-s6-export@test.local", "+905551112233"));
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
                "Aciklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                headCount: 2);
            overdueOpen.Publish();

            JobPosting overdueSecond = employer.AddJobPosting(
                hq.Id,
                category.Id,
                "Overdue Second",
                "Aciklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                headCount: 1);
            overdueSecond.Publish();

            await db.SaveChangesAsync();
        }

        #endregion Utils
    }
}
