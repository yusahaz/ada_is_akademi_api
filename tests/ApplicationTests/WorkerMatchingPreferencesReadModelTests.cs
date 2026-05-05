namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application.Tests.Support;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.AdaIsAkademi.Persistence;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.ValueTypes;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// P0 matching preferences: self read model vs employer-safe read model and persistence.
    /// </summary>
    public sealed class WorkerMatchingPreferencesReadModelTests
    {
        #region Methods

        [Fact]
        public async Task UpdateWorkerMatchingPreferences_then_GetWorkerSelfDetail_includes_matching_fields()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            (Worker worker, JobCategory category) = await SeedWorkerWithCategoryAsync(db);
            executionContext.ReplaceClaim("worker_id", worker.Id.ToString());

            var updateHandler = new UpdateWorkerMatchingPreferencesCommandHandler(sp);
            await ((IRequestHandler<UpdateWorkerMatchingPreferencesCommand, Unit>)updateHandler).HandleAsync(
                new UpdateWorkerMatchingPreferencesCommand
                {
                    SetExpectedSalary = true,
                    ExpectedSalaryMinAmount = 35_000m,
                    ExpectedSalaryMinCurrency = "TRY",
                    ExpectedSalaryMaxAmount = 42_000m,
                    ExpectedSalaryMaxCurrency = "TRY",
                    SetInterestedJobCategories = true,
                    InterestedJobCategoryIds = [category.Id],
                },
                CancellationToken.None);

            var selfHandler = new GetWorkerSelfDetailQueryHandler(sp);
            WorkerSelfDetailModel self = await ((IRequestHandler<GetWorkerSelfDetailQuery, WorkerSelfDetailModel>)selfHandler).HandleAsync(
                new GetWorkerSelfDetailQuery(),
                CancellationToken.None);

            self.ExpectedSalaryMin.Should().Be(new Money(35_000m, "TRY"));
            self.ExpectedSalaryMax.Should().Be(new Money(42_000m, "TRY"));
            self.ProfileCompletionPercent.Should().Be(26);
            self.InterestedJobCategories.Should().ContainSingle(x =>
                x.JobCategoryId == category.Id && x.Name == category.Name);
        }

        [Fact]
        public async Task GetWorkerById_employer_model_excludes_matching_fields_when_linked_by_application()
        {
            var employerContext = new TestExecutionContext(isAuthenticated: true);

            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(employerContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            SeedEmployerWorkerApplicationResult seed = await SeedEmployerWorkerApplicationAsync(db);
            employerContext.ReplaceClaim("employer_id", seed.EmployerId.ToString());

            Worker worker = await db.Set<Worker>().SingleAsync(x => x.Id == seed.WorkerId);
            worker.UpdateExpectedSalaryRange(new Money(3000m, "EUR"), new Money(5000m, "EUR"));
            worker.ReplaceInterestedJobCategories([seed.CategoryId]);
            await db.SaveChangesAsync();

            var handler = new GetWorkerByIdQueryHandler(sp);
            WorkerEmployerSafeDetailModel model = await ((IRequestHandler<GetWorkerByIdQuery, WorkerEmployerSafeDetailModel>)handler).HandleAsync(
                new GetWorkerByIdQuery { WorkerId = seed.WorkerId },
                CancellationToken.None);

            model.Id.Should().Be(seed.WorkerId);
            model.GetType().GetProperty("ExpectedSalaryMin").Should().BeNull();
            model.GetType().GetProperty("InterestedJobCategories").Should().BeNull();
        }

        [Fact]
        public async Task GetWorkerById_denies_employer_without_shared_application()
        {
            var employerContext = new TestExecutionContext(isAuthenticated: true);

            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(employerContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            SeedEmployerWorkerApplicationResult seed = await SeedEmployerWorkerApplicationAsync(db);

            Employer other = new("Other Co", null, "9876543210");
            other.SetAddress(new Address("X", "Istanbul", "TR"));
            other.SetContact(new Contact("A", "B", "other@test.local", "+905550000001"));
            db.Set<Employer>().Add(other);
            await db.SaveChangesAsync();
            other.SetAsActive();
            await db.SaveChangesAsync();

            employerContext.ReplaceClaim("employer_id", other.Id.ToString());

            var handler = new GetWorkerByIdQueryHandler(sp);
            Func<Task> act = async () =>
                await ((IRequestHandler<GetWorkerByIdQuery, WorkerEmployerSafeDetailModel>)handler).HandleAsync(
                    new GetWorkerByIdQuery { WorkerId = seed.WorkerId },
                    CancellationToken.None);

            AzoxiaException ex = (await act.Should().ThrowAsync<AzoxiaException>()).Which;
            ex.Error.Should().Be(ApplicationValidationCodes.ActorResourceAccessDenied);
        }

        #endregion Methods

        #region Utils

        private static async Task<(Worker Worker, JobCategory Category)> SeedWorkerWithCategoryAsync(AdaIsAkademiDbContext db)
        {
            var category = new JobCategory("Hospitality", null);
            db.Set<JobCategory>().Add(category);
            await db.SaveChangesAsync();

            var user = new SystemUser("worker-matching@test.local", "Password1!", SystemUserType.Worker);
            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();

            var worker = new Worker(user.Id);
            db.Set<Worker>().Add(worker);
            await db.SaveChangesAsync();
            return (worker, category);
        }

        private static async Task<SeedEmployerWorkerApplicationResult> SeedEmployerWorkerApplicationAsync(AdaIsAkademiDbContext db)
        {
            var category = new JobCategory("Warehouse", null);
            db.Set<JobCategory>().Add(category);
            await db.SaveChangesAsync();

            Employer employer = new("ACME Hiring", null, "1112223334");
            employer.SetAddress(new Address("M", "Izmir", "TR"));
            employer.SetContact(new Contact("E", "M", "emp-match@test.local", "+905551112200"));
            EmployerLocation location = employer.AddLocation("Depot");
            location.SetAddress(new Address("Depot 1", "Izmir", "TR"));
            location.SetCoordinate(new GeoCoordinate(38.4189, 27.1287));
            location.SetGeofenceRadiusMetres(500);
            employer.SetAsActive();
            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();

            var workerUser = new SystemUser("applicant@test.local", "Password1!", SystemUserType.Worker);
            db.Set<SystemUser>().Add(workerUser);
            await db.SaveChangesAsync();

            var worker = new Worker(workerUser.Id);
            db.Set<Worker>().Add(worker);
            await db.SaveChangesAsync();

            JobPosting posting = employer.AddJobPosting(
                location.Id,
                category.Id,
                "Stocker",
                "Move boxes",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                new TimeOnly(8, 0),
                new TimeOnly(17, 0),
                new Money(100m, "TRY"),
                2);
            posting.Publish();
            posting.AddApplication(worker.Id, hasConflictingShift: false);
            await db.SaveChangesAsync();

            return new SeedEmployerWorkerApplicationResult(employer.Id, worker.Id, category.Id);
        }

        private sealed record SeedEmployerWorkerApplicationResult(int EmployerId, int WorkerId, int CategoryId);

        #endregion Utils
    }
}
