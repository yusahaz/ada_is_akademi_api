namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application.Tests.Support;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.AdaIsAkademi.Persistence;
    using Azoxia.Core.Application;
    using Azoxia.Core.ValueTypes;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Tests embedding refresh sweep command behavior.
    /// </summary>
    public sealed class Sprint4EmbeddingRefreshCommandTests
    {
        #region Methods

        [Fact]
        public async Task RunEmbeddingRefreshSweepCommand_updates_worker_and_posting_embeddings()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();
            await SeedWorkerAndPostingAsync(db);

            var handler = new RunEmbeddingRefreshSweepCommandHandler(sp);
            int updatedCount = await ((IRequestHandler<RunEmbeddingRefreshSweepCommand, int>)handler).HandleAsync(
                new RunEmbeddingRefreshSweepCommand(),
                CancellationToken.None);

            updatedCount.Should().BeGreaterThan(0);

            Worker worker = await db.Set<Worker>().AsNoTracking().FirstAsync();
            worker.SkillEmbedding.Should().NotBeNull();
            worker.SkillEmbedding!.Length.Should().Be(64);
            worker.EmbeddingUpdatedAt.Should().NotBeNull();

            JobPosting posting = await db.Set<JobPosting>().AsNoTracking().FirstAsync();
            posting.DescriptionEmbedding.Should().NotBeNull();
            posting.DescriptionEmbedding!.Length.Should().Be(64);
        }

        #endregion Methods

        #region Utils

        private static async Task SeedWorkerAndPostingAsync(AdaIsAkademiDbContext db)
        {
            JobCategory category = new("Retail", null);
            db.Set<JobCategory>().Add(category);
            await db.SaveChangesAsync();

            Employer employer = new("ACME", null, "1234567890");
            employer.SetAddress(new Address("Merkez Mah. 1", "Istanbul", "TR"));
            employer.SetContact(new Contact("Ali", "Yilmaz", "embed-refresh@test.local", "+905551112233"));
            EmployerLocation location = employer.AddLocation("HQ");
            location.SetAddress(new Address("Depo Sok. 2", "Istanbul", "TR"));
            location.SetCoordinate(new GeoCoordinate(41.015137, 28.97953));
            location.SetGeofenceRadiusMetres(1000);
            employer.SetAsActive();
            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();

            SystemUser user = new("worker-embed-refresh@test.local", "Password1!", SystemUserType.Worker);
            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();

            Worker worker = new(user.Id);
            worker.AddSkill("kasiyer");
            db.Set<Worker>().Add(worker);
            await db.SaveChangesAsync();

            JobPosting posting = employer.AddJobPosting(
                location.Id,
                category.Id,
                "Kasiyer",
                "Market vardiyası",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                1);
            posting.AddSkill("kasiyer", true);
            posting.Publish();
            await db.SaveChangesAsync();
        }

        #endregion Utils
    }
}
