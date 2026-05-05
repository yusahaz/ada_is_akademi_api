namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application.Tests.Support;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.AdaIsAkademi.Persistence;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.ValueTypes;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Handler tests for Sprint 3 shift assignment bootstrap flow.
    /// </summary>
    public sealed class Sprint3AssignmentCommandHandlersTests
    {
        #region Methods

        [Fact]
        public async Task CreateShiftAssignmentHandler_creates_assignment_for_accepted_application()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (int employerId, _, int applicationId, int workerId) = await SeedAcceptedApplicationAsync(db);
            executionContext.ReplaceClaim("employer_id", employerId.ToString());

            var createHandler = new CreateShiftAssignmentCommandHandler(sp);
            int assignmentId = await ((IRequestHandler<CreateShiftAssignmentCommand, int>)createHandler).HandleAsync(
                new CreateShiftAssignmentCommand
                {
                    CheckInTokenHash = "qr-token-hash",
                    JobApplicationId = applicationId,
                },
                CancellationToken.None);

            assignmentId.Should().BeGreaterThan(0);
            ShiftAssignment? assignment = await db.Set<ShiftAssignment>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == assignmentId);
            assignment.Should().NotBeNull();
            assignment!.WorkerId.Should().Be(workerId);
            assignment.Status.Should().Be(ShiftAssignmentStatus.Pending);
        }

        [Fact]
        public async Task CheckInShiftAssignmentHandler_marks_assignment_checked_in_for_owner_worker()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (int employerId, int assignmentId, int workerId) = await SeedAssignmentAsync(db);
            executionContext.ReplaceClaim("employer_id", employerId.ToString());

            var checkInHandler = new CheckInShiftAssignmentCommandHandler(sp);
            executionContext.ReplaceClaim("worker_id", workerId.ToString());
            await ((IRequestHandler<CheckInShiftAssignmentCommand, Unit>)checkInHandler).HandleAsync(
                new CheckInShiftAssignmentCommand
                {
                    AssignmentId = assignmentId,
                    CheckInTokenHash = "qr-token-hash",
                },
                CancellationToken.None);

            ShiftAssignment? assignment = await db.Set<ShiftAssignment>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == assignmentId);
            assignment.Should().NotBeNull();
            assignment!.Status.Should().Be(ShiftAssignmentStatus.CheckedIn);
            assignment.CheckedInAt.Should().NotBeNull();
        }

        [Fact]
        public async Task CheckOutShiftAssignmentHandler_marks_assignment_checked_out_after_check_in()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (_, int assignmentId, int workerId) = await SeedAssignmentAsync(db);
            executionContext.ReplaceClaim("worker_id", workerId.ToString());

            var checkInHandler = new CheckInShiftAssignmentCommandHandler(sp);
            await ((IRequestHandler<CheckInShiftAssignmentCommand, Unit>)checkInHandler).HandleAsync(
                new CheckInShiftAssignmentCommand
                {
                    AssignmentId = assignmentId,
                    CheckInTokenHash = "qr-token-hash",
                },
                CancellationToken.None);

            var checkOutHandler = new CheckOutShiftAssignmentCommandHandler(sp);
            await ((IRequestHandler<CheckOutShiftAssignmentCommand, Unit>)checkOutHandler).HandleAsync(
                new CheckOutShiftAssignmentCommand
                {
                    AssignmentId = assignmentId,
                },
                CancellationToken.None);

            ShiftAssignment? assignment = await db.Set<ShiftAssignment>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == assignmentId);
            assignment.Should().NotBeNull();
            assignment!.Status.Should().Be(ShiftAssignmentStatus.CheckedOut);
            assignment.CheckedOutAt.Should().NotBeNull();
            assignment.IsAnomalyFlagged.Should().BeTrue();
            assignment.AnomalyCode.Should().Be("EARLY_CHECKOUT");
        }

        [Fact]
        public async Task CheckInShiftAssignmentHandler_throws_for_non_owner_worker()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (_, int assignmentId, _) = await SeedAssignmentAsync(db);
            executionContext.ReplaceClaim("worker_id", "999999");

            var checkInHandler = new CheckInShiftAssignmentCommandHandler(sp);
            Func<Task> act = async () =>
                await ((IRequestHandler<CheckInShiftAssignmentCommand, Unit>)checkInHandler).HandleAsync(
                    new CheckInShiftAssignmentCommand
                    {
                        AssignmentId = assignmentId,
                        CheckInTokenHash = "qr-token-hash",
                    },
                    CancellationToken.None);

            AzoxiaException ex = (await act.Should().ThrowAsync<AzoxiaException>()).Which;
            ex.Error.Should().Be(ApplicationValidationCodes.ActorResourceAccessDenied);
        }

        #endregion Methods

        #region Utils

        private static async Task<(int employerId, int assignmentId, int workerId)> SeedAssignmentAsync(AdaIsAkademiDbContext db)
        {
            (int employerId, int postingId, int applicationId, int workerId) = await SeedAcceptedApplicationAsync(db);

            var assignment = new ShiftAssignment(
                postingId,
                applicationId,
                workerId,
                "qr-token-hash");
            db.Set<ShiftAssignment>().Add(assignment);
            await db.SaveChangesAsync();

            return (employerId, assignment.Id, workerId);
        }

        private static async Task<(int employerId, int postingId, int applicationId, int workerId)> SeedAcceptedApplicationAsync(
            AdaIsAkademiDbContext db)
        {
            var category = new JobCategory("Retail", null);
            db.Set<JobCategory>().Add(category);
            await db.SaveChangesAsync();

            var employer = new Employer("ACME", null, "1234567890");
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", "employer-seed@test.local", "+905551112233"));
            EmployerLocation hq = employer.AddLocation("HQ");
            hq.SetAddress(new Address("Depo Sok. 2", "Istanbul", "TR"));
            hq.SetCoordinate(new GeoCoordinate(41.015137, 28.97953));
            hq.SetGeofenceRadiusMetres(1000);
            employer.SetAsActive();
            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();

            var user = new SystemUser("worker-sprint3@test.local", "Password1!", SystemUserType.Worker);
            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();

            var worker = new Worker(user.Id);
            db.Set<Worker>().Add(worker);
            await db.SaveChangesAsync();

            JobPosting posting = employer.AddJobPosting(
                hq.Id,
                category.Id,
                "Kasiyer",
                "Açıklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                headCount: 2);
            posting.Publish();
            JobApplication application = posting.AddApplication(worker.Id, hasConflictingShift: false, note: null);
            posting.AcceptApplication(application.Id);
            await db.SaveChangesAsync();

            return (employer.Id, posting.Id, application.Id, worker.Id);
        }

        #endregion Utils
    }
}
