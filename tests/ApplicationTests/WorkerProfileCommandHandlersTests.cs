namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application.Tests.Support;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.AdaIsAkademi.Persistence;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Exceptions;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Handler tests for worker profile update command.
    /// </summary>
    public sealed class WorkerProfileCommandHandlersTests
    {
        #region Methods

        [Fact]
        public async Task UpdateWorkerProfileHandler_updates_nationality_and_university_for_actor_worker()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            Worker worker = await SeedWorkerAsync(db);
            executionContext.ReplaceClaim("worker_id", worker.Id.ToString());

            var handler = new UpdateWorkerProfileCommandHandler(sp);
            await ((IRequestHandler<UpdateWorkerProfileCommand, Unit>)handler).HandleAsync(
                new UpdateWorkerProfileCommand
                {
                    Nationality = "TR",
                    University = "Bogazici University",
                    Gender = WorkerGender.Male,
                },
                CancellationToken.None);

            Worker? reloaded = await db.Set<Worker>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == worker.Id);
            reloaded.Should().NotBeNull();
            reloaded!.Nationality.Should().Be("TR");
            reloaded.University.Should().Be("Bogazici University");
            reloaded.Gender.Should().Be(WorkerGender.Male);
        }

        [Fact]
        public async Task UpdateWorkerProfileHandler_updates_phone_on_linked_system_user()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            Worker worker = await SeedWorkerAsync(db);
            executionContext.ReplaceClaim("worker_id", worker.Id.ToString());

            var handler = new UpdateWorkerProfileCommandHandler(sp);
            await ((IRequestHandler<UpdateWorkerProfileCommand, Unit>)handler).HandleAsync(
                new UpdateWorkerProfileCommand
                {
                    Phone = "+905383760000",
                },
                CancellationToken.None);

            SystemUser? reloadedUser = await db.Set<SystemUser>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == worker.SystemUserId);
            reloadedUser.Should().NotBeNull();
            reloadedUser!.Phone.Should().Be("+905383760000");
        }

        [Fact]
        public async Task UpdateWorkerProfileHandler_throws_when_actor_worker_not_found()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            executionContext.ReplaceClaim("worker_id", "999999");

            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;

            var handler = new UpdateWorkerProfileCommandHandler(sp);
            Func<Task> act = async () =>
                await ((IRequestHandler<UpdateWorkerProfileCommand, Unit>)handler).HandleAsync(
                    new UpdateWorkerProfileCommand
                    {
                        Nationality = "TR",
                        University = "Any University",
                    },
                    CancellationToken.None);

            AzoxiaException ex = (await act.Should().ThrowAsync<AzoxiaException>()).Which;
            ex.Error.Should().Be(AzoxiaErrorCodes.NotFound);
        }

        [Fact]
        public async Task AddAndRemoveWorkerAvailability_handlers_manage_collection_for_actor_worker()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            Worker worker = await SeedWorkerAsync(db);
            executionContext.ReplaceClaim("worker_id", worker.Id.ToString());

            var addHandler = new AddWorkerAvailabilityCommandHandler(sp);
            int availabilityId = await ((IRequestHandler<AddWorkerAvailabilityCommand, int>)addHandler).HandleAsync(
                new AddWorkerAvailabilityCommand
                {
                    DayOfWeek = DayOfWeek.Monday,
                    TimeFrom = new TimeOnly(9, 0),
                    TimeTo = new TimeOnly(18, 0),
                },
                CancellationToken.None);

            WorkerAvailability? added = await db.Set<WorkerAvailability>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == availabilityId);
            added.Should().NotBeNull();
            added!.WorkerId.Should().Be(worker.Id);

            var removeHandler = new RemoveWorkerAvailabilityCommandHandler(sp);
            await ((IRequestHandler<RemoveWorkerAvailabilityCommand, Unit>)removeHandler).HandleAsync(
                new RemoveWorkerAvailabilityCommand
                {
                    AvailabilityId = availabilityId,
                },
                CancellationToken.None);

            Worker? reloaded = await db.Set<Worker>()
                .Include(x => x.Availabilities)
                .FirstOrDefaultAsync(x => x.Id == worker.Id);
            reloaded.Should().NotBeNull();
            reloaded!.Availabilities.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteWorkerCommandHandler_soft_deletes_worker_and_linked_system_user()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            Worker worker = await SeedWorkerAsync(db);

            var handler = new DeleteWorkerCommandHandler(sp);
            await ((IRequestHandler<DeleteWorkerCommand, Unit>)handler).HandleAsync(
                new DeleteWorkerCommand
                {
                    WorkerId = worker.Id,
                },
                CancellationToken.None);

            Worker? workerReloaded = await db.Set<Worker>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == worker.Id);
            SystemUser? userReloaded = await db.Set<SystemUser>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == worker.SystemUserId);

            workerReloaded.Should().NotBeNull();
            userReloaded.Should().NotBeNull();
            workerReloaded!.IsDeleted.Should().BeTrue();
            userReloaded!.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task ConfirmWorkerCvReviewHandler_is_idempotent_when_session_already_confirmed()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            (Worker worker, CvUploadSession session) = await SeedConfirmedCvReviewSessionAsync(db);
            executionContext.ReplaceClaim("worker_id", worker.Id.ToString());

            int educationCountBefore = await db.Set<WorkerEducation>()
                .AsNoTracking()
                .CountAsync(x => x.WorkerId == worker.Id);

            var handler = new ConfirmWorkerCvReviewCommandHandler(sp);
            await ((IRequestHandler<ConfirmWorkerCvReviewCommand, Unit>)handler).HandleAsync(
                new ConfirmWorkerCvReviewCommand
                {
                    CvUploadSessionId = session.Id,
                },
                CancellationToken.None);

            CvUploadSession? reloadedSession = await db.Set<CvUploadSession>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == session.Id);
            reloadedSession.Should().NotBeNull();
            reloadedSession!.Status.Should().Be(CvUploadSessionStatus.Confirmed);

            int educationCountAfter = await db.Set<WorkerEducation>()
                .AsNoTracking()
                .CountAsync(x => x.WorkerId == worker.Id);
            educationCountAfter.Should().Be(educationCountBefore);
        }

        #endregion Methods

        #region Utils

        private static async Task<Worker> SeedWorkerAsync(AdaIsAkademiDbContext db)
        {
            var user = new SystemUser("worker-profile@test.local", "Password1!", SystemUserType.Worker);
            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();

            Worker worker = new(user.Id);
            db.Set<Worker>().Add(worker);
            await db.SaveChangesAsync();
            return worker;
        }

        private static async Task<(Worker worker, CvUploadSession session)> SeedConfirmedCvReviewSessionAsync(AdaIsAkademiDbContext db)
        {
            Worker worker = await SeedWorkerAsync(db);

            var session = new CvUploadSession(
                worker.Id,
                $"workers/{worker.Id}/cv/sample.pdf",
                "sample.pdf",
                "application/pdf",
                1024,
                CvFileFormat.Pdf);
            session.MarkAsExtracting();
            session.MarkAsAwaitingReview(
                """
                {
                  "educations": [
                    {
                      "school": "Ada University",
                      "department": "Computer Engineering",
                      "educationType": "Bachelor",
                      "startYear": "2020",
                      "endYear": "2024",
                      "isOngoing": false
                    }
                  ]
                }
                """);
            session.Confirm();

            db.Set<CvUploadSession>().Add(session);
            await db.SaveChangesAsync();
            return (worker, session);
        }

        #endregion Utils
    }
}
