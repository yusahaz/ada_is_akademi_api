namespace Azoxia.AdaIsAkademi.SeedRunner.Stages;

using Azoxia.AdaIsAkademi.Domain;
using Azoxia.AdaIsAkademi.Persistence;
using Azoxia.AdaIsAkademi.SeedRunner.Generators;
using Azoxia.Core.ValueTypes;
using Bogus;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Creates demo workers and employers with locations and supervisors.
/// </summary>
internal static class WorkforceStage
{
    #region Utils

    /// <summary>
    /// Seeds workers and employers when the seed marker user is absent.
    /// </summary>
    internal static async Task RunAsync(
        AdaIsAkademiDbContext db,
        SeederState state,
        SeedOptions options,
        Random rnd,
        Faker faker,
        ObjectStorageMediaUploader? mediaUploader,
        CancellationToken cancellationToken)
    {
        string[] clusters =
        [
            JobCategoryCatalog.ClusterFood,
            JobCategoryCatalog.ClusterLogistics,
            JobCategoryCatalog.ClusterRetail,
            JobCategoryCatalog.ClusterEvent,
            JobCategoryCatalog.ClusterCleaning,
            JobCategoryCatalog.ClusterOffice,
        ];

        Console.WriteLine(
            $"[WorkforceStage] Workforce seed başlıyor. targetWorkers={options.Workers}, targetEmployers={options.Employers}");

        for (int i = 1; i <= options.Workers; i++)
        {
            string cluster = clusters[rnd.Next(clusters.Length)];
            SystemUser user = new(WorkerEmail(i), SeedConstants.DefaultPassword, SystemUserType.Worker);
            user.Update(faker.Name.FirstName(), faker.Name.LastName(), PhoneTurkey(faker));
            user.Reactivate();
            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync(cancellationToken);

            Worker worker = new(user.Id);
            worker.UpdateProfile("TR", faker.Company.CompanyName() + " Üniversitesi");
            decimal min = rnd.Next(14_000, 22_000);
            decimal max = min + rnd.Next(2_000, 8_000);
            worker.UpdateExpectedSalaryRange(new Money(min, "TRY"), new Money(max, "TRY"));

            db.Set<Worker>().Add(worker);
            await db.SaveChangesAsync(cancellationToken);

            IReadOnlyList<string> pool = SkillCatalog.GetTagsForCluster(cluster);
            int skillCount = rnd.Next(5, 10);
            HashSet<string> tags = new(StringComparer.Ordinal);
            while (tags.Count < skillCount)
            {
                tags.Add(pool[rnd.Next(pool.Count)]);
            }

            if (rnd.NextDouble() < 0.20)
            {
                string otherCluster = clusters[rnd.Next(clusters.Length)];
                if (otherCluster != cluster)
                {
                    IReadOnlyList<string> otherPool = SkillCatalog.GetTagsForCluster(otherCluster);
                    tags.Add(otherPool[rnd.Next(otherPool.Count)]);
                }
            }

            foreach (string tag in tags)
            {
                worker.AddSkill(tag);
            }

            List<int> interested = LeafCategoryIds(state, cluster);
            if (interested.Count == 0)
            {
                interested = JobCategoryCatalog.Rows
                    .Where(r => r.ParentKey is not null && state.CategoryIdByKey.ContainsKey(r.Key))
                    .Select(r => state.CategoryIdByKey[r.Key])
                    .ToList();
            }

            List<int> picked = PickDistinct(rnd, interested, rnd.Next(1, Math.Min(4, interested.Count + 1)));
            if (picked.Count > 0)
            {
                worker.ReplaceInterestedJobCategories(picked);
            }

            AddWeeklyAvailability(worker, rnd);
            AddEducation(worker, rnd, faker);
            AddExperience(worker, rnd, faker);
            worker.AddLanguage("Türkçe", LanguageLevel.Native);
            if (rnd.NextDouble() < 0.55)
            {
                worker.AddLanguage("İngilizce", (LanguageLevel)rnd.Next(20, 51));
            }

            if (rnd.NextDouble() < 0.30)
            {
                worker.AddCertificate(
                    "İş güvenliği eğitimi",
                    "İSG Katılım",
                    DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                    expiresAt: null);
            }

            if (rnd.NextDouble() < 0.20)
            {
                worker.AddReference(
                    faker.Company.CompanyName(),
                    "İK Uzmanı",
                    new Contact(faker.Name.FirstName(), faker.Name.LastName(), faker.Internet.Email(), PhoneTurkey(faker)));
            }

            MaybeSetWorkerBio(worker, rnd, faker);
            MaybeSetWorkerSocialLinks(worker, rnd, faker);
            worker.SetProfilePhotoObjectKey($"seed/demo/workers/worker-{i:D3}/profile.jpg");

            string embedSeed = string.Join('|', tags.OrderBy(x => x));
            worker.UpdateSkillEmbedding(EmbeddingFaker.GenerateDeterministic(embedSeed));

            state.Workers.Add(new SeederState.WorkerSeed
            {
                Cluster = cluster,
                SkillTags = tags.ToList(),
                User = user,
                Worker = worker,
            });

            if (i % 25 == 0 || i == options.Workers)
            {
                Console.WriteLine($"[WorkforceStage] Worker ilerleme: {i}/{options.Workers}");
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        if (mediaUploader is not null && mediaUploader.CanUploadForScale(options))
        {
            Console.WriteLine("[WorkforceStage] MinIO: worker profil görselleri yükleniyor...");
            await mediaUploader.SeedWorkerAvatarsAsync(state, cancellationToken);
        }

        (string City, string Country, double Lat, double Lon)[] geo =
        [
            ("İstanbul", "TR", 41.015137, 28.979530),
            ("Ankara", "TR", 39.9334, 32.8597),
            ("İzmir", "TR", 38.4237, 27.1428),
            ("Bursa", "TR", 40.1826, 29.0665),
            ("Antalya", "TR", 36.8969, 30.7133),
        ];

        for (int e = 1; e <= options.Employers; e++)
        {
            string tax = $"{rnd.Next(100_000_000, 999_999_999):D9}";
            var employer = new Employer(faker.Company.CompanyName() + " A.Ş.", "Seed işveren kaydı.", tax);
            employer.SetAddress(new Address($"{faker.Address.StreetAddress()}", geo[e % geo.Length].City, geo[e % geo.Length].Country));
            employer.SetContact(new Contact(faker.Name.FirstName(), faker.Name.LastName(), EmployerEmail(e), PhoneTurkey(faker)));
            employer.SetCommissionRate(Math.Round((decimal)(0.08 + rnd.NextDouble() * 0.07), 4));
            employer.SetAsActive();
            db.Set<Employer>().Add(employer);
            await db.SaveChangesAsync(cancellationToken);

            MaybeDecorateEmployerProfile(employer, e, rnd, faker);

            SystemUser primary = new(EmployerEmail(e), SeedConstants.DefaultPassword, SystemUserType.Employer);
            primary.Update(faker.Name.FirstName(), faker.Name.LastName(), PhoneTurkey(faker));
            primary.Reactivate();
            db.Set<SystemUser>().Add(primary);
            await db.SaveChangesAsync(cancellationToken);

            primary.BindToEmployerOrganization(employer.Id);
            await db.SaveChangesAsync(cancellationToken);
            List<SystemUser> extraSupervisors = [];
            if (rnd.NextDouble() < 0.50)
            {
                SystemUser sup = new($"employer{e:D2}-super2@adaisakademi.seed.local", SeedConstants.DefaultPassword, SystemUserType.Supervisor);
                sup.Update(faker.Name.FirstName(), faker.Name.LastName(), PhoneTurkey(faker));
                sup.Reactivate();
                db.Set<SystemUser>().Add(sup);
                await db.SaveChangesAsync(cancellationToken);
                sup.InitializeEmployerScopedSupervisor(employer.Id);
                employer.AddSupervisor(sup.Id);
                extraSupervisors.Add(sup);
            }

            int locCount = rnd.Next(1, 4);
            List<EmployerLocation> locs = [];
            for (int l = 0; l < locCount; l++)
            {
                (string City, string Country, double Lat, double Lon) g = geo[(e + l) % geo.Length];
                EmployerLocation loc = employer.AddLocation($"{g.City} Şube {l + 1}");
                loc.SetAddress(new Address(faker.Address.StreetAddress(), g.City, g.Country));
                loc.SetCoordinate(new GeoCoordinate(g.Lat + rnd.NextDouble() * 0.04 - 0.02, g.Lon + rnd.NextDouble() * 0.04 - 0.02));
                loc.SetGeofenceRadiusMetres(rnd.Next(400, 1200));
                locs.Add(loc);
            }

            await db.SaveChangesAsync(cancellationToken);

            state.Employers.Add(new SeederState.EmployerSeed
            {
                Employer = employer,
                Locations = locs,
                PrimaryUser = primary,
                ExtraSupervisorUsers = extraSupervisors,
            });

            if (e % 10 == 0 || e == options.Employers)
            {
                Console.WriteLine($"[WorkforceStage] Employer ilerleme: {e}/{options.Employers}");
            }
        }

        if (mediaUploader is not null && mediaUploader.CanUploadForScale(options))
        {
            Console.WriteLine("[WorkforceStage] MinIO: işveren logoları yükleniyor...");
            await mediaUploader.SeedEmployerLogosAsync(state, cancellationToken);
        }

        Console.WriteLine(
            $"[WorkforceStage] Workforce seed tamamlandı. workers={state.Workers.Count}, employers={state.Employers.Count}");
    }

    private static void AddEducation(Worker worker, Random rnd, Faker faker)
    {
        EducationType[] types = [EducationType.HighSchool, EducationType.VocationalCourse, EducationType.AssociateDegree, EducationType.BachelorDegree];
        EducationType t = types[rnd.Next(types.Length)];
        int start = rnd.Next(2008, 2018);
        worker.AddEducation(faker.Company.CompanyName(), faker.Commerce.Department(), t, start, start + (t >= EducationType.BachelorDegree ? 4 : 2), isOngoing: false);
    }

    private static void AddExperience(Worker worker, Random rnd, Faker faker)
    {
        DateOnly start = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-rnd.Next(1, 6)));
        DateOnly? end = rnd.NextDouble() < 0.7 ? DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-rnd.Next(1, 24))) : null;
        worker.AddExperience(faker.Company.CompanyName(), faker.Name.JobTitle(), start, end, description: "Geçici dönem proje desteği.");
    }

    private static string AsHttpsUrl(Faker faker)
    {
        string raw = faker.Internet.Url();
        if (raw.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return raw;
        }

        if (raw.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
        {
            return "https://" + raw.AsSpan("http://".Length).ToString();
        }

        return "https://" + raw.TrimStart('/');
    }

    private static void MaybeDecorateEmployerProfile(Employer employer, int employerIndex, Random rnd, Faker faker)
    {
        if (rnd.NextDouble() < 0.65)
        {
            employer.ReplaceSocialLinks(
            [
                new EmployerSocialLinkInput(SocialMediaPlatform.Website, AsHttpsUrl(faker)),
                new EmployerSocialLinkInput(
                    SocialMediaPlatform.LinkedIn,
                    "https://www.linkedin.com/company/" + faker.Random.AlphaNumeric(10)),
            ]);
        }

        employer.SetLogoObjectKey($"seed/demo/employers/employer-{employerIndex:D2}/logo.png");
    }

    private static void MaybeSetWorkerBio(Worker worker, Random rnd, Faker faker)
    {
        if (rnd.NextDouble() < 0.65)
        {
            worker.UpdateBio(faker.Lorem.Paragraph());
        }
    }

    private static void MaybeSetWorkerSocialLinks(Worker worker, Random rnd, Faker faker)
    {
        if (rnd.NextDouble() > 0.45)
        {
            return;
        }

        List<WorkerSocialLinkInput> links =
        [
            new WorkerSocialLinkInput(
                SocialMediaPlatform.LinkedIn,
                "https://www.linkedin.com/in/" + faker.Random.AlphaNumeric(12)),
        ];

        if (rnd.NextDouble() < 0.55)
        {
            links.Add(new WorkerSocialLinkInput(SocialMediaPlatform.Website, AsHttpsUrl(faker)));
        }

        worker.ReplaceSocialLinks(links);
    }

    private static void AddWeeklyAvailability(Worker worker, Random rnd)
    {
        int slots = rnd.Next(3, 6);
        var days = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday };
        var seen = new HashSet<(DayOfWeek Day, int Start, int End)>();
        for (int s = 0; s < slots; s++)
        {
            DayOfWeek d = days[rnd.Next(days.Length)];
            int h1 = rnd.Next(8, 12);
            int h2 = rnd.Next(14, 20);
            var key = (d, h1, h2);
            if (!seen.Add(key))
            {
                continue;
            }

            worker.AddAvailability(d, new TimeOnly(h1, 0), new TimeOnly(h2, 0));
        }
    }

    private static string EmployerEmail(int index)
        => $"employer{index:D2}@adaisakademi.seed.local";

    private static List<int> LeafCategoryIds(SeederState state, string cluster)
    {
        List<int> ids = [];
        foreach (JobCategoryCatalog.CategoryRow row in JobCategoryCatalog.Rows)
        {
            if (row.ParentKey is not null && row.Cluster == cluster && state.CategoryIdByKey.TryGetValue(row.Key, out int id))
            {
                ids.Add(id);
            }
        }

        return ids;
    }

    private static string PhoneTurkey(Faker faker)
        => "+90" + faker.Random.Number(530, 559).ToString() + faker.Random.Number(1000000, 9999999).ToString();

    private static List<int> PickDistinct(Random rnd, List<int> source, int count)
    {
        if (source.Count == 0 || count <= 0)
        {
            return [];
        }

        count = Math.Min(count, source.Count);
        return source.OrderBy(_ => rnd.Next()).Take(count).ToList();
    }

    private static string WorkerEmail(int index)
        => $"worker{index:D3}@adaisakademi.seed.local";

    #endregion Utils
}
