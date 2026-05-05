namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application.Tests.Support;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.AdaIsAkademi.Persistence;
    using Azoxia.Core.Application;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.ValueTypes;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Sprint 7 tests for commission receivable generation and query.
    /// </summary>
    public sealed class Sprint7CommissionReceivableTests
    {
        #region Methods

        [Fact]
        public async Task Generate_commission_receivable_should_be_idempotent_for_same_period()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            Employer employer = await SeedEmployerAsync(db);

            var commandHandler = new GenerateCommissionReceivableCommandHandler(sp);
            GenerateCommissionReceivableCommand command = new()
            {
                EmployerId = employer.Id,
                Amount = 150m,
                Currency = "TRY",
                PeriodStart = new DateOnly(2026, 5, 1),
                PeriodEnd = new DateOnly(2026, 5, 31),
            };

            int firstId = await ((IRequestHandler<GenerateCommissionReceivableCommand, int>)commandHandler)
                .HandleAsync(command, CancellationToken.None);
            int secondId = await ((IRequestHandler<GenerateCommissionReceivableCommand, int>)commandHandler)
                .HandleAsync(command, CancellationToken.None);

            firstId.Should().Be(secondId);
            int rowCount = await db.Set<CommissionReceivable>().CountAsync();
            rowCount.Should().Be(1);

            var queryHandler = new GetCommissionReceivableByPeriodQueryHandler(sp);
            CommissionReceivableDetailModel detail =
                await ((IRequestHandler<GetCommissionReceivableByPeriodQuery, CommissionReceivableDetailModel>)queryHandler)
                    .HandleAsync(
                        new GetCommissionReceivableByPeriodQuery
                        {
                            EmployerId = employer.Id,
                            PeriodStart = new DateOnly(2026, 5, 1),
                            PeriodEnd = new DateOnly(2026, 5, 31),
                        },
                        CancellationToken.None);

            detail.Id.Should().Be(firstId);
            detail.Amount.Should().Be(150m);
            detail.Currency.Should().Be("TRY");

            int auditCount = await db.Set<CommissionAuditLog>()
                .CountAsync(x => x.CommissionReceivableId == firstId
                                 && x.EventType == CommissionAuditEventType.CommissionReceivableGenerated);
            auditCount.Should().Be(1);
        }

        [Fact]
        public async Task Generate_commission_receivable_should_fail_for_non_active_employer()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            Employer employer = await SeedEmployerAsync(db);
            employer.SetAsSuspended();
            await db.SaveChangesAsync();

            var commandHandler = new GenerateCommissionReceivableCommandHandler(sp);
            Func<Task> act = async () =>
                await ((IRequestHandler<GenerateCommissionReceivableCommand, int>)commandHandler)
                    .HandleAsync(
                        new GenerateCommissionReceivableCommand
                        {
                            EmployerId = employer.Id,
                            Amount = 150m,
                            Currency = "TRY",
                            PeriodStart = new DateOnly(2026, 5, 1),
                            PeriodEnd = new DateOnly(2026, 5, 31),
                        },
                        CancellationToken.None);

            AzoxiaException ex = (await act.Should().ThrowAsync<AzoxiaException>()).Which;
            ex.Error.Should().Be(DomainErrorCodes.CommissionReceivableEmployerNotActive);
        }

        #endregion Methods

        #region Utils

        private static async Task<Employer> SeedEmployerAsync(AdaIsAkademiDbContext db)
        {
            Employer employer = new("ACME", null, "1234567890");
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", "receivable@test.local", "+905551112233"));
            employer.SetAsActive();

            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();
            return employer;
        }

        #endregion Utils
    }
}
