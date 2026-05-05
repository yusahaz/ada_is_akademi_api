namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application.Tests.Support;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.AdaIsAkademi.Persistence;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.ValueTypes;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Query tests for Sprint 4 semantic matching freshness guard and fallback behavior.
    /// </summary>
    public sealed class Sprint4SemanticMatchingQueryTests
    {
        #region Methods

        [Fact]
        public async Task ListSemanticMatchedJobPostingsQueryHandler_returns_fallback_rows_when_worker_embedding_stale()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            (int workerId, int olderPostingId, int newerPostingId) = await SeedWorkerAndOpenPostingsAsync(db);
            executionContext.ReplaceClaim("worker_id", workerId.ToString());

            Worker worker = await db.Set<Worker>().FirstAsync(x => x.Id == workerId);
            db.Entry(worker).Property(x => x.SkillEmbedding).CurrentValue = new float[] { 1f, 0f, 0f };
            db.Entry(worker).Property(x => x.EmbeddingUpdatedAt).CurrentValue = DateTimeOffset.UtcNow.AddDays(-45);
            await db.SaveChangesAsync();

            var handler = new ListSemanticMatchedJobPostingsQueryHandler(sp);
            IReadOnlyList<SemanticMatchedJobPostingModel> rows = await ((IRequestHandler<ListSemanticMatchedJobPostingsQuery, IReadOnlyList<SemanticMatchedJobPostingModel>>)handler).HandleAsync(
                new ListSemanticMatchedJobPostingsQuery
                {
                    Limit = 10,
                },
                CancellationToken.None);

            rows.Should().HaveCount(2);
            rows.Select(x => x.JobPostingId).Should().ContainInOrder(olderPostingId, newerPostingId);
            rows.All(x => x.SimilarityScore == 0d).Should().BeTrue();
        }

        [Fact]
        public async Task ListSemanticMatchedJobPostingsQueryHandler_filters_rows_by_worker_availability_when_embedding_is_fresh()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            (int workerId, int mondayPostingId, int tuesdayPostingId) = await SeedWorkerAndOpenPostingsAsync(db);
            executionContext.ReplaceClaim("worker_id", workerId.ToString());

            Worker worker = await db.Set<Worker>().FirstAsync(x => x.Id == workerId);
            db.Entry(worker).Property(x => x.SkillEmbedding).CurrentValue = new float[] { 1f, 0f, 0f };
            db.Entry(worker).Property(x => x.EmbeddingUpdatedAt).CurrentValue = DateTimeOffset.UtcNow;
            worker.AddAvailability(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(20, 0));

            JobPosting mondayPosting = await db.Set<JobPosting>().FirstAsync(x => x.Id == mondayPostingId);
            JobPosting tuesdayPosting = await db.Set<JobPosting>().FirstAsync(x => x.Id == tuesdayPostingId);
            db.Entry(mondayPosting).Property(x => x.DescriptionEmbedding).CurrentValue = new float[] { 1f, 0f, 0f };
            db.Entry(tuesdayPosting).Property(x => x.DescriptionEmbedding).CurrentValue = new float[] { 0f, 1f, 0f };
            await db.SaveChangesAsync();

            var handler = new ListSemanticMatchedJobPostingsQueryHandler(sp);
            IReadOnlyList<SemanticMatchedJobPostingModel> rows = await ((IRequestHandler<ListSemanticMatchedJobPostingsQuery, IReadOnlyList<SemanticMatchedJobPostingModel>>)handler).HandleAsync(
                new ListSemanticMatchedJobPostingsQuery
                {
                    Limit = 10,
                },
                CancellationToken.None);

            rows.Should().HaveCount(1);
            rows.Single().JobPostingId.Should().Be(mondayPostingId);
        }

        #endregion Methods

        #region Utils

        private static async Task<(int workerId, int olderPostingId, int newerPostingId)> SeedWorkerAndOpenPostingsAsync(AdaIsAkademiDbContext db)
        {
            JobCategory category = new("Retail", null);
            db.Set<JobCategory>().Add(category);
            await db.SaveChangesAsync();

            Employer employer = new("ACME", null, "1234567890");
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", "employer-sprint4@test.local", "+905551112233"));
            EmployerLocation location = employer.AddLocation("HQ");
            location.SetAddress(new Address("Depo Sok. 2", "Istanbul", "TR"));
            location.SetCoordinate(new GeoCoordinate(41.015137, 28.97953));
            location.SetGeofenceRadiusMetres(1000);
            employer.SetAsActive();
            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();

            SystemUser user = new("worker-sprint4@test.local", "Password1!", SystemUserType.Worker);
            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();

            Worker worker = new(user.Id);
            db.Set<Worker>().Add(worker);
            await db.SaveChangesAsync();

            DateOnly nextMonday = ResolveNextDay(DayOfWeek.Monday);
            DateOnly nextTuesday = ResolveNextDay(DayOfWeek.Tuesday);

            JobPosting olderPosting = employer.AddJobPosting(
                location.Id,
                category.Id,
                "Kasiyer 1",
                "Açıklama 1",
                nextMonday,
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                1);
            olderPosting.Publish();

            JobPosting newerPosting = employer.AddJobPosting(
                location.Id,
                category.Id,
                "Kasiyer 2",
                "Açıklama 2",
                nextTuesday,
                new TimeOnly(10, 0),
                new TimeOnly(19, 0),
                new Money(120m, "TRY"),
                1);
            newerPosting.Publish();

            await db.SaveChangesAsync();
            return (worker.Id, olderPosting.Id, newerPosting.Id);
        }

        private static DateOnly ResolveNextDay(DayOfWeek dayOfWeek)
        {
            DateTime utcNow = DateTime.UtcNow.Date;
            int offset = ((int)dayOfWeek - (int)utcNow.DayOfWeek + 7) % 7;
            offset = offset == 0 ? 7 : offset;
            return DateOnly.FromDateTime(utcNow.AddDays(offset));
        }

        #endregion Utils
    }
}
