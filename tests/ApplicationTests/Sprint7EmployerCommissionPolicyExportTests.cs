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
    /// Sprint 7 tests for employer commission policy export query.
    /// </summary>
    public sealed class Sprint7EmployerCommissionPolicyExportTests
    {
        #region Methods

        [Fact]
        public async Task Export_commission_policies_should_return_csv_package()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            Employer employer1 = await SeedEmployerAsync(db, "ACME", "acme@test.local");
            Employer employer2 = await SeedEmployerAsync(db, "BETA", "beta@test.local");

            var setHandler = new SetEmployerCommissionRateCommandHandler(sp);
            await ((IRequestHandler<SetEmployerCommissionRateCommand, Unit>)setHandler)
                .HandleAsync(new SetEmployerCommissionRateCommand { EmployerId = employer1.Id, CommissionRate = 0.15m }, CancellationToken.None);
            await ((IRequestHandler<SetEmployerCommissionRateCommand, Unit>)setHandler)
                .HandleAsync(new SetEmployerCommissionRateCommand { EmployerId = employer2.Id, CommissionRate = 0.2m }, CancellationToken.None);

            var exportHandler = new ExportEmployerCommissionPoliciesCsvQueryHandler(sp);
            EmployerCommissionPolicyExportPackageModel result =
                await ((IRequestHandler<ExportEmployerCommissionPoliciesCsvQuery, EmployerCommissionPolicyExportPackageModel>)exportHandler)
                    .HandleAsync(new ExportEmployerCommissionPoliciesCsvQuery(), CancellationToken.None);

            result.ContentType.Should().Be("text/csv");
            result.RowCount.Should().BeGreaterThanOrEqualTo(2);
            result.CsvContent.Should().Contain("employer_id,employer_name,employer_status,commission_rate");
            result.CsvContent.Should().Contain("ACME");
            result.CsvContent.Should().Contain("BETA");
        }

        #endregion Methods

        #region Utils

        private static async Task<Employer> SeedEmployerAsync(AdaIsAkademiDbContext db, string name, string email)
        {
            Employer employer = new(name, null, Guid.NewGuid().ToString("N")[..10]);
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", email, "+905551112233"));
            employer.SetAsActive();

            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();
            return employer;
        }

        #endregion Utils
    }
}
