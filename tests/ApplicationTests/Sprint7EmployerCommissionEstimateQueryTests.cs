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
    /// Sprint 7 tests for employer commission estimate query.
    /// </summary>
    public sealed class Sprint7EmployerCommissionEstimateQueryTests
    {
        #region Methods

        [Fact]
        public async Task Commission_estimate_query_should_calculate_gross_and_commission()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            Employer employer = await SeedEmployerWithAcceptedApplicationsAsync(db);

            var handler = new GetEmployerCommissionEstimateQueryHandler(sp);
            EmployerCommissionEstimateModel model =
                await ((IRequestHandler<GetEmployerCommissionEstimateQuery, EmployerCommissionEstimateModel>)handler)
                    .HandleAsync(
                        new GetEmployerCommissionEstimateQuery
                        {
                            EmployerId = employer.Id,
                        },
                        CancellationToken.None);

            model.EmployerId.Should().Be(employer.Id);
            model.CommissionRate.Should().Be(0.2m);
            model.AcceptedApplicationCount.Should().Be(2);
            model.EstimatedGrossTransactionVolume.Should().Be(250m);
            model.EstimatedCommissionAmount.Should().Be(50m);
        }

        #endregion Methods

        #region Utils

        private static async Task<Employer> SeedEmployerWithAcceptedApplicationsAsync(AdaIsAkademiDbContext db)
        {
            JobCategory category = new("Retail", null);
            db.Set<JobCategory>().Add(category);
            await db.SaveChangesAsync();

            Employer employer = new("ACME", null, "1234567890");
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", "estimate@test.local", "+905551112233"));
            employer.SetAsActive();
            employer.SetCommissionRate(0.2m);
            EmployerLocation location = employer.AddLocation("HQ");
            location.SetAddress(new Address("Depo Sok. 2", "Istanbul", "TR"));
            location.SetCoordinate(new GeoCoordinate(41.015137, 28.97953));
            location.SetGeofenceRadiusMetres(1000);

            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();

            Worker worker1 = await AddWorkerAsync(db, "worker-est-1@test.local");
            Worker worker2 = await AddWorkerAsync(db, "worker-est-2@test.local");

            JobPosting posting1 = employer.AddJobPosting(
                location.Id,
                category.Id,
                "Estimate Posting 1",
                "Aciklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(125m, "TRY"),
                headCount: 1);
            posting1.Publish();

            JobPosting posting2 = employer.AddJobPosting(
                location.Id,
                category.Id,
                "Estimate Posting 2",
                "Aciklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(125m, "TRY"),
                headCount: 1);
            posting2.Publish();

            JobApplication app1 = posting1.AddApplication(worker1.Id, false, null);
            posting1.AcceptApplication(app1.Id);
            JobApplication app2 = posting2.AddApplication(worker2.Id, false, null);
            posting2.AcceptApplication(app2.Id);

            await db.SaveChangesAsync();

            return employer;
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
