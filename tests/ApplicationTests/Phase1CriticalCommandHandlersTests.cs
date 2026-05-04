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
    /// In-memory handler flows for Faz 1 MVP (ilan + başvuru + email doğrulama).
    /// </summary>
    public sealed class Phase1CriticalCommandHandlersTests
    {
        #region Methods

        [Fact]
        public async Task AcceptJobPostingApplicationHandler_accepts_pending_application_when_employer_matches()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (int employerId, _, _, int postingId, _, int applicationId) =
                await SeedOpenPostingWithOneApplicationAsync(db);
            executionContext.ReplaceClaim("employer_id", employerId.ToString());

            var handler = new AcceptJobPostingApplicationCommandHandler(sp);
            await ((IRequestHandler<AcceptJobPostingApplicationCommand, Unit>)handler).HandleAsync(
                new AcceptJobPostingApplicationCommand
                {
                    ApplicationId = applicationId,
                    JobPostingId = postingId,
                },
                CancellationToken.None);

            JobApplication? app = await db.Set<JobApplication>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == applicationId);
            app.Should().NotBeNull();
            app!.Status.Should().Be(JobApplicationStatus.Accepted);
        }

        [Fact]
        public async Task CreateJobPostingHandler_persists_draft_posting()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (int employerId, int locationId, int categoryId) = await SeedActiveEmployerAndCategoryAsync(db);
            executionContext.ReplaceClaim("employer_id", employerId.ToString());

            DateOnly shiftDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
            var command = new CreateJobPostingCommand
            {
                Description = "Vardiya açıklaması",
                EmployerLocationId = locationId,
                HeadCount = 2,
                JobCategoryId = categoryId,
                ShiftDate = shiftDate,
                ShiftEndTime = new TimeOnly(18, 0),
                ShiftStartTime = new TimeOnly(9, 0),
                Title = "Kasiyer",
                WageAmount = 100m,
                WageCurrency = "TRY",
            };

            var handler = new CreateJobPostingCommandHandler(sp);
            int postingId = await ((IRequestHandler<CreateJobPostingCommand, int>)handler).HandleAsync(
                command,
                CancellationToken.None);

            postingId.Should().BeGreaterThan(0);
            JobPosting? posting = await db.Set<JobPosting>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == postingId);
            posting.Should().NotBeNull();
            posting!.Status.Should().Be(JobPostingStatus.Draft);
            posting.EmployerId.Should().Be(employerId);
        }

        [Fact]
        public async Task ListJobApplicationsByJobPostingIdHandler_returns_rows_after_submit()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (int employerId, _, _, int postingId, int workerId, _) =
                await SeedOpenPostingWithOneApplicationAsync(db);
            executionContext.ReplaceClaim("employer_id", employerId.ToString());

            var listHandler = new ListJobApplicationsByJobPostingIdQueryHandler(sp);
            IReadOnlyList<JobApplicationListItemModel> list =
                await ((IRequestHandler<ListJobApplicationsByJobPostingIdQuery, IReadOnlyList<JobApplicationListItemModel>>)listHandler)
                    .HandleAsync(
                        new ListJobApplicationsByJobPostingIdQuery
                        {
                            JobPostingId = postingId,
                        },
                        CancellationToken.None);

            list.Should().HaveCount(1);
            list[0].WorkerId.Should().Be(workerId);
        }

        [Fact]
        public async Task ListJobApplicationsByJobPostingIdHandler_throws_when_employer_mismatch()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (int employerId, _, _, int postingId, _, _) = await SeedOpenPostingWithOneApplicationAsync(db);
            executionContext.ReplaceClaim("employer_id", (employerId + 9999).ToString());

            var listHandler = new ListJobApplicationsByJobPostingIdQueryHandler(sp);
            Func<Task> act = async () =>
                await ((IRequestHandler<ListJobApplicationsByJobPostingIdQuery, IReadOnlyList<JobApplicationListItemModel>>)listHandler)
                    .HandleAsync(
                        new ListJobApplicationsByJobPostingIdQuery
                        {
                            JobPostingId = postingId,
                        },
                        CancellationToken.None);

            AzoxiaException ex = (await act.Should().ThrowAsync<AzoxiaException>()).Which;
            ex.Error.Should().Be(AzoxiaErrorCodes.NotFound);
        }

        [Fact]
        public async Task PublishJobPostingHandler_throws_when_employer_mismatch()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (int employerId, _, _, int postingId) = await SeedDraftPostingAsync(db);
            executionContext.ReplaceClaim("employer_id", (employerId + 9999).ToString());

            var handler = new PublishJobPostingCommandHandler(sp);
            Func<Task> act = async () =>
                await ((IRequestHandler<PublishJobPostingCommand, Unit>)handler).HandleAsync(
                    new PublishJobPostingCommand
                    {
                        JobPostingId = postingId,
                    },
                    CancellationToken.None);

            AzoxiaException ex = (await act.Should().ThrowAsync<AzoxiaException>()).Which;
            ex.Error.Should().Be(AzoxiaErrorCodes.NotFound);
        }

        [Fact]
        public async Task SubmitJobPostingApplicationHandler_second_submit_same_worker_returns_same_id()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (_, _, _, int postingId, int workerId, _) =
                await SeedOpenPostingWithOneApplicationAsync(db);
            executionContext.ReplaceClaim("worker_id", workerId.ToString());

            var submitHandler = new SubmitJobPostingApplicationCommandHandler(sp);
            var cmd = new SubmitJobPostingApplicationCommand
            {
                HasConflictingShift = false,
                JobPostingId = postingId,
                Note = null,
            };

            int first = await ((IRequestHandler<SubmitJobPostingApplicationCommand, int>)submitHandler).HandleAsync(
                cmd,
                CancellationToken.None);
            int second = await ((IRequestHandler<SubmitJobPostingApplicationCommand, int>)submitHandler).HandleAsync(
                cmd,
                CancellationToken.None);

            second.Should().Be(first);
            (await db.Set<JobApplication>().CountAsync(x => x.JobPostingId == postingId)).Should().Be(1);
        }

        [Fact]
        public async Task WithdrawJobPostingApplicationHandler_marks_pending_application_as_withdrawn_for_owner_worker()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            (_, _, _, int postingId, int workerId, int applicationId) =
                await SeedOpenPostingWithOneApplicationAsync(db);
            executionContext.ReplaceClaim("worker_id", workerId.ToString());

            var withdrawHandler = new WithdrawJobPostingApplicationCommandHandler(sp);
            await ((IRequestHandler<WithdrawJobPostingApplicationCommand, Unit>)withdrawHandler).HandleAsync(
                new WithdrawJobPostingApplicationCommand
                {
                    ApplicationId = applicationId,
                    JobPostingId = postingId,
                },
                CancellationToken.None);

            JobApplication? app = await db.Set<JobApplication>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == applicationId);
            app.Should().NotBeNull();
            app!.Status.Should().Be(JobApplicationStatus.Withdrawn);
        }

        [Fact]
        public async Task VerifySystemUserEmailHandler_activates_user_when_token_matches()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            var user = new SystemUser("verify-handler@test.local", "Password1!", SystemUserType.Worker);
            user.RequestEmailVerification("plain-token", DateTimeOffset.UtcNow.AddHours(1));
            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();

            var handler = new VerifySystemUserEmailCommandHandler(sp);
            await ((IRequestHandler<VerifySystemUserEmailCommand, Unit>)handler).HandleAsync(
                new VerifySystemUserEmailCommand { SystemUserId = user.Id, TokenHash = "plain-token" },
                CancellationToken.None);

            SystemUser? reloaded = await db.Set<SystemUser>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);
            reloaded.Should().NotBeNull();
            reloaded!.AccountStatus.Should().Be(AccountStatus.Active);
            reloaded.EmailVerificationToken.Should().BeNull();
        }

        [Fact]
        public async Task RefreshSystemUserTokenHandler_throws_when_user_is_not_active()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            var user = new SystemUser("refresh-suspended@test.local", "Password1!", SystemUserType.Worker);
            user.RequestEmailVerification("verify-token", DateTimeOffset.UtcNow.AddHours(1));
            user.VerifyEmail("verify-token");
            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();

            SystemUserDevice device = user.AddOrUpdateDevice("device-1", DevicePlatform.Android, "push-1");
            await db.SaveChangesAsync();
            user.IssueRefreshToken("refresh-token-1", device.Id, DateTimeOffset.UtcNow.AddHours(1));
            user.Suspend();
            await db.SaveChangesAsync();

            var handler = new RefreshSystemUserTokenCommandHandler(sp);
            Func<Task> act = async () =>
                await ((IRequestHandler<RefreshSystemUserTokenCommand, SystemUserTokenModel>)handler).HandleAsync(
                    new RefreshSystemUserTokenCommand
                    {
                        SystemUserId = user.Id,
                        DeviceIdentifier = "device-1",
                        RefreshToken = "refresh-token-1",
                    },
                    CancellationToken.None);

            AzoxiaException ex = (await act.Should().ThrowAsync<AzoxiaException>()).Which;
            ex.Error.Should().Be(AzoxiaErrorCodes.NotFound);
        }

        #endregion Methods

        #region Utils

        private static async Task<(int employerId, int locationId, int categoryId)> SeedActiveEmployerAndCategoryAsync(
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

            // Avoid `.Include(x => x.Locations)` on Employer with InMemory + complex Address (EF shaper KeyNotFound).
            EmployerLocation firstLocation = employer.Locations[0];
            return (employer.Id, firstLocation.Id, category.Id);
        }

        private static async Task<(int employerId, int locationId, int categoryId, int postingId)> SeedDraftPostingAsync(
            AdaIsAkademiDbContext db)
        {
            (int employerId, int locationId, int categoryId) = await SeedActiveEmployerAndCategoryAsync(db);
            Employer employerEntity = await db.Set<Employer>().FirstAsync(x => x.Id == employerId);
            await db.Entry(employerEntity).Collection(x => x.Locations).LoadAsync(CancellationToken.None);

            JobPosting posting = employerEntity.AddJobPosting(
                locationId,
                categoryId,
                "Kasiyer",
                "Taslak ilan",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                headCount: 2);
            await db.SaveChangesAsync();

            return (employerId, locationId, categoryId, posting.Id);
        }

        private static async Task<(int employerId, int locationId, int categoryId, int postingId, int workerId, int applicationId)>
            SeedOpenPostingWithOneApplicationAsync(AdaIsAkademiDbContext db)
        {
            (int employerId, int locationId, int categoryId) = await SeedActiveEmployerAndCategoryAsync(db);

            var user = new SystemUser("worker-seed@test.local", "Password1!", SystemUserType.Worker);
            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();

            var worker = new Worker(user.Id);
            db.Set<Worker>().Add(worker);
            await db.SaveChangesAsync();

            Employer employerEntity = await db.Set<Employer>().FirstAsync(x => x.Id == employerId);
            await db.Entry(employerEntity).Collection(x => x.Locations).LoadAsync(CancellationToken.None);

            JobPosting posting = employerEntity.AddJobPosting(
                locationId,
                categoryId,
                "Kasiyer",
                "Açıklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                headCount: 2);
            posting.Publish();
            await db.SaveChangesAsync();

            JobApplication application = posting.AddApplication(worker.Id, hasConflictingShift: false, note: null);
            await db.SaveChangesAsync();

            return (employerId, locationId, categoryId, posting.Id, worker.Id, application.Id);
        }

        #endregion Utils
    }
}
