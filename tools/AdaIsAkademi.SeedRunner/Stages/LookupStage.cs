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

    private const string DemoAdminPassword = "Ada!Test123";

    private const string DefaultAdminGroupName = "Default Admin";

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
                }

                state.CategoryIdByKey[row.Key] = category.Id;
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
                }
            }
        }

        SystemUser? adminUser = await db.Set<SystemUser>()
            .FirstOrDefaultAsync(u => u.Email == DemoAdminEmail, cancellationToken);

        if (adminUser is null)
        {
            adminUser = new SystemUser(DemoAdminEmail, DemoAdminPassword, SystemUserType.Admin);
            adminUser.Update("Seed", "Admin", phone: null);
            adminUser.Reactivate();
            db.Set<SystemUser>().Add(adminUser);
            await db.SaveChangesAsync(cancellationToken);

            SystemUserGroup? adminGroup = await db.Set<SystemUserGroup>()
                .FirstOrDefaultAsync(g => g.Name == DefaultAdminGroupName, cancellationToken);

            if (adminGroup is not null)
            {
                bool hasMembership = await db.Set<SystemUserGroupMembership>()
                    .AnyAsync(
                        m => m.SystemUserGroupId == adminGroup.Id
                            && m.SystemUserId == adminUser.Id
                            && m.ScopeType == MembershipScopeType.Global
                            && m.ScopeId == null,
                        cancellationToken);

                if (!hasMembership)
                {
                    db.Set<SystemUserGroupMembership>()
                        .Add(new SystemUserGroupMembership(adminGroup.Id, adminUser.Id));
                    await db.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }

    #endregion Utils
}
