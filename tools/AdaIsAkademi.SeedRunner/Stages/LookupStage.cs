namespace Azoxia.AdaIsAkademi.SeedRunner.Stages;

using Azoxia.AdaIsAkademi.Domain;
using Azoxia.AdaIsAkademi.Persistence;
using Azoxia.AdaIsAkademi.SeedRunner.Generators;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Seeds hierarchical <see cref="JobCategory"/> rows and the demo admin account.
/// </summary>
internal static class LookupStage
{
    #region Fields

    private const string DemoAdminEmail = "admin@adaisakademi.test";

    #endregion Fields

    #region Utils

    /// <summary>
    /// Ensures categories and admin exist; populates <paramref name="state"/> keys.
    /// </summary>
    internal static async Task RunAsync(
        AdaIsAkademiDbContext db,
        SeederState state,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"[LookupStage] Kategori seed başlıyor. Toplam satır={JobCategoryCatalog.Rows.Count}.");
        int insertedCategoryCount = 0;
        int updatedCategoryParentCount = 0;

        foreach (JobCategoryCatalog.CategoryRow row in JobCategoryCatalog.Rows)
        {
            JobCategory? existing = await db.Set<JobCategory>()
                .FirstOrDefaultAsync(c => c.Name == row.Name, cancellationToken);

            if (existing is null)
            {
                var category = new JobCategory(row.Name, row.Description);
                db.Set<JobCategory>().Add(category);
                await db.SaveChangesAsync(cancellationToken);

                if (row.ParentKey is not null
                    && state.CategoryIdByKey.TryGetValue(row.ParentKey, out int parentId))
                {
                    category.SetParent(parentId);
                    await db.SaveChangesAsync(cancellationToken);
                    updatedCategoryParentCount++;
                }

                state.CategoryIdByKey[row.Key] = category.Id;
                insertedCategoryCount++;
            }
            else
            {
                state.CategoryIdByKey[row.Key] = existing.Id;
                if (row.ParentKey is not null
                    && state.CategoryIdByKey.TryGetValue(row.ParentKey, out int parentId)
                    && existing.ParentId != parentId)
                {
                    existing.SetParent(parentId);
                    await db.SaveChangesAsync(cancellationToken);
                    updatedCategoryParentCount++;
                }
            }
        }

        Console.WriteLine(
            $"[LookupStage] Kategori seed tamamlandı. inserted={insertedCategoryCount}, parentUpdated={updatedCategoryParentCount}, mapCount={state.CategoryIdByKey.Count}.");

        SystemUser? adminUser = await db.Set<SystemUser>()
            .FirstOrDefaultAsync(u => u.Email == DemoAdminEmail, cancellationToken);

        if (adminUser is null)
        {
            adminUser = new SystemUser(DemoAdminEmail, SeedConstants.DefaultPassword, SystemUserType.Admin);
            adminUser.Update("Seed", "Admin", phone: null);
            adminUser.Reactivate();
            db.Set<SystemUser>().Add(adminUser);
            await db.SaveChangesAsync(cancellationToken);
        }

        Console.WriteLine($"[LookupStage] Demo admin kontrolü tamamlandı. email={DemoAdminEmail}");
    }

    #endregion Utils
}
