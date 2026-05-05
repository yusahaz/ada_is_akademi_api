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
    /// Sprint 7 tests for commission receivable list query.
    /// </summary>
    public sealed class Sprint7CommissionReceivableListQueryTests
    {
        #region Methods

        [Fact]
        public async Task List_receivables_should_return_latest_periods_first()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            Employer employer = await SeedEmployerAsync(db);
            var commandHandler = new GenerateCommissionReceivableCommandHandler(sp);

            await ((IRequestHandler<GenerateCommissionReceivableCommand, int>)commandHandler).HandleAsync(
                new GenerateCommissionReceivableCommand
                {
                    EmployerId = employer.Id,
                    Amount = 100m,
                    Currency = "TRY",
                    PeriodStart = new DateOnly(2026, 4, 1),
                    PeriodEnd = new DateOnly(2026, 4, 30),
                },
                CancellationToken.None);

            await ((IRequestHandler<GenerateCommissionReceivableCommand, int>)commandHandler).HandleAsync(
                new GenerateCommissionReceivableCommand
                {
                    EmployerId = employer.Id,
                    Amount = 200m,
                    Currency = "TRY",
                    PeriodStart = new DateOnly(2026, 5, 1),
                    PeriodEnd = new DateOnly(2026, 5, 31),
                },
                CancellationToken.None);

            var queryHandler = new ListCommissionReceivablesByEmployerQueryHandler(sp);
            IReadOnlyList<CommissionReceivableListItemModel> rows =
                await ((IRequestHandler<ListCommissionReceivablesByEmployerQuery, IReadOnlyList<CommissionReceivableListItemModel>>)queryHandler)
                    .HandleAsync(
                        new ListCommissionReceivablesByEmployerQuery
                        {
                            EmployerId = employer.Id,
                            Limit = 10,
                        },
                        CancellationToken.None);

            rows.Should().HaveCount(2);
            rows[0].Amount.Should().Be(200m);
            rows[0].PeriodStart.Should().Be(new DateOnly(2026, 5, 1));
            rows[1].Amount.Should().Be(100m);
        }

        [Fact]
        public void List_receivables_validator_should_fail_for_invalid_payload()
        {
            var validator = new ListCommissionReceivablesByEmployerQueryValidator();
            ValidationResult result = validator.Validate(
                new ListCommissionReceivablesByEmployerQuery
                {
                    EmployerId = 0,
                    Limit = 0,
                });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
        }

        #endregion Methods

        #region Utils

        private static async Task<Employer> SeedEmployerAsync(AdaIsAkademiDbContext db)
        {
            Employer employer = new("ACME", null, "1234567890");
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", "receivable-list@test.local", "+905551112233"));
            employer.SetAsActive();

            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();
            return employer;
        }

        #endregion Utils
    }
}
