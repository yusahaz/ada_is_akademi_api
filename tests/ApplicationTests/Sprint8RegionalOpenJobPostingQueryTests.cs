namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application.Tests.Support;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.AdaIsAkademi.Persistence;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.ValueTypes;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Sprint 8 tests for regional filtering on open job postings.
    /// </summary>
    public sealed class Sprint8RegionalOpenJobPostingQueryTests
    {
        [Fact]
        public async Task ListOpenJobPostingsQueryHandler_filters_by_country_code_when_provided()
        {
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider();
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            await SeedOpenPostingAsync(db, "TR", "TR Posting");
            await SeedOpenPostingAsync(db, "DE", "DE Posting");

            var handler = new ListOpenJobPostingsQueryHandler(sp);
            PagedQueryResultModel<JobPostingSummaryModel> result =
                await ((IRequestHandler<ListOpenJobPostingsQuery, PagedQueryResultModel<JobPostingSummaryModel>>)handler)
                    .HandleAsync(
                        new ListOpenJobPostingsQuery
                        {
                            CountryCode = "TR",
                            Limit = 10,
                            Offset = 0,
                        },
                        CancellationToken.None);

            result.Items.Should().HaveCount(1);
            result.Items[0].Title.Should().Be("TR Posting");
        }

        private static async Task SeedOpenPostingAsync(AdaIsAkademiDbContext db, string country, string title)
        {
            JobCategory category = new("Retail", null);
            db.Set<JobCategory>().Add(category);
            await db.SaveChangesAsync();

            Employer employer = new($"ACME-{country}", null, $"{DateTime.UtcNow.Ticks % 10000000000:D10}");
            employer.SetAddress(new Address("Merkez Mah. 1", "City", country));
            employer.SetContact(new Contact("Ali", "Yilmaz", $"regional-{country}@test.local", "+905551112233"));
            EmployerLocation hq = employer.AddLocation($"HQ-{country}");
            hq.SetAddress(new Address("Depo Sok. 2", "City", country));
            hq.SetCoordinate(new GeoCoordinate(41.015137, 28.97953));
            hq.SetGeofenceRadiusMetres(1000);
            employer.SetAsActive();
            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync();

            JobPosting posting = employer.AddJobPosting(
                hq.Id,
                category.Id,
                title,
                "Açıklama",
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                new Money(120m, "TRY"),
                1);
            posting.Publish();
            await db.SaveChangesAsync();
        }
    }
}
