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
    /// Sprint 7 tests for employer commission summary list query.
    /// </summary>
    public sealed class Sprint7EmployerCommissionSummaryListQueryTests
    {
        #region Methods

        [Fact]
        public async Task Commission_summary_list_should_return_active_employers_ordered_by_estimate()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            await SeedEmployersAsync(db);

            var handler = new ListEmployerCommissionSummariesQueryHandler(sp);
            IReadOnlyList<EmployerCommissionListItemModel> result =
                await ((IRequestHandler<ListEmployerCommissionSummariesQuery, IReadOnlyList<EmployerCommissionListItemModel>>)handler)
                    .HandleAsync(new ListEmployerCommissionSummariesQuery { Limit = 10 }, CancellationToken.None);

            result.Should().HaveCount(2);
            result[0].EmployerName.Should().Be("ALPHA");
            result[0].EstimatedCommissionAmount.Should().Be(30m);
            result[1].EmployerName.Should().Be("BETA");
            result[1].EstimatedCommissionAmount.Should().Be(5m);
        }

        [Fact]
        public void Commission_summary_list_validator_should_fail_for_out_of_range_limit()
        {
            var validator = new ListEmployerCommissionSummariesQueryValidator();
            ValidationResult result = validator.Validate(new ListEmployerCommissionSummariesQuery { Limit = 0 });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.Field == nameof(ListEmployerCommissionSummariesQuery.Limit));
        }

        #endregion Methods

        #region Utils

        private static async Task SeedEmployersAsync(AdaIsAkademiDbContext db)
        {
            JobCategory category = new("Retail", null);
            db.Set<JobCategory>().Add(category);
            await db.SaveChangesAsync();

            Employer alpha = await AddEmployerAsync(db, "ALPHA", "alpha@test.local", 0.15m);
            Employer beta = await AddEmployerAsync(db, "BETA", "beta@test.local", 0.10m);
            await AddEmployerAsync(db, "SUSP", "susp@test.local", 0.25m, active: false);

            Worker worker1 = await AddWorkerAsync(db, "w1@test.local");
            Worker worker2 = await AddWorkerAsync(db, "w2@test.local");
            Worker worker3 = await AddWorkerAsync(db, "w3@test.local");

            await AddAcceptedPostingAsync(db, alpha, category, worker1, 100m);
            await AddAcceptedPostingAsync(db, alpha, category, worker2, 100m);
            await AddAcceptedPostingAsync(db, beta, category, worker3, 50m);
        }

        private static async Task<Employer> AddEmployerAsync(
            AdaIsAkademiDbContext db,
            string name,
            string email,
            decimal commissionRate,
            bool active = true)
        {
            Employer employer = new(name, null, Guid.NewGuid().ToString("N")[..10]);
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", email, "+905551112233"));
            if (active)
            {
                employer.SetAsActive();
            }
            else
            {
                employer.SetAsSuspended();
            }

            employer.SetCommissionRate(commissionRate);
            EmployerLocation location = employer.AddLocation("HQ");
            location.SetAddress(new Address("Depo Sok. 2", "Istanbul", "TR"));
            location.SetCoordinate(new GeoCoordinate(41.015137, 28.97953));
            location.SetGeofenceRadiusMetres(1000);

            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();
            return employer;
        }

        private static async Task AddAcceptedPostingAsync(
            AdaIsAkademiDbContext db,
            Employer employer,
            JobCategory category,
            Worker worker,
            decimal wageAmount)
        {
            EmployerLocation location = employer.Locations.First();
            JobPosting posting = employer.AddJobPosting(
                location.Id,
                category.Id,
                $"Posting-{Guid.NewGuid():N}",
                "Aciklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(wageAmount, "TRY"),
                headCount: 1);
            posting.Publish();
            JobApplication app = posting.AddApplication(worker.Id, false, null);
            posting.AcceptApplication(app.Id);
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
