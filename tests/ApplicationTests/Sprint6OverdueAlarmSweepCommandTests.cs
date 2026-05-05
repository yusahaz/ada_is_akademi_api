namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application.Tests.Support;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.AdaIsAkademi.Persistence;
    using Azoxia.Core.Application;
    using Azoxia.Core.Domain;
    using Azoxia.Core.ValueTypes;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Sprint 6 tests for overdue alarm sweep automation command.
    /// </summary>
    public sealed class Sprint6OverdueAlarmSweepCommandTests
    {
        #region Methods

        [Fact]
        public async Task Sweep_command_creates_daily_alarms_idempotently()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            await SeedOverduePostingsAsync(db);

            var handler = new RunOverdueAlarmSweepCommandHandler(sp);
            int firstRunCreated =
                await ((IRequestHandler<RunOverdueAlarmSweepCommand, int>)handler)
                    .HandleAsync(new RunOverdueAlarmSweepCommand(), CancellationToken.None);
            int secondRunCreated =
                await ((IRequestHandler<RunOverdueAlarmSweepCommand, int>)handler)
                    .HandleAsync(new RunOverdueAlarmSweepCommand(), CancellationToken.None);

            firstRunCreated.Should().Be(2);
            secondRunCreated.Should().Be(0);

            int alarmCount = await db.Set<OverdueJobAlarm>().CountAsync();
            alarmCount.Should().Be(2);
        }

        #endregion Methods

        #region Utils

        private static async Task SeedOverduePostingsAsync(AdaIsAkademiDbContext db)
        {
            var category = new JobCategory("Retail", null);
            db.Set<JobCategory>().Add(category);
            await db.SaveChangesAsync();

            var employer = new Employer("ACME", null, "1234567890");
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", "employer-s6-alarm@test.local", "+905551112233"));
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
                "Açıklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                headCount: 1);
            overdueSecond.Publish();

            JobPosting futureOpen = employer.AddJobPosting(
                hq.Id,
                category.Id,
                "Future Open",
                "Açıklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                headCount: 1);
            futureOpen.Publish();

            JobPosting deletedOverdue = employer.AddJobPosting(
                hq.Id,
                category.Id,
                "Deleted Overdue",
                "Açıklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                headCount: 1);
            deletedOverdue.Publish();

            await db.SaveChangesAsync();

            db.Entry(deletedOverdue).Property(nameof(DeletableEntityBase.IsDeleted)).CurrentValue = true;
            db.Entry(deletedOverdue).Property(nameof(DeletableEntityBase.DeletedAt)).CurrentValue = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync();
        }
        #endregion Utils
    }
}
