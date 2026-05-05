namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application.Tests.Support;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.AdaIsAkademi.Persistence;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Azoxia.Core.ValueTypes;

    /// <summary>
    /// Handler tests for employer profile/location/supervisor lifecycle commands.
    /// </summary>
    public sealed class EmployerProfileCommandHandlersTests
    {
        #region Methods

        [Fact]
        public async Task AddEmployerLocationCommandHandler_adds_location_for_actor_employer()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            Employer employer = await SeedEmployerAsync(db);
            executionContext.ReplaceClaim("employer_id", employer.Id.ToString());

            var handler = new AddEmployerLocationCommandHandler(sp);
            int locationId = await ((IRequestHandler<AddEmployerLocationCommand, int>)handler).HandleAsync(
                new AddEmployerLocationCommand
                {
                    Name = "Kadikoy",
                    Description = "Istanbul center",
                    Line1 = "Rıhtım Cad. 1",
                    City = "Istanbul",
                    Country = "TR",
                    Latitude = 41.01d,
                    Longitude = 29.0d,
                    GeofenceRadiusMetres = 150,
                },
                CancellationToken.None);

            EmployerLocation? location = await db.Set<EmployerLocation>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == locationId);
            location.Should().NotBeNull();
            location!.EmployerId.Should().Be(employer.Id);
            location.Name.Should().Be("Kadikoy");
        }

        [Fact]
        public async Task AddAndRemoveEmployerSupervisor_handlers_manage_supervisor_state()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            Employer employer = await SeedEmployerAsync(db);
            executionContext.ReplaceClaim("employer_id", employer.Id.ToString());

            SystemUser supervisorUser = new("supervisor@test.local", "Password1!", SystemUserType.Employer);
            db.Set<SystemUser>().Add(supervisorUser);
            await db.SaveChangesAsync();

            EmployerLocation location = employer.AddLocation("Umraniye", "HQ");
            location.SetAddress(new Address("Site Sokak 1", "Istanbul", "TR"));
            location.SetCoordinate(new GeoCoordinate(41.02d, 29.12d));
            location.SetGeofenceRadiusMetres(120);
            await db.SaveChangesAsync();

            var addHandler = new AddEmployerSupervisorCommandHandler(sp);
            int supervisorId = await ((IRequestHandler<AddEmployerSupervisorCommand, int>)addHandler).HandleAsync(
                new AddEmployerSupervisorCommand
                {
                    SystemUserId = supervisorUser.Id,
                    LocationId = location.Id,
                },
                CancellationToken.None);

            ShiftSupervisor? added = await db.Set<ShiftSupervisor>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == supervisorId);
            added.Should().NotBeNull();
            added!.IsActive.Should().BeTrue();

            var removeHandler = new RemoveEmployerSupervisorCommandHandler(sp);
            await ((IRequestHandler<RemoveEmployerSupervisorCommand, Unit>)removeHandler).HandleAsync(
                new RemoveEmployerSupervisorCommand
                {
                    SystemUserId = supervisorUser.Id,
                },
                CancellationToken.None);

            ShiftSupervisor? removed = await db.Set<ShiftSupervisor>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == supervisorId);
            removed.Should().NotBeNull();
            removed!.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteEmployerCommandHandler_soft_deletes_employer_and_related_users()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            Employer employer = await SeedEmployerAsync(db);
            EmployerLocation location = employer.AddLocation("Besiktas", "Main");
            location.SetAddress(new Address("Barbaros Bulv. 1", "Istanbul", "TR"));
            location.SetCoordinate(new GeoCoordinate(41.04d, 29.0d));
            location.SetGeofenceRadiusMetres(100);
            await db.SaveChangesAsync();

            SystemUser supervisorUser = new("delete-supervisor@test.local", "Password1!", SystemUserType.Employer);
            SystemUser employerScopedUser = new("delete-employer-scoped@test.local", "Password1!", SystemUserType.Employer);
            SystemUser locationScopedUser = new("delete-location-scoped@test.local", "Password1!", SystemUserType.Employer);
            SystemUser unrelatedUser = new("delete-unrelated@test.local", "Password1!", SystemUserType.Worker);
            db.Set<SystemUser>().AddRange(supervisorUser, employerScopedUser, locationScopedUser, unrelatedUser);
            await db.SaveChangesAsync();

            employer.AddShiftSupervisor(supervisorUser.Id, location.Id);
            await db.SaveChangesAsync();

            SystemUserGroup group = new("Ops", null, false);
            db.Set<SystemUserGroup>().Add(group);
            await db.SaveChangesAsync();

            db.Set<SystemUserGroupMembership>().AddRange(
                new SystemUserGroupMembership(group.Id, employerScopedUser.Id, MembershipScopeType.EmployerScoped, employer.Id),
                new SystemUserGroupMembership(group.Id, locationScopedUser.Id, MembershipScopeType.LocationScoped, location.Id));
            await db.SaveChangesAsync();

            var handler = new DeleteEmployerCommandHandler(sp);
            await ((IRequestHandler<DeleteEmployerCommand, Unit>)handler).HandleAsync(
                new DeleteEmployerCommand
                {
                    EmployerId = employer.Id,
                },
                CancellationToken.None);

            Employer? employerReloaded = await db.Set<Employer>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == employer.Id);
            employerReloaded.Should().NotBeNull();
            employerReloaded!.IsDeleted.Should().BeTrue();

            SystemUser? supervisorReloaded = await db.Set<SystemUser>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == supervisorUser.Id);
            SystemUser? employerScopedReloaded = await db.Set<SystemUser>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == employerScopedUser.Id);
            SystemUser? locationScopedReloaded = await db.Set<SystemUser>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == locationScopedUser.Id);
            SystemUser? unrelatedReloaded = await db.Set<SystemUser>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == unrelatedUser.Id);

            supervisorReloaded!.IsDeleted.Should().BeTrue();
            employerScopedReloaded!.IsDeleted.Should().BeTrue();
            locationScopedReloaded!.IsDeleted.Should().BeTrue();
            unrelatedReloaded!.IsDeleted.Should().BeFalse();
        }

        #endregion Methods

        #region Utils

        private static async Task<Employer> SeedEmployerAsync(AdaIsAkademiDbContext db)
        {
            Employer employer = new("ACME", "desc", "1234567890");
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ada", "Employer", "employer@test.local", "+905551112233"));
            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();
            return employer;
        }

        #endregion Utils
    }
}
