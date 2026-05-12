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
    /// Tests worker payout lifecycle and commission audit logging flow.
    /// </summary>
    public sealed class Sprint3WorkerPayoutCommandHandlersTests
    {
        #region Methods

        [Fact]
        public async Task CreateWorkerPayout_is_idempotent_and_writes_audit_log()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (int employerId, int workerId, int assignmentId) = await SeedCheckedOutAssignmentAsync(db);
            executionContext.ReplaceClaim("employer_id", employerId.ToString());

            var createHandler = new CreateWorkerPayoutCommandHandler(sp);
            WorkerPayoutSnapshotModel first = await ((IRequestHandler<CreateWorkerPayoutCommand, WorkerPayoutSnapshotModel>)createHandler).HandleAsync(
                new CreateWorkerPayoutCommand { AssignmentId = assignmentId },
                CancellationToken.None);
            WorkerPayoutSnapshotModel second = await ((IRequestHandler<CreateWorkerPayoutCommand, WorkerPayoutSnapshotModel>)createHandler).HandleAsync(
                new CreateWorkerPayoutCommand { AssignmentId = assignmentId },
                CancellationToken.None);

            int firstId = first.WorkerPayoutId;
            firstId.Should().Be(second.WorkerPayoutId);
            WorkerPayout payout = await db.Set<WorkerPayout>().AsNoTracking().FirstAsync(x => x.Id == firstId);
            payout.WorkerId.Should().Be(workerId);
            payout.Status.Should().Be(WorkerPayoutStatus.Pending);

            int auditCount = await db.Set<CommissionAuditLog>()
                .AsNoTracking()
                .CountAsync(x => x.AssignmentId == assignmentId && x.EventType == CommissionAuditEventType.WorkerPayoutCreated);
            auditCount.Should().Be(1);
        }

        [Fact]
        public async Task WorkerPayout_lifecycle_transitions_to_paid_with_audit_rows()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (int employerId, int workerId, int assignmentId) = await SeedCheckedOutAssignmentAsync(db);

            executionContext.ReplaceClaim("employer_id", employerId.ToString());
            var createHandler = new CreateWorkerPayoutCommandHandler(sp);
            WorkerPayoutSnapshotModel created = await ((IRequestHandler<CreateWorkerPayoutCommand, WorkerPayoutSnapshotModel>)createHandler).HandleAsync(
                new CreateWorkerPayoutCommand { AssignmentId = assignmentId },
                CancellationToken.None);
            int payoutId = created.WorkerPayoutId;

            var markHandler = new MarkWorkerPayoutAsProcessingCommandHandler(sp);
            await ((IRequestHandler<MarkWorkerPayoutAsProcessingCommand, WorkerPayoutSnapshotModel>)markHandler).HandleAsync(
                new MarkWorkerPayoutAsProcessingCommand { WorkerPayoutId = payoutId },
                CancellationToken.None);

            executionContext.ReplaceClaim("worker_id", workerId.ToString());
            var confirmHandler = new ConfirmWorkerPayoutCommandHandler(sp);
            await ((IRequestHandler<ConfirmWorkerPayoutCommand, WorkerPayoutSnapshotModel>)confirmHandler).HandleAsync(
                new ConfirmWorkerPayoutCommand { WorkerPayoutId = payoutId },
                CancellationToken.None);

            WorkerPayout payout = await db.Set<WorkerPayout>().AsNoTracking().FirstAsync(x => x.Id == payoutId);
            payout.Status.Should().Be(WorkerPayoutStatus.Paid);
            payout.PaidAt.Should().NotBeNull();

            List<CommissionAuditEventType> eventTypes = await db.Set<CommissionAuditLog>()
                .AsNoTracking()
                .Where(x => x.WorkerPayoutId == payoutId
                    || (x.AssignmentId == assignmentId && x.EventType == CommissionAuditEventType.WorkerPayoutCreated))
                .OrderBy(x => x.Id)
                .Select(x => x.EventType)
                .ToListAsync();

            eventTypes.Should().Contain(CommissionAuditEventType.WorkerPayoutCreated);
            eventTypes.Should().Contain(CommissionAuditEventType.WorkerPayoutMarkedAsPaid);
            eventTypes.Should().Contain(CommissionAuditEventType.WorkerPayoutConfirmed);
        }

        [Fact]
        public async Task WorkerPayout_fail_and_retry_flow_respects_retry_counter()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (int employerId, _, int assignmentId) = await SeedCheckedOutAssignmentAsync(db);
            executionContext.ReplaceClaim("employer_id", employerId.ToString());

            var createHandler = new CreateWorkerPayoutCommandHandler(sp);
            WorkerPayoutSnapshotModel created = await ((IRequestHandler<CreateWorkerPayoutCommand, WorkerPayoutSnapshotModel>)createHandler).HandleAsync(
                new CreateWorkerPayoutCommand { AssignmentId = assignmentId },
                CancellationToken.None);
            int payoutId = created.WorkerPayoutId;

            var markHandler = new MarkWorkerPayoutAsProcessingCommandHandler(sp);
            await ((IRequestHandler<MarkWorkerPayoutAsProcessingCommand, WorkerPayoutSnapshotModel>)markHandler).HandleAsync(
                new MarkWorkerPayoutAsProcessingCommand { WorkerPayoutId = payoutId },
                CancellationToken.None);

            var failHandler = new FailWorkerPayoutCommandHandler(sp);
            await ((IRequestHandler<FailWorkerPayoutCommand, WorkerPayoutSnapshotModel>)failHandler).HandleAsync(
                new FailWorkerPayoutCommand { WorkerPayoutId = payoutId, Reason = "bank_timeout" },
                CancellationToken.None);

            var retryHandler = new RetryWorkerPayoutCommandHandler(sp);
            await ((IRequestHandler<RetryWorkerPayoutCommand, WorkerPayoutSnapshotModel>)retryHandler).HandleAsync(
                new RetryWorkerPayoutCommand { WorkerPayoutId = payoutId },
                CancellationToken.None);

            WorkerPayout payout = await db.Set<WorkerPayout>().AsNoTracking().FirstAsync(x => x.Id == payoutId);
            payout.Status.Should().Be(WorkerPayoutStatus.Pending);
            payout.RetryCount.Should().Be(1);
        }

        [Fact]
        public async Task ConfirmWorkerPayout_throws_for_non_owner_worker()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (int employerId, _, int assignmentId) = await SeedCheckedOutAssignmentAsync(db);
            executionContext.ReplaceClaim("employer_id", employerId.ToString());

            var createHandler = new CreateWorkerPayoutCommandHandler(sp);
            WorkerPayoutSnapshotModel created = await ((IRequestHandler<CreateWorkerPayoutCommand, WorkerPayoutSnapshotModel>)createHandler).HandleAsync(
                new CreateWorkerPayoutCommand { AssignmentId = assignmentId },
                CancellationToken.None);
            int payoutId = created.WorkerPayoutId;

            var markHandler = new MarkWorkerPayoutAsProcessingCommandHandler(sp);
            await ((IRequestHandler<MarkWorkerPayoutAsProcessingCommand, WorkerPayoutSnapshotModel>)markHandler).HandleAsync(
                new MarkWorkerPayoutAsProcessingCommand { WorkerPayoutId = payoutId },
                CancellationToken.None);

            executionContext.ReplaceClaim("worker_id", "777777");
            var confirmHandler = new ConfirmWorkerPayoutCommandHandler(sp);
            Func<Task> act = async () =>
                await ((IRequestHandler<ConfirmWorkerPayoutCommand, WorkerPayoutSnapshotModel>)confirmHandler).HandleAsync(
                    new ConfirmWorkerPayoutCommand { WorkerPayoutId = payoutId },
                    CancellationToken.None);

            AzoxiaException ex = (await act.Should().ThrowAsync<AzoxiaException>()).Which;
            ex.Error.Should().Be(ApplicationValidationCodes.ActorResourceAccessDenied);
        }

        #endregion Methods

        #region Utils

        private static async Task<(int employerId, int workerId, int assignmentId)> SeedCheckedOutAssignmentAsync(AdaIsAkademiDbContext db)
        {
            var category = new JobCategory("Retail", null);
            db.Set<JobCategory>().Add(category);
            await db.SaveChangesAsync();

            var employer = new Employer("ACME", null, "1234567890");
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", "payout-seed@test.local", "+905551112233"));
            EmployerLocation hq = employer.AddLocation("HQ");
            hq.SetAddress(new Address("Depo Sok. 2", "Istanbul", "TR"));
            hq.SetCoordinate(new GeoCoordinate(41.015137, 28.97953));
            hq.SetGeofenceRadiusMetres(1000);
            employer.SetAsActive();
            employer.SetCommissionRate(0.2m);
            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();

            var user = new SystemUser("worker-payout@test.local", "Password1!", SystemUserType.Worker);
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

            var assignment = new ShiftAssignment(
                posting.Id,
                application.Id,
                worker.Id,
                "qr-token-hash",
                "supervisor-qr-token-hash");
            db.Set<ShiftAssignment>().Add(assignment);
            await db.SaveChangesAsync();

            assignment.CheckIn("qr-token-hash");
            assignment.SupervisorCheckIn("supervisor-qr-token-hash");
            assignment.CheckOut();
            await db.SaveChangesAsync();

            return (employer.Id, worker.Id, assignment.Id);
        }

        #endregion Utils
    }
}
