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
    /// Sprint 7 tests for employer commission policy command/query.
    /// </summary>
    public sealed class Sprint7EmployerCommissionPolicyTests
    {
        #region Methods

        [Fact]
        public async Task Set_and_get_commission_policy_should_return_updated_rate()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            Employer employer = await SeedEmployerAsync(db);

            var setHandler = new SetEmployerCommissionRateCommandHandler(sp);
            await ((IRequestHandler<SetEmployerCommissionRateCommand, Unit>)setHandler)
                .HandleAsync(
                    new SetEmployerCommissionRateCommand
                    {
                        EmployerId = employer.Id,
                        CommissionRate = 0.15m,
                    },
                    CancellationToken.None);

            var getHandler = new GetEmployerCommissionPolicyQueryHandler(sp);
            EmployerCommissionPolicyModel model =
                await ((IRequestHandler<GetEmployerCommissionPolicyQuery, EmployerCommissionPolicyModel>)getHandler)
                    .HandleAsync(
                        new GetEmployerCommissionPolicyQuery
                        {
                            EmployerId = employer.Id,
                        },
                        CancellationToken.None);

            model.EmployerId.Should().Be(employer.Id);
            model.CommissionRate.Should().Be(0.15m);
        }

        #endregion Methods

        #region Utils

        private static async Task<Employer> SeedEmployerAsync(AdaIsAkademiDbContext db)
        {
            Employer employer = new("ACME", null, "1234567890");
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", "commission@test.local", "+905551112233"));
            employer.SetAsActive();

            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();

            return employer;
        }

        #endregion Utils
    }
}
